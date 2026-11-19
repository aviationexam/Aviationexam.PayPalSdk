using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Aviationexam.PreprocessOpenApi;

/// <summary>
/// Preprocesses PayPal's OpenAPI v3 specifications before they are fed to Kiota.
///
/// PayPal's spec (since the 2026-04-07 update, commit
/// paypal/paypal-rest-api-specifications@9f0f5281) wraps each <c>allOf</c> entry
/// in a redundant inner <c>allOf</c> wrapper, e.g.:
/// <code>
/// allOf:
///   - allOf: [ { $ref: '#/components/schemas/x' }, { title: 'x' } ]
///   - type: object
///     properties: ...
/// </code>
/// Kiota's schema flattener (all versions tested up to 1.31.1) does not handle
/// nested <c>allOf</c> wrappers and emits empty C# classes for any schema that
/// uses this pattern (Order, Order_authorize_response, Payee, Card_request, ...).
///
/// This tool walks every JSON object in the spec and unwraps redundant
/// <c>allOf</c> wrappers so Kiota sees a flat composition.
///
/// It then renames the components that would take the name of an inline type,
/// see <see cref="RenameMixinsNamedAfterInlineTypes"/>.
/// </summary>
internal static class Program
{
    private const string SchemaReferencePrefix = "#/components/schemas/";

    private const string RenamedMixinSuffix = "_fields";

    private static async Task<int> Main(string[] args)
    {
        if (args.Length != 2)
        {
            await Console.Error.WriteLineAsync("Usage: Aviationexam.PreprocessOpenApi <input.json> <output.json>");
            return 1;
        }

        var inputPath = args[0];
        var outputPath = args[1];

        if (!File.Exists(inputPath))
        {
            await Console.Error.WriteLineAsync($"Input file not found: {inputPath}");
            return 2;
        }

        JsonNode? root;
        await using (var input = File.OpenRead(inputPath))
        {
            root = await JsonNode.ParseAsync(input, documentOptions: new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            });
        }

        if (root is null)
        {
            await Console.Error.WriteLineAsync("Input is empty or invalid JSON.");
            return 3;
        }

        var stats = new Stats();
        FlattenAllOf(root, stats);
        RenameMixinsNamedAfterInlineTypes(root, stats);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        await using (var output = File.Create(outputPath))
        {
            await using var writer = new Utf8JsonWriter(output, new JsonWriterOptions
            {
                Indented = true,
            });
            root.WriteTo(writer, options);
        }

        await Console.Out.WriteLineAsync($"Preprocessed OpenAPI: flattened {stats.UnwrappedAllOfEntries} redundant allOf wrappers across {stats.SchemasTouched} schemas, renamed {stats.RenamedMixins} mixin schemas.");
        return 0;
    }

    private static void FlattenAllOf(JsonNode node, Stats stats)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var kvp in obj)
                {
                    if (kvp.Value is not null)
                    {
                        FlattenAllOf(kvp.Value, stats);
                    }
                }

                if (obj["allOf"] is JsonArray allOf)
                {
                    var changed = FlattenAllOfArray(allOf, stats);
                    if (changed)
                    {
                        stats.SchemasTouched++;
                    }
                }
                break;

            case JsonArray arr:
                foreach (var item in arr)
                {
                    if (item is not null)
                    {
                        FlattenAllOf(item, stats);
                    }
                }
                break;
        }
    }

    private static bool FlattenAllOfArray(JsonArray allOf, Stats stats)
    {
        var changed = false;
        var i = 0;
        while (i < allOf.Count)
        {
            if (allOf[i] is JsonObject candidate && IsRedundantAllOfWrapper(candidate))
            {
                var innerAllOf = candidate["allOf"]!.AsArray();

                // JsonNode forbids living in two parents simultaneously,
                // so the inner items must be detached before reinsertion.
                var innerItems = new JsonNode?[innerAllOf.Count];
                for (var j = 0; j < innerAllOf.Count; j++)
                {
                    innerItems[j] = innerAllOf[j];
                }
                innerAllOf.Clear();

                allOf.RemoveAt(i);
                for (var j = 0; j < innerItems.Length; j++)
                {
                    allOf.Insert(i + j, innerItems[j]);
                }

                stats.UnwrappedAllOfEntries++;
                changed = true;
                // Re-check this index: spliced item may itself be a wrapper.
            }
            else
            {
                i++;
            }
        }
        return changed;
    }

    /// <summary>
    /// A redundant <c>allOf</c> wrapper is a schema object whose only meaningful
    /// content is an <c>allOf</c> array. Such a wrapper adds no information and
    /// can safely be replaced by its inner items.
    ///
    /// Decorative metadata (<c>title</c>, <c>description</c>) is tolerated and
    /// silently dropped during unwrapping; any other schema keyword
    /// (<c>type</c>, <c>$ref</c>, <c>properties</c>, <c>oneOf</c>, etc.) makes
    /// the wrapper non-redundant and we leave it alone.
    /// </summary>
    private static bool IsRedundantAllOfWrapper(JsonObject candidate)
    {
        if (candidate["allOf"] is not JsonArray)
        {
            return false;
        }

        foreach (var kvp in candidate)
        {
            if (kvp.Key is "allOf" or "title" or "description")
            {
                continue;
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Kiota names a type declared inline in a property <c>{model}_{property}</c>,
    /// so the inline <c>status</c> enum of <c>capture</c> is <c>Capture_status</c>.
    /// Since Kiota 1.36.0 (microsoft/kiota#5967) that name gets a numeric suffix
    /// when a component has it, even a component Kiota does not generate.
    ///
    /// PayPal composes <c>capture</c>, <c>authorization</c> and <c>refund</c> from
    /// mixins named <c>capture_status</c> and so on, which declare the inline
    /// <c>status</c> enum. Kiota generates no class for the mixins, but their
    /// names still turn the enums into <c>Capture_status1</c> and so on.
    ///
    /// This renames each mixin named <c>{model}_{property}</c> that declares
    /// <c>{property}</c> inline, together with the references to it.
    /// </summary>
    private static void RenameMixinsNamedAfterInlineTypes(JsonNode root, Stats stats)
    {
        if (root["components"]?["schemas"] is not JsonObject schemas)
        {
            return;
        }

        var renames = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (modelName, model) in schemas)
        {
            if (model is not JsonObject modelObject || modelObject["allOf"] is not JsonArray allOf)
            {
                continue;
            }

            var mixinPrefix = modelName + "_";
            foreach (var item in allOf)
            {
                if (
                    GetSchemaReferenceName(item) is { } mixinName
                    && mixinName.StartsWith(mixinPrefix, StringComparison.OrdinalIgnoreCase)
                    && DeclaresInlineType(schemas[mixinName], mixinName[mixinPrefix.Length..])
                )
                {
                    renames[mixinName] = mixinName + RenamedMixinSuffix;
                }
            }
        }

        if (renames.Count == 0)
        {
            return;
        }

        foreach (var (oldName, newName) in renames)
        {
            if (schemas.Any(x => string.Equals(x.Key, newName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Cannot rename the schema {oldName} to {newName}: a schema with that name exists.");
            }

            var index = schemas.IndexOf(oldName);
            var schema = schemas[oldName];
            schemas.RemoveAt(index);
            schemas.Insert(index, newName, schema);
        }

        RenameSchemaReferences(root, renames);
        stats.RenamedMixins = renames.Count;
    }

    private static bool DeclaresInlineType(JsonNode? schema, string propertyName)
    {
        if (schema is not JsonObject schemaObject || schemaObject["properties"] is not JsonObject properties)
        {
            return false;
        }

        foreach (var (name, property) in properties)
        {
            if (string.Equals(name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return property is JsonObject propertySchema && IsInlineType(propertySchema);
            }
        }

        return false;
    }

    /// <summary>
    /// Kiota generates a type for an inline enum and for an inline object with
    /// properties.
    /// </summary>
    private static bool IsInlineType(JsonObject schema)
    {
        if (schema.ContainsKey("$ref"))
        {
            return false;
        }

        if (schema["enum"] is JsonArray)
        {
            return true;
        }

        return schema["type"] is JsonValue type
            && type.TryGetValue<string>(out var typeName)
            && typeName == "object"
            && schema["properties"] is JsonObject { Count: > 0 };
    }

    private static string? GetSchemaReferenceName(JsonNode? schema)
    {
        if (
            schema is JsonObject schemaObject
            && schemaObject["$ref"] is JsonValue reference
            && reference.TryGetValue<string>(out var path)
            && path.StartsWith(SchemaReferencePrefix, StringComparison.Ordinal)
        )
        {
            return path[SchemaReferencePrefix.Length..];
        }

        return null;
    }

    private static void RenameSchemaReferences(JsonNode node, IReadOnlyDictionary<string, string> renames)
    {
        switch (node)
        {
            case JsonObject obj:
                if (GetSchemaReferenceName(obj) is { } name && renames.TryGetValue(name, out var newName))
                {
                    obj["$ref"] = SchemaReferencePrefix + newName;
                }

                foreach (var kvp in obj)
                {
                    if (kvp.Value is not null)
                    {
                        RenameSchemaReferences(kvp.Value, renames);
                    }
                }
                break;

            case JsonArray arr:
                foreach (var item in arr)
                {
                    if (item is not null)
                    {
                        RenameSchemaReferences(item, renames);
                    }
                }
                break;
        }
    }

    private sealed class Stats
    {
        public int UnwrappedAllOfEntries { get; set; }

        public int SchemasTouched { get; set; }

        public int RenamedMixins { get; set; }
    }
}

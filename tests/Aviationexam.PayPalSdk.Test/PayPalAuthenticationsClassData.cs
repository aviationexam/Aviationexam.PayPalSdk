using PayPal.Sdk.Checkout.Test.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Xunit;

namespace PayPal.Sdk.Checkout.Test;

public sealed class PayPalAuthenticationsClassData() : TheoryData<PayPalAuthenticationsClassData.AuthenticationData?>(
    GetData()
)
{
    public static IEnumerable<TheoryDataRow<AuthenticationData?>> GetData()
    {
        Loader.LoadEnvFile(".env.local");

        var clientId = Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID")?.Trim();
        var clientSecret = Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET")?.Trim();

        if (
            clientId is null
            || clientSecret is null
        )
        {
            yield return new TheoryDataRow<AuthenticationData?>(null)
            {
                Skip = "Authentication data is not set. Please set PAYPAL_CLIENT_ID, and PAYPAL_CLIENT_SECRET environment variables.",
            };
            yield break;
        }

        yield return new TheoryDataRow<AuthenticationData?>(new AuthenticationData(clientId, clientSecret));
    }

    public sealed record AuthenticationData(
        string ClientId,
        string ClientSecret
    ) : IFormattable, IParsable<AuthenticationData>
    {
        public string ToString(string? format, IFormatProvider? formatProvider) => new JsonArray(
            ClientId,
            ClientSecret
        ).ToString();

        public static AuthenticationData Parse(string s, IFormatProvider? provider)
        {
            if (JsonNode.Parse(s) is not JsonArray { Count: 3 } arr)
            {
                throw new FormatException("Input string is not a valid AuthenticationData JSON array.");
            }

            return new AuthenticationData(
                arr[0]?.GetValue<string>() ?? throw new FormatException("ClientId missing."),
                arr[1]?.GetValue<string>() ?? throw new FormatException("ClientSecret missing.")
            );
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out AuthenticationData result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}

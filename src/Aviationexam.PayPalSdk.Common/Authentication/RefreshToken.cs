using System.Text.Json.Serialization;

namespace Aviationexam.PayPalSdk.Common.Authentication;

public sealed class RefreshToken
{
    [JsonPropertyName("refresh_token")]
    public required string Token { get; set; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public required string ExpiresIn { get; set; }

    [JsonPropertyName("id_token")]
    public required string IdToken { get; set; }
}

using System;
using System.Text.Json.Serialization;

namespace Aviationexam.PayPalSdk.Common.Authentication;

public sealed class TokenResponse
{
    [JsonPropertyName("scope")]
    public required string Scope { get; set; }

    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }

    [JsonPropertyName("app_id")]
    public required string AppId { get; set; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }

    [JsonPropertyName("nonce")]
    public required int Nonce { get; set; }

    public TimeSpan ExpiresInTimeSpan => TimeSpan.FromSeconds(ExpiresIn);
}

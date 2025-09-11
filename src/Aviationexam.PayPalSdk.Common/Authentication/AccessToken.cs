using System;
using System.Text.Json.Serialization;

namespace Aviationexam.PayPalSdk.Common.Authentication;

public class AccessToken(
    DateTimeOffset? receivedDate = null
)
{
    [JsonPropertyName("access_token")]
    public required string Token { get; set; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }

    private DateTimeOffset ReceivedDate { get; } = receivedDate ?? DateTimeOffset.UtcNow;

    [JsonConstructor]
    public AccessToken(
        string token, string tokenType, int expiresIn
    ) : this()
    {
        Token = token;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
    }

    public bool IsExpired(DateTimeOffset now)
    {
        var expireDate = ReceivedDate.Add(TimeSpan.FromSeconds(ExpiresIn));

        return now.CompareTo(expireDate) > 0;
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public class PayPalAuthenticationOptions
{
    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    [Required]
    public TimeSpan JwtEarlyExpirationOffset { get; set; }

    [Required]
    public Uri TokenEndpoint { get; set; } = null!;

    public string TokenPath { get; } = "/v1/oauth2/token";
}

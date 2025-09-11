using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public class PayPalAuthenticationOptions
{
    [Required]
    public string ClientId { get; set; } = null!;

    [Required]
    public string ClientSecret { get; set; } = null!;

    [Required]
    public TimeSpan JwtEarlyExpirationOffset { get; set; }

    [Required]
    public Uri TokenEndpoint { get; set; } = null!;

    public string TokenPath { get; } = "/connect/token";

    public string AuthorizationString() => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));
}

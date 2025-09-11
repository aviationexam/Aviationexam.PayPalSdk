using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Options;

namespace PayPal.Sdk.Checkout.Configuration;

public class PayPalOptions
{
    public string LiveEndpoint { get; set; } = "https://api-m.paypal.com";

    public string SandboxEndpoint { get; set; } = "https://api-m.sandbox.paypal.com";

    [Required]
    public EPayPalEnvironment Environment { get; set; } = EPayPalEnvironment.Sandbox;

    [Required]
    public string ClientId { get; set; } = null!;

    [Required] public string ClientSecret { get; set; } = null!;

    [Required] public TimeSpan Timeout { get; set; }

    public bool SendPayPalUserAgentHeader { get; set; } = true;

    [Required] public Uri Endpoint { get; set; } = null!;

    [Required] public IReadOnlyCollection<string> AllowedHosts { get; set; } = null!;

    public string AuthorizationString() => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public class PayPalResApiOptions
{
    public string LiveEndpoint { get; set; } = "https://api-m.paypal.com";

    public string SandboxEndpoint { get; set; } = "https://api-m.sandbox.paypal.com";

    [Required]
    public EPayPalEnvironment Environment { get; set; } = EPayPalEnvironment.Sandbox;

    [Required]
    public TimeSpan Timeout { get; set; }

    [Required]
    public Uri Endpoint { get; set; } = null!;

    [Required]
    public IReadOnlyCollection<string> AllowedHosts { get; set; } = null!;


}

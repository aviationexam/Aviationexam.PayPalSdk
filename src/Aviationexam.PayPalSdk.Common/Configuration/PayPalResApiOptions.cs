using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public class PayPalResApiOptions
{
    public required EPayPalEnvironment Environment { get; set; }

    [Required]
    public TimeSpan Timeout { get; set; }

    [Required]
    public Uri Endpoint { get; set; } = null!;

    [Required]
    public IReadOnlyCollection<string> AllowedHosts { get; set; } = null!;
}

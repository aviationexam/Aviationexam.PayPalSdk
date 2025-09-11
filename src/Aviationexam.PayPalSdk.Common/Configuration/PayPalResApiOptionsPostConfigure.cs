using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public sealed class PayPalResApiOptionsPostConfigure : IPostConfigureOptions<PayPalResApiOptions>
{
    public void PostConfigure(string? name, PayPalResApiOptions options)
    {
        if (options.Environment is EPayPalEnvironment.Live)
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            options.Endpoint ??= new Uri("https://api-m.paypal.com", UriKind.Absolute);
        }
        else if (options.Environment is EPayPalEnvironment.Sandbox)
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            options.Endpoint ??= new Uri("https://api-m.sandbox.paypal.com", UriKind.Absolute);
        }

        if (options is { Endpoint: { } endpoint })
        {
            options.AllowedHosts ??= // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            [
                endpoint.Authority,
            ];
        }
    }
}

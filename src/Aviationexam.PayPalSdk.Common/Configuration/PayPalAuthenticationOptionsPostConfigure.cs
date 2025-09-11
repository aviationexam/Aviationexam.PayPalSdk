using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public sealed class PayPalAuthenticationOptionsPostConfigure(
    IOptions<PayPalResApiOptions> authenticationOptions
) : IPostConfigureOptions<PayPalAuthenticationOptions>
{
    public void PostConfigure(string? name, PayPalAuthenticationOptions options)
    {
        if (options.JwtEarlyExpirationOffset == TimeSpan.Zero)
        {
            options.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(20);
        }

        if (authenticationOptions.Value is { Endpoint: { } endpoint })
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            options.TokenEndpoint ??= new Uri(endpoint, options.TokenPath);
        }
    }
}

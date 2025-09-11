using Aviationexam.PayPalSdk.Common.Authentication;
using Aviationexam.PayPalSdk.Common.Configuration;
using Aviationexam.PayPalSdk.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.PayPalSdk.Common.KiotaServices;

public sealed class DefaultAccessTokenProvider(
    [FromKeyedServices(DependencyInjectionExtensions.PayPalServiceKey)]
    IPayPalAccessTokenProvider accessTokenProvider,
    IOptions<PayPalResApiOptions> payPalOptions
) : IAccessTokenProvider
{
    public Task<string> GetAuthorizationTokenAsync(
        Uri uri, Dictionary<string, object>? additionalAuthenticationContext, CancellationToken cancellationToken
    ) => accessTokenProvider.GetAuthorizationTokenAsync(cancellationToken).AsTask();

    public AllowedHostsValidator AllowedHostsValidator { get; } = new(payPalOptions.Value.AllowedHosts);
}

using Aviationexam.PayPalSdk.Common.Configuration;
using Aviationexam.PayPalSdk.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.PayPalSdk.Common.Authentication;

public sealed class PayPalAccessTokenProvider(
    [FromKeyedServices(DependencyInjectionExtensions.PayPalHttpTokenClient)]
    HttpClient httpClient,
    TimeProvider timeProvider,
    IOptions<PayPalAuthenticationOptions> authenticationOptions
) : IPayPalAccessTokenProvider
{
    public ValueTask<string> GetAuthorizationTokenAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}

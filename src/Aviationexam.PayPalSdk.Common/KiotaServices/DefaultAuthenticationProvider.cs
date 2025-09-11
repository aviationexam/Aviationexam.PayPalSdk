using Aviationexam.PayPalSdk.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Aviationexam.PayPalSdk.Common.KiotaServices;

public sealed class DefaultAuthenticationProvider(
    [FromKeyedServices(DependencyInjectionExtensions.PayPalServiceKey)]
    IAccessTokenProvider accessTokenProvider
) : BaseBearerTokenAuthenticationProvider(accessTokenProvider);

using Aviationexam.PayPalSdk.Common.Configuration;
using Aviationexam.PayPalSdk.Common.Extensions;
using Aviationexam.PayPalSdk.Orders.Extensions;
using Aviationexam.PayPalSdk.Payments.Extensions;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace PayPal.Sdk.Checkout.Test.Infrastructure;

public static class ServiceProviderFactory
{
    public static ServiceProvider Create(
        PayPalAuthenticationsClassData.AuthenticationData authenticationData,
        bool shouldRedactHeaderValue = true
    ) => new ServiceCollection()
        .AddLogging(builder => builder
            .SetMinimumLevel(LogLevel.Trace)
            .AddProvider(new XUnitLoggerProvider(TestContext.Current.TestOutputHelper, appendScope: false))
        )
        .AddSingleton<TimeProvider>(_ => TimeProvider.System)
        .AddPayPalRestApiClient(
            builder => builder.Configure(x =>
            {
                x.Environment = EPayPalEnvironment.Sandbox;
                x.Timeout = TimeSpan.FromSeconds(20);
            }),
            shouldRedactHeaderValue: shouldRedactHeaderValue
        )
        .AddAuthorization(builder => builder.Configure(x =>
        {
            x.ClientId = authenticationData.ClientId;
            x.ClientSecret = authenticationData.ClientSecret;
            x.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(20);
        }), shouldRedactHeaderValue: shouldRedactHeaderValue)
        .AddPaymentsApi()
        .AddCheckoutOrdersApi()
        .Services
        .BuildServiceProvider();
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PayPal.Sdk.Checkout.Configuration;
using PayPal.Sdk.Checkout.Core;
using PayPal.Sdk.Checkout.Core.Interfaces;
using System;

namespace PayPal.Sdk.Checkout.Extensions;

public static class DependencyInjectionExtensions
{
    public const string PayPalRestApiHttpClient = "PayPalSdk.RestApiHttpClient";

    public static IServiceCollection AddPayPalCheckout(
        this IServiceCollection services,
        Action<OptionsBuilder<PayPalOptions>>? configureOptions = null
    )
    {
        if (configureOptions != null)
        {
            var options = services.AddOptions<PayPalOptions>();
            configureOptions(options);
        }

        services.AddScoped<IPayPayEncoder, PayPayEncoder>();

        return services;
    }
}

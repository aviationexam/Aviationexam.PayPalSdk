using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PayPal.Sdk.Checkout.Configuration;
using PayPal.Sdk.Checkout.Core;
using PayPal.Sdk.Checkout.Core.Interfaces;
using System;
using System.Net;
using System.Net.Http;

namespace PayPal.Sdk.Checkout.Extensions;

public static class DependencyInjectionExtensions
{
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

        services.AddHttpClient<IPayPalHttpClient, PayPalHttpClient>((serviceProvider, httpClient) =>
            {
                var payPalOptions = serviceProvider.GetRequiredService<IOptions<PayPalOptions>>().Value;

                httpClient.Timeout = payPalOptions.Timeout;
                httpClient.BaseAddress = new Uri(payPalOptions.BaseUrl);

                if (payPalOptions.SendPayPalUserAgentHeader)
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent.GetUserAgentHeader());
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            });

        return services;
    }
}

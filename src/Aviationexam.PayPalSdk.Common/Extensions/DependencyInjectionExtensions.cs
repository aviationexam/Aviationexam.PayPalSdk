using Aviationexam.PayPalSdk.Common.Authentication;
using Aviationexam.PayPalSdk.Common.Configuration;
using Aviationexam.PayPalSdk.Common.KiotaServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Net;
using System.Net.Http;

namespace Aviationexam.PayPalSdk.Common.Extensions;

public static class DependencyInjectionExtensions
{
    public const string PayPalServiceKey = "PayPalSdk";
    public const string PayPalRestApiHttpClient = "PayPalSdk.RestApiHttpClient";
    public const string PayPalHttpTokenClient = "PayPalSdk.HttpTokenClient";

    public static PayPalBuilder AddPayPalRestApiClient(
        this IServiceCollection serviceCollection,
        Action<OptionsBuilder<PayPalResApiOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        serviceCollection.AddKeyedScoped<LoggingHandler>(
            PayPalRestApiHttpClient,
            static (serviceProvider, key) => new LoggingHandler(
                serviceProvider.GetRequiredService<TimeProvider>(),
                serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(key!.ToString()!)
            )
        );

        var httpClientBuilder = serviceCollection.AddHttpClient(PayPalRestApiHttpClient)
            .AttachKiotaHandlers(serviceKey: PayPalServiceKey)
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<PayPalResApiOptions>>().Value;
                httpClient.Timeout = options.Timeout;
                httpClient.BaseAddress = options.Endpoint;
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            })
            .AddDefaultLogger();

        serviceCollection.Configure<HttpClientFactoryOptions>(
            httpClientBuilder.Name,
            options => options.HttpMessageHandlerBuilderActions.Add(b => b.AdditionalHandlers.Add(b.Services.GetRequiredKeyedService<LoggingHandler>(httpClientBuilder.Name)))
        );

#if NET9_0_OR_GREATER
        httpClientBuilder.AddAsKeyed();
#else
        serviceCollection.AddKeyedTransient<HttpClient>(httpClientBuilder.Name,
            static (serviceProvider, key) => serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(key!.ToString()!)
        );
#endif

        if (shouldRedactHeaderValue is false)
        {
            serviceCollection
                .Configure<HttpClientFactoryOptions>(httpClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
        }

        optionsBuilder(serviceCollection
            .AddOptions<PayPalResApiOptions>()
        );

        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IPostConfigureOptions<PayPalResApiOptions>, PayPalResApiOptionsPostConfigure>()
        );
        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IValidateOptions<PayPalResApiOptions>, PayPalResApiOptionsValidate>()
        );

        serviceCollection.TryAddKeyedTransient<IRequestAdapter, DefaultHttpClientRequestAdapter>(PayPalServiceKey);
        serviceCollection.TryAddKeyedSingleton<IAuthenticationProvider, DefaultAuthenticationProvider>(PayPalServiceKey);
        serviceCollection.TryAddKeyedSingleton<IAccessTokenProvider, DefaultAccessTokenProvider>(PayPalServiceKey);

        return new PayPalBuilder(serviceCollection);
    }

    public static HttpMessageHandler CreateHttpMessageHandler(
        this IServiceProvider serviceProvider
    )
    {
        var httpClientHandler = new HttpClientHandler();

        return httpClientHandler;
    }

    public static PayPalBuilder AddAuthorization(
        this PayPalBuilder builder,
        Action<OptionsBuilder<PayPalAuthenticationOptions>> optionsBuilder,
        bool shouldRedactHeaderValue = true
    )
    {
        var serviceCollection = builder.Services;

        var httpTokenClientBuilder = serviceCollection.AddHttpClient(PayPalHttpTokenClient)
            .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler)
            .AddDefaultLogger();

#if NET9_0_OR_GREATER
        httpTokenClientBuilder.AddAsKeyed();
#else
        serviceCollection.AddKeyedTransient<HttpClient>(httpTokenClientBuilder.Name,
            static (serviceProvider, key) => serviceProvider.GetRequiredService<IHttpClientFactory>()
                .CreateClient(key!.ToString()!)
        );
#endif

        if (shouldRedactHeaderValue is false)
        {
            serviceCollection.Configure<HttpClientFactoryOptions>(httpTokenClientBuilder.Name, x => x.ShouldRedactHeaderValue = _ => false);
        }

        optionsBuilder(serviceCollection
            .AddOptions<PayPalAuthenticationOptions>()
        );

        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IPostConfigureOptions<PayPalAuthenticationOptions>, PayPalAuthenticationOptionsPostConfigure>()
        );
        serviceCollection.TryAddEnumerable(ServiceDescriptor
            .Singleton<IValidateOptions<PayPalAuthenticationOptions>, PayPalAuthenticationOptionsValidate>()
        );

        serviceCollection.TryAddKeyedSingleton<IPayPalAccessTokenProvider, PayPalAccessTokenProvider>(PayPalServiceKey);

        return builder;
    }
}

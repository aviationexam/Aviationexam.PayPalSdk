using Aviationexam.PayPalSdk.Common.Extensions;
using Aviationexam.PayPalSdk.Payments.PayPalPaymentsClientV2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;

namespace Aviationexam.PayPalSdk.Payments.Extensions;

public static class DependencyInjectionExtensions
{
    public static PayPalBuilder AddPaymentsApi(
        this PayPalBuilder builder
    )
    {
        var serviceCollection = builder.Services;
        serviceCollection.AddTransient<PayPalPaymentsApiV2Client>(serviceProvider => new PayPalPaymentsApiV2Client(
            serviceProvider.GetRequiredKeyedService<IRequestAdapter>(Aviationexam.PayPalSdk.Common.Extensions.DependencyInjectionExtensions.PayPalServiceKey)
        ));

        return builder;
    }
}

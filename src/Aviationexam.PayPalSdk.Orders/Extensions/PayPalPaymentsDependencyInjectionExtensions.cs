using Aviationexam.PayPalSdk.Common.Extensions;
using Aviationexam.PayPalSdk.Payments.PayPalCheckoutOrdersClientV2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;

namespace Aviationexam.PayPalSdk.Orders.Extensions;

public static class PayPalPaymentsDependencyInjectionExtensions
{
    public static PayPalBuilder AddPaymentsApi(
        this PayPalBuilder builder
    )
    {
        builder.Services.AddTransient<PayPalOrdersApiV2Client>(serviceProvider => new PayPalOrdersApiV2Client(
            serviceProvider.GetRequiredKeyedService<IRequestAdapter>(DependencyInjectionExtensions.PayPalServiceKey)
        ));

        return builder;
    }
}

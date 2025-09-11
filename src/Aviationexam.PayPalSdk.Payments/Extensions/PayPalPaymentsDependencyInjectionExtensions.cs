using Aviationexam.PayPalSdk.Common.Extensions;
using Aviationexam.PayPalSdk.Payments.PayPalPaymentsClientV2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions;

namespace Aviationexam.PayPalSdk.Payments.Extensions;

public static class PayPalPaymentsDependencyInjectionExtensions
{
    public static PayPalBuilder AddPaymentsApi(
        this PayPalBuilder builder
    )
    {
        builder.Services.AddTransient<PayPalPaymentsApiV2Client>(serviceProvider => new PayPalPaymentsApiV2Client(
            serviceProvider.GetRequiredKeyedService<IRequestAdapter>(DependencyInjectionExtensions.PayPalServiceKey)
        ));

        return builder;
    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;
using PayPalAuthentication = PayPal.Sdk.Checkout.Authentication;
using PayPalOrders = PayPal.Sdk.Checkout.Orders;
using PayPalPayments = PayPal.Sdk.Checkout.Payments;

namespace PayPal.Sdk.Checkout.Core.MessageSerializers;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default
)]
[JsonSerializable(typeof(PayPalAuthentication.AccessToken))]
[JsonSerializable(typeof(PayPalAuthentication.RefreshToken))]
[JsonSerializable(typeof(PayPalOrders.AuthorizeRequest))]
[JsonSerializable(typeof(IReadOnlyCollection<PayPalOrders.StringPatch>))]
[JsonSerializable(typeof(PayPalOrders.Order))]
[JsonSerializable(typeof(PayPalOrders.OrderActionRequest))]
[JsonSerializable(typeof(PayPalOrders.OrderCaptureRequest))]
[JsonSerializable(typeof(PayPalOrders.OrderRequest))]
[JsonSerializable(typeof(PayPalPayments.CaptureRequest))]
[JsonSerializable(typeof(PayPalPayments.PaymentAuthorization))]
[JsonSerializable(typeof(PayPalPayments.PaymentCapture))]
[JsonSerializable(typeof(PayPalPayments.PaymentRefund))]
[JsonSerializable(typeof(PayPalPayments.ReauthorizeRequest))]
[JsonSerializable(typeof(PayPalPayments.RefundRequest))]
internal partial class PayPalJsonSerializerContext : JsonSerializerContext
{
    static PayPalJsonSerializerContext()
    {
#if NET8_0_OR_GREATER
        Default = new PayPalJsonSerializerContext(new System.Text.Json.JsonSerializerOptions(s_defaultOptions));
#endif
    }
}

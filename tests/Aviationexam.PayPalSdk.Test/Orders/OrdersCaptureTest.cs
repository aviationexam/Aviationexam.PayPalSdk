using Aviationexam.PayPalSdk.Payments.PayPalCheckoutOrdersClientV2;
using Aviationexam.PayPalSdk.Payments.PayPalCheckoutOrdersClientV2.Models;
using Aviationexam.PayPalSdk.Test.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aviationexam.PayPalSdk.Test.Orders;

[Collection("Orders")]
public class OrdersCaptureTest
{
    [Theory]
    [ClassData(typeof(PayPalAuthenticationsClassData), Explicit = true)]
    public async Task TestOrdersCaptureRequest(
        PayPalAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(
            authenticationData!,
            shouldRedactHeaderValue: true
        );

        var payPalOrdersApiV2Client = serviceProvider.GetRequiredService<PayPalOrdersApiV2Client>();

        var payPalRequestId = Guid.NewGuid();
        var createdOrder = await payPalOrdersApiV2Client.V2.Checkout.Orders.PostAsync(
            OrdersCreateTest.BuildRequestBody(),
            x => x.Headers.Add("PayPal-Request-Id", payPalRequestId.ToString()),
            cancellationToken: TestContext.Current.CancellationToken
        );

        Assert.NotNull(createdOrder);

        var getOrderResponse = await payPalOrdersApiV2Client.V2.Checkout.Orders[createdOrder.Id].GetAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getOrderResponse);

        if (getOrderResponse.Status is Order_status.APPROVED)
        {
            var captureOrderResponse = await payPalOrdersApiV2Client.V2.Checkout.Orders[createdOrder.Id].Capture.PostAsync(
                new Order_capture_request(),
                cancellationToken: TestContext.Current.CancellationToken
            );

            Assert.NotNull(captureOrderResponse);
            Assert.Equal(Order_status.COMPLETED, captureOrderResponse.Status);
            Assert.NotNull(captureOrderResponse.Payer);
        }
    }
}

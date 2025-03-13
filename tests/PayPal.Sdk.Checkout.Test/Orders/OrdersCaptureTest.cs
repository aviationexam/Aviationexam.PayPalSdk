using PayPal.Sdk.Checkout.ContractEnums;
using PayPal.Sdk.Checkout.Extensions;
using PayPal.Sdk.Checkout.Orders;
using PayPal.Sdk.Checkout.Test.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PayPal.Sdk.Checkout.Test.Orders;

[Collection("Orders")]
public class OrdersCaptureTest
{
    [Fact(Skip = "This test is an example. In production, you will need a credit card")]
    public async Task TestOrdersCaptureRequest()
    {
        using var payPalHttpClient = TestHttpClientFactory.CreateHttpClient();

        var accessToken = await payPalHttpClient.AuthenticateAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(accessToken);

        var orderResponse = await OrdersCreateTest.CreateOrder(payPalHttpClient, accessToken);

        Assert.NotNull(orderResponse.ResponseBody);

        var createdOrderId = orderResponse.ResponseBody.Id;

        var getOrderResponse = await payPalHttpClient.GetOrderRawAsync(accessToken, createdOrderId, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(getOrderResponse.ResponseBody);

        if (getOrderResponse.ResponseBody.CheckoutPaymentIntent == EOrderIntent.Authorize)
        {
            var request = new OrdersCaptureRequest(createdOrderId);
            request.SetPreferReturn(EPreferReturn.Representation);
            request.SetRequestBody(new OrderActionRequest
            {
                //PaymentSource = new PaymentSource
                //{
                //    Card = new Card { },
                //    Token = new Token
                //    {
                //        Id = "",
                //        Type = ETokenType.BillingAgreement,
                //    },
                //}
            });

            var response = await payPalHttpClient.ExecuteAsync<OrdersCaptureRequest, OrderActionRequest, Order>(request, accessToken, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, response.ResponseStatusCode);
            Assert.NotNull(response.ResponseBody);
        }
    }
}

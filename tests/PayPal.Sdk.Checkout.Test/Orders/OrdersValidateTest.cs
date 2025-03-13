using PayPal.Sdk.Checkout.Extensions;
using PayPal.Sdk.Checkout.Orders;
using PayPal.Sdk.Checkout.Test.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PayPal.Sdk.Checkout.Test.Orders;

[Collection("Orders")]
public class OrdersValidateTest
{
    [Fact(Skip = "This test is an example. In production, you will need a credit card")]
    public async Task TestOrdersValidateRequest()
    {
        using var payPalHttpClient = TestHttpClientFactory.CreateHttpClient();

        var accessToken = await payPalHttpClient.AuthenticateAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(accessToken);

        var orderResponse = await OrdersCreateTest.CreateOrder(payPalHttpClient, accessToken);
        Assert.NotNull(orderResponse.ResponseBody);
        var createdOrder = orderResponse.ResponseBody;

        var response = await payPalHttpClient.ValidateOrderRawAsync(
            accessToken,
            createdOrder.Id,
            request =>
            {
                request.SetRequestBody(new OrderActionRequest
                {
                    PaymentSource = new PaymentSource
                    {
                        Card = new Card(),
                        Token = new Token(),
                    },
                });
            },
            cancellationToken: TestContext.Current.CancellationToken
        );
        Assert.Equal(HttpStatusCode.OK, response.ResponseStatusCode);
        Assert.NotNull(response.ResponseBody);
    }
}

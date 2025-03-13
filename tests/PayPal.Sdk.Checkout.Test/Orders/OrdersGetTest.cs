using PayPal.Sdk.Checkout.Extensions;
using PayPal.Sdk.Checkout.Orders;
using PayPal.Sdk.Checkout.Test.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PayPal.Sdk.Checkout.Test.Orders;

[Collection("Orders")]
public class OrdersGetTest(
    ITestOutputHelper testOutputHelper
)
{
    [Fact]
    public async Task TestOrdersGetRequest()
    {
        using var payPalHttpClient = TestHttpClientFactory.CreateHttpClient();

        var accessToken = await payPalHttpClient.AuthenticateAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(accessToken);

        var orderResponse = await OrdersCreateTest.CreateOrder(payPalHttpClient, accessToken);

        Assert.NotNull(orderResponse.ResponseBody);
        var createdOrder = orderResponse.ResponseBody;

        var getOrderResponse = await payPalHttpClient.GetOrderRawAsync(accessToken, createdOrder.Id, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, getOrderResponse.ResponseStatusCode);

        Assert.NotNull(getOrderResponse.ResponseBody);

        var retrievedOrder = getOrderResponse.ResponseBody;
        Assert.NotNull(retrievedOrder);

        Assert.Equal(retrievedOrder.Id, createdOrder.Id);
        Assert.NotNull(retrievedOrder.PurchaseUnits);
        Assert.Equal(retrievedOrder.PurchaseUnits.Count, createdOrder.PurchaseUnits.Count);

        foreach (var purchaseUnit in retrievedOrder.PurchaseUnits)
        {
            var createdOrderPurchaseUnit = Assert.Single(createdOrder.PurchaseUnits, x => string.Equals(x.ReferenceId, purchaseUnit.ReferenceId, System.StringComparison.Ordinal));

            Assert.Equal(purchaseUnit.ReferenceId, createdOrderPurchaseUnit.ReferenceId);
            Assert.Equal(purchaseUnit.AmountWithBreakdown.CurrencyCode, createdOrderPurchaseUnit.AmountWithBreakdown.CurrencyCode);
            Assert.Equal(purchaseUnit.AmountWithBreakdown.Value, createdOrderPurchaseUnit.AmountWithBreakdown.Value);
        }

        Assert.NotNull(retrievedOrder.CreateTime);

        Assert.NotNull(createdOrder.Links);
        var foundApproveUrl = false;
        foreach (var linkDescription in createdOrder.Links)
        {
            if (string.Equals(linkDescription.Rel, "approve", System.StringComparison.Ordinal))
            {
                foundApproveUrl = true;
                Assert.NotNull(linkDescription.Href);
                Assert.Equal(EHttpMethod.Get, linkDescription.Method);
                testOutputHelper.WriteLine(linkDescription.Href);
            }
        }

        testOutputHelper.WriteLine(createdOrder.Id);
        Assert.True(foundApproveUrl);
        Assert.Equal(EOrderStatus.Created, createdOrder.Status);
    }
}

/*
using PayPal.Sdk.Checkout.Extensions;
using PayPal.Sdk.Checkout.Orders;
using PayPal.Sdk.Checkout.Test.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PayPal.Sdk.Checkout.Test.Orders;

/*
[Collection("Orders")]
public class OrdersPatchTest
{
    private IReadOnlyCollection<StringPatch> BuildRequestBody()
    {
        return
        [
            new()
            {
                Op = "add",
                Path = "/purchase_units/@reference_id=='test_ref_id1'/description",
                Value = "added_description",
            },
        ];
    }

    [Fact]
    public async Task TestOrdersPatchRequest()
    {
        using var payPalHttpClient = TestHttpClientFactory.CreateHttpClient();

        var accessToken = await payPalHttpClient.AuthenticateAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(accessToken);

        var response = await OrdersCreateTest.CreateOrder(payPalHttpClient, accessToken);

        Assert.NotNull(response.ResponseBody);
        var createdOrder = response.ResponseBody;

        var patchResponse = await payPalHttpClient.OrdersPatchRequestRawAsync(
            accessToken,
            createdOrder.Id,
            BuildRequestBody(),
            cancellationToken: TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.NoContent, patchResponse.ResponseStatusCode);

        var getOrderResponse = await payPalHttpClient.GetOrderRawAsync(
            accessToken, createdOrder.Id, cancellationToken: TestContext.Current.CancellationToken
        );

        Assert.Equal(HttpStatusCode.OK, getOrderResponse.ResponseStatusCode);

        Assert.NotNull(getOrderResponse.ResponseBody);
        var getOrder = getOrderResponse.ResponseBody;

        var firstPurchaseUnit = getOrder.PurchaseUnits.Single();
        Assert.Equal("test_ref_id1", firstPurchaseUnit.ReferenceId);
        Assert.Equal("USD", firstPurchaseUnit.AmountWithBreakdown.CurrencyCode);
        Assert.Equal("100.00", firstPurchaseUnit.AmountWithBreakdown.Value);
        Assert.Equal("added_description", firstPurchaseUnit.Description);
    }
}
*/

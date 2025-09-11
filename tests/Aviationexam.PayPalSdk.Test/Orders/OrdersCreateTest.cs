using Aviationexam.PayPalSdk.Payments.PayPalCheckoutOrdersClientV2;
using Aviationexam.PayPalSdk.Payments.PayPalCheckoutOrdersClientV2.Models;
using Microsoft.Extensions.DependencyInjection;
using PayPal.Sdk.Checkout.Test.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PayPal.Sdk.Checkout.Test.Orders;

[Collection("Orders")]
public class OrdersCreateTest
{
    private static Order_request BuildRequestBody() => new()
    {
        Intent = Checkout_payment_intent.CAPTURE,
        PurchaseUnits =
        [
            new Purchase_unit_request
            {
                ReferenceId = "test_ref_id1",
                InvoiceId = "123456",
                Amount = new Amount_with_breakdown
                {
                    CurrencyCode = "EUR",
                    Value = "230.00",
                    Breakdown = new Amount_breakdown
                    {
                        ItemTotal = new Money
                        {
                            CurrencyCode = "EUR",
                            Value = "220.00",
                        },
                        Shipping = new Money
                        {
                            CurrencyCode = "EUR",
                            Value = "10.00",
                        },
                    },
                },
                Items =
                [
                    new Item
                    {
                        Name = "T-shirt",
                        UnitAmount = new Money
                        {
                            CurrencyCode = "EUR",
                            Value = "20.00",
                        },
                        Quantity = "1",
                        Sku = "sku1",
                        Category = Item_category.PHYSICAL_GOODS,
                    },
                    new Item
                    {
                        Name = "Shoes",
                        UnitAmount = new Money
                        {
                            CurrencyCode = "EUR",
                            Value = "100.00",
                        },
                        Quantity = "2",
                        Sku = "sku2",
                        Category = Item_category.PHYSICAL_GOODS,
                    },
                ],
            },
        ],
        ApplicationContext = new Order_application_context
        {
            ReturnUrl = "https://www.example.com",
            CancelUrl = "https://www.example.com",
        },
    };

    [Theory]
    [ClassData(typeof(PayPalAuthenticationsClassData))]
    public async Task TestOrdersCreateRequest(
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
            BuildRequestBody(),
            x => x.Headers.Add("PayPal-Request-Id", payPalRequestId.ToString()),
            cancellationToken: TestContext.Current.CancellationToken
        );

        Assert.NotNull(createdOrder);
        Assert.NotNull(createdOrder.Id);
        Assert.Equal(Order_status.CREATED, createdOrder.Status);

        Assert.NotNull(createdOrder.Links);

        var approveUrl =Assert.Single(createdOrder.Links, x => string.Equals(x.Rel, "approve", StringComparison.Ordinal));
        Assert.NotNull(approveUrl.Href);
        Assert.Equal(Link_description_method.GET, approveUrl.Method);

        TestContext.Current.TestOutputHelper?.WriteLine("OrderId: {0}", createdOrder.Id);
        TestContext.Current.TestOutputHelper?.WriteLine("ApproveUrl: {0}", approveUrl.Href);
    }
}

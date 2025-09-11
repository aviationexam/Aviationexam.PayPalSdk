# REST API SDK for Dotnet V2

[![Build Status](https://github.com/aviationexam/Aviationexam.PayPalSdk/actions/workflows/build.yml/badge.svg?branch=develop)](https://github.com/aviationexam/Aviationexam.PayPalSdk/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/Aviationexam.PayPalSdk.Common.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Aviationexam.PayPalSdk.Common/)
[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Faviationexam%2Fpaypal-sdk-checkout%2Fshield%2FAviationexam.PayPalSdk.Common%2Flatest&label=Aviationexam.PayPalSdk.Common)](https://f.feedz.io/aviationexam/paypal-sdk-checkout/packages/Aviationexam.PayPalSdk.Common/latest/download)

__Welcome to PayPal Dotnet SDK__. This repository content is build on top of the [paypal-rest-api-specifications](https://github.com/paypal/paypal-rest-api-specifications/tree/main?tab=readme-ov-file) using [microsoft/kiota](https://github.com/microsoft/kiota).

Please refer to the [PayPal REST APIs](https://developer.paypal.com/api/rest/) for more information.

## Prerequisites

.NET 8.0, or higher

## Usage

### Setting up credentials
Get client ID and client secret by going to https://developer.paypal.com/dashboard/.

## Install the package

This repository contains the following packages:
- Aviationexam.PayPalSdk.Common
- Aviationexam.PayPalSdk.Payments
- Aviationexam.PayPalSdk.Orders

Common package is required to be installed for other packages to work. Payments contains Payments related APIs. Orders contains Checkout Orders related APIs.

## Configure library

```c#
void Configure(IServiceCollection serviceCollection)
{
    serviceCollection
        .AddPayPalRestApiClient(
            builder => builder.Configure(x =>
            {
                x.Environment = EPayPalEnvironment.Sandbox;
                x.Timeout = TimeSpan.FromSeconds(20);
            }),
            shouldRedactHeaderValue: shouldRedactHeaderValue
        )
        .AddAuthorization(builder => builder.Configure(x =>
        {
            x.ClientId = "ClientId";
            x.ClientSecret = "ClientSecret";
            x.JwtEarlyExpirationOffset = TimeSpan.FromMinutes(5);
        }), shouldRedactHeaderValue: shouldRedactHeaderValue)
        .AddPaymentsApi() // adds payments api
        .AddCheckoutOrdersApi() // adds checkot orders api
}
```

### Creating an Order
This will create an order and print order id for the created order

```c#
public async static Task<Aviationexam.PayPalSdk.Payments.PayPalCheckoutOrdersClientV2.Models.Order?> CreateOrder(PayPalOrdersApiV2Client payPalOrdersApiV2Client)
{
    var order = new Order_request
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

    var payPalRequestId = Guid.NewGuid();
    var createdOrder = await payPalOrdersApiV2Client.V2.Checkout.Orders.PostAsync(
        BuildRequestBody(),
        x => x.Headers.Add("PayPal-Request-Id", payPalRequestId.ToString()),
        cancellationToken: TestContext.Current.CancellationToken
    );

    Console.WriteLine("Status: {0}", createdOrder.Status);
    Console.WriteLine("Order Id: {0}", createdOrder.Id);
    Console.WriteLine("Links:");
    foreach (var link in createdOrder.Links)
    {
         Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
    }
    return createdOrder;
}
```

### Capturing an Order
Before capturing an order, order should be approved by the buyer using the approve link in create order response
```c#
public async static Task<Aviationexam.PayPalSdk.Payments.PayPalCheckoutOrdersClientV2.Models.Order?> CaptureOrder(this PayPalOrdersApiV2Client payPalOrdersApiV2Client, string orderId)
{
    var getOrderResponse = await payPalOrdersApiV2Client.V2.Checkout.Orders[createdOrder.Id].GetAsync(cancellationToken: TestContext.Current.CancellationToken);

    if (getOrderResponse.Status is Order_status.APPROVED)
    {
        var captureOrderResponse = await payPalOrdersApiV2Client.V2.Checkout.Orders[createdOrder.Id].Capture.PostAsync(
            new Order_capture_request(),
            cancellationToken: TestContext.Current.CancellationToken
        );

        return captureOrderResponse;
    }

    return null;
}
```
## Running tests

To run integration tests using your client id and secret, run the `test`
```shell
PAYPAL_CLIENT_ID=YOUR_SANDBOX_CLIENT_ID PAYPAL_CLIENT_SECRET=YOUR_SANDBOX_CLIENT_SECRET dotnet test
```

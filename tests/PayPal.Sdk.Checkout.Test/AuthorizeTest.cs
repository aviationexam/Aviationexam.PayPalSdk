using Aviationexam.PayPalSdk.Common.Authentication;
using Aviationexam.PayPalSdk.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PayPal.Sdk.Checkout.Test.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace PayPal.Sdk.Checkout.Test;

public class AuthorizeTest
{
    [Theory]
    [ClassData(typeof(PayPalAuthenticationsClassData))]
    public async Task GetAuthorizationTokenWorks(
        PayPalAuthenticationsClassData.AuthenticationData? authenticationData
    )
    {
        await using var serviceProvider = ServiceProviderFactory.Create(
            authenticationData!,
            shouldRedactHeaderValue: false
        );

        var payPalAccessTokenProvider = serviceProvider.GetRequiredKeyedService<IPayPalAccessTokenProvider>(
            serviceKey: DependencyInjectionExtensions.PayPalServiceKey
        );

        var accessToken = await payPalAccessTokenProvider.GetAuthorizationTokenAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(accessToken);

        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        var httpClient = httpClientFactory.CreateClient(nameof(GetAuthorizationTokenWorks));

        using var httpMessage = new HttpRequestMessage(HttpMethod.Get, "https://api-m.sandbox.paypal.com/v1/identity/openidconnect/userinfo?schema=openid");
        httpMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(httpMessage, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStreamAsync(TestContext.Current.CancellationToken);

        var jsonDocument = await JsonDocument.ParseAsync(
            responseContent,
            new JsonDocumentOptions
            {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow,
                MaxDepth = 0,
            },
            TestContext.Current.CancellationToken
        );

        var jsonElement = jsonDocument.RootElement;

        Assert.True(jsonElement.TryGetProperty("user_id", out var userId));
        Assert.True(jsonElement.TryGetProperty("sub", out var sub));
        Assert.NotEmpty(userId.GetString()!);
        Assert.NotEmpty(sub.GetString()!);
    }
}

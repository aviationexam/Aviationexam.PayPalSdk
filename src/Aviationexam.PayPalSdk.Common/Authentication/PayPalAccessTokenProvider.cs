using Aviationexam.PayPalSdk.Common.Configuration;
using Aviationexam.PayPalSdk.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.PayPalSdk.Common.Authentication;

public sealed class PayPalAccessTokenProvider(
    [FromKeyedServices(DependencyInjectionExtensions.PayPalHttpTokenClient)]
    HttpClient httpClient,
    TimeProvider timeProvider,
    IOptions<PayPalAuthenticationOptions> authenticationOptions
) : IPayPalAccessTokenProvider
{
    private TokenResponse? _cachedToken;
    private DateTimeOffset? _tokenExpiresAt;

    private readonly
#if NET9_0_OR_GREATER
        Lock
#else
        object
#endif
        _lock = new();

    public ValueTask<string> GetAuthorizationTokenAsync(
        CancellationToken cancellationToken
    )
    {
        var now = timeProvider.GetUtcNow();

        lock (_lock)
        {
            if (_cachedToken != null && now < _tokenExpiresAt)
            {
                return ValueTask.FromResult(_cachedToken.AccessToken);
            }

            _cachedToken = null;
            _tokenExpiresAt = null;
        }

        return GetAuthorizationTokenInternalAsync(cancellationToken);
    }

    private async ValueTask<string> GetAuthorizationTokenInternalAsync(
        CancellationToken cancellationToken
    )
    {
        var options = authenticationOptions.Value;
        var now = timeProvider.GetUtcNow();

        using var request = new HttpRequestMessage(HttpMethod.Post, options.TokenEndpoint);
        request.Content = new FormUrlEncodedContent([
            KeyValuePair.Create("grant_type", "client_credentials"),
        ]);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.ClientId}:{options.ClientSecret}")));

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var tokenResponse = await JsonSerializer.DeserializeAsync(jsonStream, PayPalAuthenticationJsonContext.Default.TokenResponse, cancellationToken);

        if (tokenResponse is null)
        {
            throw new InvalidOperationException("Failed to deserialize token response.");
        }

        var expiresAt = now + tokenResponse.ExpiresInTimeSpan - options.JwtEarlyExpirationOffset;
        lock (_lock)
        {
            _cachedToken = tokenResponse;
            _tokenExpiresAt = expiresAt;
        }

        return tokenResponse.AccessToken;
    }
}

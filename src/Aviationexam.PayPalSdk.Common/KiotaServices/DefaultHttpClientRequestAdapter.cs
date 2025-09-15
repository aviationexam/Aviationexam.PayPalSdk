using Aviationexam.PayPalSdk.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System.Net.Http;

namespace Aviationexam.PayPalSdk.Common.KiotaServices;

public class DefaultHttpClientRequestAdapter(
    [FromKeyedServices(DependencyInjectionExtensions.PayPalServiceKey)]
    IAuthenticationProvider authenticationProvider,
    [FromKeyedServices(DependencyInjectionExtensions.PayPalServiceKey)]
    IParseNodeFactory parseNodeFactory,
    [FromKeyedServices(DependencyInjectionExtensions.PayPalServiceKey)]
    ISerializationWriterFactory serializationWriterFactory,
    [FromKeyedServices(DependencyInjectionExtensions.PayPalRestApiHttpClient)]
    HttpClient httpClient,
    [FromKeyedServices(DependencyInjectionExtensions.PayPalServiceKey)]
    ObservabilityOptions observabilityOptions
) : HttpClientRequestAdapter(
    authenticationProvider, parseNodeFactory, serializationWriterFactory, httpClient, observabilityOptions
);

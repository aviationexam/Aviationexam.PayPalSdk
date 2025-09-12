using Microsoft.Kiota.Abstractions;
using System;

namespace Aviationexam.PayPalSdk.Common.Extensions;

public static class RequestConfigurationExtensions
{
    public static RequestConfiguration<TQueryParameters> SetPreferReturn<TQueryParameters>(
        this RequestConfiguration<TQueryParameters> requestConfiguration, EPreferReturn preferReturn
    ) where TQueryParameters : class, new()
    {
        var preferValue = preferReturn switch
        {
            EPreferReturn.Minimal => "return=minimal",
            EPreferReturn.Representation => "return=representation",
            _ => throw new ArgumentOutOfRangeException(nameof(preferReturn), preferReturn, null),
        };

        requestConfiguration.Headers.Add("Prefer", preferValue);

        return requestConfiguration;
    }

    public static RequestConfiguration<TQueryParameters> SetPayPalRequestId<TQueryParameters>(
        this RequestConfiguration<TQueryParameters> requestConfiguration, string payPalRequestId
    ) where TQueryParameters : class, new()
    {
        requestConfiguration.Headers.Add("PayPal-Request-Id", payPalRequestId);

        return requestConfiguration;
    }

    public static RequestConfiguration<TQueryParameters> SetPayPalClientMetadataId<TQueryParameters>(
        this RequestConfiguration<TQueryParameters> requestConfiguration, string payPalClientMetadataId
    ) where TQueryParameters : class, new()
    {
        requestConfiguration.Headers.Add("PayPal-Client-Metadata-Id", payPalClientMetadataId);

        return requestConfiguration;
    }

    public static RequestConfiguration<TQueryParameters> SetPayPalPartnerAttributionId<TQueryParameters>(
        this RequestConfiguration<TQueryParameters> requestConfiguration, string payPalPartnerAttributionId
    ) where TQueryParameters : class, new()
    {
        requestConfiguration.Headers.Add("PayPal-Partner-Attribution-Id", payPalPartnerAttributionId);

        return requestConfiguration;
    }
}

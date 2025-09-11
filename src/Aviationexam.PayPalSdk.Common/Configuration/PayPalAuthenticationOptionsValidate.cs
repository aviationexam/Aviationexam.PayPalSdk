using Microsoft.Extensions.Options;
using System;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public sealed class PayPalAuthenticationOptionsValidate : IValidateOptions<PayPalAuthenticationOptions>
{
    public ValidateOptionsResult Validate(
        string? name, PayPalAuthenticationOptions options
    )
    {
        if (options.JwtEarlyExpirationOffset <= TimeSpan.Zero)
        {
            return ValidateOptionsResult.Fail(
                $"The '{nameof(options.JwtEarlyExpirationOffset)}' option must be a positive value, '{options.JwtEarlyExpirationOffset}' given."
            );
        }

        if (options.JwtEarlyExpirationOffset > TimeSpan.FromMinutes(20))
        {
            return ValidateOptionsResult.Fail(
                $"The '{nameof(options.JwtEarlyExpirationOffset)}' option must not be bigger than 20 minutes, '{options.JwtEarlyExpirationOffset}' given."
            );
        }

        return ValidateOptionsResult.Success;
    }
}

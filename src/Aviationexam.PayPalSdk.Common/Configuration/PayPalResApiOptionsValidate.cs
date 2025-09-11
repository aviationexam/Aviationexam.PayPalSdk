using Microsoft.Extensions.Options;

namespace Aviationexam.PayPalSdk.Common.Configuration;

public sealed class PayPalResApiOptionsValidate : IValidateOptions<PayPalResApiOptions>
{
    public ValidateOptionsResult Validate(
        string? name, PayPalResApiOptions options
    ) => ValidateOptionsResult.Success;
}

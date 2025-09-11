using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum ETokenType : byte
{
    /// <summary>
    /// The PayPal billing agreement ID. References an approved recurring payment for goods or services.
    /// </summary>
    [EnumMember(Value = "BILLING_AGREEMENT")]
    BillingAgreement,
}

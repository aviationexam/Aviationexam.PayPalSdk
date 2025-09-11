using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum EPayeePreferred : byte
{
    /// <summary>
    /// Accepts any type of payment from the customer.
    /// </summary>
    [EnumMember(Value = "UNRESTRICTED")]
    Unrestricted,

    /// <summary>
    /// Accepts only immediate payment from the customer. For example, credit card, PayPal balance, or instant ACH. Ensures that at the time of capture, the payment does not have the `pending` status.
    /// </summary>
    [EnumMember(Value = "IMMEDIATE_PAYMENT_REQUIRED")]
    ImmediatePaymentRequired,
}

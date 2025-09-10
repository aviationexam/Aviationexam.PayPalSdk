using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum ELandingPage : byte
{
    /// <summary>
    /// When the customer clicks PayPal Checkout, the customer is redirected to a page to log in to PayPal and approve the payment.
    /// </summary>
    [EnumMember(Value = "LOGIN")]
    Login,

    /// <summary>
    /// When the customer clicks PayPal Checkout, the customer is redirected to a page to enter credit or debit card and other relevant billing information required to complete the purchase.
    /// </summary>
    [EnumMember(Value = "BILLING")]
    Billing,

    /// <summary>
    /// When the customer clicks PayPal Checkout, the customer is redirected to either a page to log in to PayPal and approve the payment or to a page to enter credit or debit card and other relevant billing information required to complete the purchase, depending on their previous interaction with PayPal.
    /// </summary>
    [EnumMember(Value = "NO_PREFERENCE")]
    NoPreference,
}

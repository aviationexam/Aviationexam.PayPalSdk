using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum EUserAction : byte
{
    /// <summary>
    /// After you redirect the customer to the PayPal payment page, a Continue button appears.
    /// Use this option when the final amount is not known when the checkout flow is initiated
    /// and you want to redirect the customer to the merchant page without processing the payment.
    /// </summary>
    [EnumMember(Value = "CONTINUE")]
    Continue,

    /// <summary>
    /// After you redirect the customer to the PayPal payment page, a Pay Now button appears.
    /// Use this option when the final amount is known when the checkout is initiated
    /// and you want to process the payment immediately when the customer clicks Pay Now.
    /// </summary>
    [EnumMember(Value = "PAY_NOW")]
    PayNow,
}

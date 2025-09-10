using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum EShippingPreference : byte
{
    /// <summary>
    /// When the customer clicks PayPal Checkout, the customer is redirected to a page to log in to PayPal and approve the payment.
    /// </summary>
    [EnumMember(Value = "LOGIN")]
    Login,

    /// <summary>
    /// Use the customer-provided shipping address on the PayPal site.
    /// </summary>
    [EnumMember(Value = "GET_FROM_FILE")]
    GetFromFile,

    /// <summary>
    /// Redact the shipping address from the PayPal site. Recommended for digital goods.
    /// </summary>
    [EnumMember(Value = "NO_SHIPPING")]
    NoShipping,

    /// <summary>
    /// Use the merchant-provided address. The customer cannot change this address on the PayPal site.
    /// </summary>
    [EnumMember(Value = "SET_PROVIDED_ADDRESS")]
    SetProvidedAddress,
}

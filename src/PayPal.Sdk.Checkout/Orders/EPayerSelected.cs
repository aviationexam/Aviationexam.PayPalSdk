using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum EPayerSelected : byte
{
    [EnumMember(Value = "PAYPAL")]
    PayPal,
}

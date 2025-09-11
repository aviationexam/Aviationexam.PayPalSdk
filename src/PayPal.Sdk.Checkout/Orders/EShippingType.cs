using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum EShippingType : byte
{
    /// <summary>
    /// The payer intends to receive the items at a specified address.
    /// </summary>
    [EnumMember(Value = "SHIPPING")]
    Shipping,

    /// <summary>
    /// The payer intends to pick up the items from the payee in person.
    /// </summary>
    [EnumMember(Value = "PICKUP_IN_PERSON")]
    PickupInPerson,
}

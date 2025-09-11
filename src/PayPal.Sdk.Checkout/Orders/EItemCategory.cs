using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum EItemCategory : byte
{
    /// <summary>
    /// Goods that are stored, delivered, and used in their electronic format.
    /// This value is not currently supported for API callers that leverage the [PayPal for Commerce Platform](https://www.paypal.com/us/webapps/mpp/commerce-platform) product.
    /// </summary>
    [EnumMember(Value = "DIGITAL_GOODS")]
    DigitalGoods,

    /// <summary>
    /// A tangible item that can be shipped with proof of delivery.
    /// </summary>
    [EnumMember(Value = "PHYSICAL_GOODS")]
    PhysicalGoods,
}

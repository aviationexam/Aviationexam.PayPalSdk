using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum EDisbursementMode : byte
{
    /// <summary>
    /// The funds are released to the merchant immediately.
    /// </summary>
    [EnumMember(Value = "INSTANT")]
    Instant,

    /// <summary>
    /// The funds are held for a finite number of days. The actual duration depends on the region and type of integration. You can release the funds through a referenced payout. Otherwise, the funds disbursed automatically after the specified duration.
    /// </summary>
    [EnumMember(Value = "DELAYED")]
    Delayed,
}

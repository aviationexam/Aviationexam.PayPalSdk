using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

public enum ETaxIdType : byte
{
    /// <summary>
    /// The individual tax ID type, typically is 11 characters long.
    /// </summary>
    [EnumMember(Value = "BR_CPF")]
    BrCpf,

    /// <summary>
    /// The business tax ID type, typically is 14 characters long.
    /// </summary>
    [EnumMember(Value = "BR_CNPJ")]
    BrCnpj,
}

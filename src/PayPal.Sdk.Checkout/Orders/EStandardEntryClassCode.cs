using System.Runtime.Serialization;

namespace PayPal.Sdk.Checkout.Orders;

/// <summary>
/// NACHA (the regulatory body governing the ACH network) requires that API callers (merchants, partners) obtain the consumer’s explicit authorization before initiating a transaction. To stay compliant, you’ll need to make sure that you retain a compliant authorization for each transaction that you originate to the ACH Network using this API. ACH transactions are categorized (using SEC codes) by how you capture authorization from the Receiver (the person whose bank account is being debited or credited). PayPal supports the following SEC codes.
/// </summary>
public enum EStandardEntryClassCode : byte
{
    /// <summary>
    /// The API caller (merchant/partner) accepts authorization and payment information from a consumer over the telephone.
    /// </summary>
    [EnumMember(Value = "TEL")]
    Tel,

    /// <summary>
    /// The API caller (merchant/partner) accepts Debit transactions from a consumer on their website.
    /// </summary>
    [EnumMember(Value = "WEB")]
    Web,

    /// <summary>
    /// Cash concentration and disbursement for corporate debit transaction. Used to disburse or consolidate funds. Entries are usually Optional high-dollar, low-volume, and time-critical. (e.g. intra-company transfers or invoice payments to suppliers).
    /// </summary>
    [EnumMember(Value = "CCD")]
    Ccd,

    /// <summary>
    /// Prearranged payment and deposit entries. Used for debit payments authorized by a consumer account holder, and usually initiated by a company. These are usually recurring debits (such as insurance premiums).
    /// </summary>
    [EnumMember(Value = "PPD")]
    Ppd,
}

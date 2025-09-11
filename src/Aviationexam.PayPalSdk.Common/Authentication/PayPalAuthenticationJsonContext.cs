using System.Text.Json.Serialization;

namespace Aviationexam.PayPalSdk.Common.Authentication;

[JsonSerializable(typeof(AccessToken))]
[JsonSerializable(typeof(RefreshToken))]
public partial class PayPalAuthenticationJsonContext : JsonSerializerContext;

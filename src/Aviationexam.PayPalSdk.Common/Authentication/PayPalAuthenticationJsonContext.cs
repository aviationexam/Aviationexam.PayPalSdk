using System.Text.Json.Serialization;

namespace Aviationexam.PayPalSdk.Common.Authentication;

[JsonSerializable(typeof(TokenResponse))]
public partial class PayPalAuthenticationJsonContext : JsonSerializerContext;

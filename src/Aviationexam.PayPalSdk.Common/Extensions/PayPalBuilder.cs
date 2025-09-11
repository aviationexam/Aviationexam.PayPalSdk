using Microsoft.Extensions.DependencyInjection;

namespace Aviationexam.PayPalSdk.Common.Extensions;

public class PayPalBuilder(
    IServiceCollection serviceCollection
)
{
    public IServiceCollection Services { get; } = serviceCollection;
}

using System.Threading;
using System.Threading.Tasks;

namespace Aviationexam.PayPalSdk.Common.Authentication;

public interface IPayPalAccessTokenProvider
{
    ValueTask<string> GetAuthorizationTokenAsync(CancellationToken cancellationToken);
}

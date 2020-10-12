using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.CallProxying.Services
{
    public interface IProxyActions : IDisposable
    {
        Task OnBeforeCallAsync();

        Task OnSuccessAsync(object responsePayload);

        Task<ImmutableList<object>> OnFailureAsync(Exception exception = null);

        Task OnTerminatingAsync();
    }
}
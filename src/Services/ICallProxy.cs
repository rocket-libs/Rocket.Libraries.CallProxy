using System;
using System.Threading.Tasks;
using Rocket.Libraries.CallProxying.Models;

namespace Rocket.Libraries.CallProxying.Services
{
    public interface ICallProxy : IDisposable
    {
        Task<WrappedResponse<TSuccess>> CallAsync<TSuccess>(Func<Task<TSuccess>> runner);
    }
}
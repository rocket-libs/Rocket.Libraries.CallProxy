using System;
using System.Threading.Tasks;
using Rocket.Libraries.CallProxying.Models;

namespace Rocket.Libraries.CallProxying.Services
{
    public interface ICallProxy : IDisposable
    {
        Task<WrappedResponse<TResponse>> CallAsync<TResponse>(Func<Task<TResponse>> runner);
    }
}
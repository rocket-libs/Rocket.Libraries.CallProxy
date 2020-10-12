namespace Rocket.Libraries.CallProxying.Services
{
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Rocket.Libraries.CallProxy.Models;
    using Rocket.Libraries.CallProxying.Models;

    public class CallProxy : ICallProxy
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private IProxyActions proxyActions;

        public CallProxy(IProxyActions proxyActions, IHttpContextAccessor httpContextAccessor)
        {
            this.proxyActions = proxyActions;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void RepondThatRequestWasBad<TResponse>(TResponse payload)
        {
            throw new BadRequestException<TResponse>(payload);
        }

        public async Task<WrappedResponse<TResponse>> CallAsync<TResponse>(Func<Task<TResponse>> runner)
        {
            try
            {
                if (disposedValue)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                await proxyActions.OnBeforeCallAsync();
                var responsePayload = await runner();
                await proxyActions.OnSuccessAsync(responsePayload);
                return GeTResponseResponse<TResponse>(responsePayload);
            }
            catch (Exception e)
            {
                var errors = await proxyActions.OnFailureAsync(e);
                return GetErrorResponse<TResponse>(e, errors);
            }
            finally
            {
                await CleanUpAsync();
                runner = null;
            }
        }

        private async Task CleanUpAsync()
        {
            await proxyActions?.OnTerminatingAsync();
            proxyActions?.Dispose();
            this.proxyActions = null;
        }

        private WrappedResponse<TResponse> GeTResponseResponse<TResponse>(TResponse response)
        {
            return new WrappedResponse<TResponse>
            {
                Code = 1,
                Message = "Success",
                Payload = response,
            };
        }

        private WrappedResponse<TResponse> GetErrorResponse<TResponse>(Exception e, ImmutableList<object> errors)
        {
            var badRequestException = e as BadRequestException<TResponse>;
            if (badRequestException != null)
            {
                httpContextAccessor.HttpContext.Response.StatusCode = 400;
            }

            return new WrappedResponse<TResponse>
            {
                Code = 2,
                Message = "Error Occured On Server.",
                Errors = errors,
                Payload = badRequestException != null ? badRequestException.Payload : default,
            };
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Responder()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
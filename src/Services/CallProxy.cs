namespace Rocket.Libraries.CallProxying.Services
{
    using Rocket.Libraries.CallProxying.Models;
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    public class CallProxy : ICallProxy
    {
        private IProxyActions proxyActions;

        public CallProxy(IProxyActions proxyActions)
        {
            this.proxyActions = proxyActions;
        }

        public async Task<WrappedResponse<TSuccess>> CallAsync<TSuccess>(Func<Task<TSuccess>> runner)
        {
            try
            {
                if (disposedValue)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                await proxyActions.OnBeforeCallAsync();
                var response = await runner();
                await proxyActions.OnSuccessAsync();
                return GetSuccessResponse<TSuccess>(response);
            }
            catch (Exception e)
            {
                var errors = await proxyActions.OnFailureAsync(e);
                return GetErrorResponse<TSuccess>(e, errors);
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

        private WrappedResponse<TSuccess> GetSuccessResponse<TSuccess>(TSuccess response)
        {
            return new WrappedResponse<TSuccess>
            {
                Code = 1,
                Message = "Success",
                Payload = response,
            };
        }

        private WrappedResponse<TSuccess> GetErrorResponse<TSuccess>(Exception e, ImmutableList<object> errors)
        {
            return new WrappedResponse<TSuccess>
            {
                Code = 2,
                Message = "Error Occured On Server.",
                Errors = errors,
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
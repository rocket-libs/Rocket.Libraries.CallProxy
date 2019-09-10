namespace Rocket.Libraries.CallProxying.Services
{
    using System;
    using System.Threading.Tasks;
    using Rocket.Libraries.CallProxying.Models;
    using Rocket.Libraries.Validation.Exceptions;
    using Rocket.Libraries.Validation.Models;
    using src.Services;

    public class CallProxy : ICallProxy
    {
        private IProxyActions proxyActions;

        public CallProxy(IProxyActions proxyActions)
        {
            this.proxyActions = proxyActions;
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
                var response = await runner();
                await proxyActions.OnSuccessAsync();
                return GetSuccessResponse(response);
            }
            catch (Exception e)
            {
                await proxyActions.OnFailureAsync(e);
                return GetErrorResponse<TResponse>(e);
            }
            finally
            {
                CleanUp();
                runner = null;
            }
        }

        private void CleanUp()
        {
            this.proxyActions?.OnTerminatingAsync();
            this.proxyActions = null;
        }

        private static ImmutableList<Error> GetErrorsIfAny(Exception e)
        {
            var failedValidationException = e as FailedValidationException;
            if (failedValidationException != null)
            {
                return failedValidationException.Errors;
            }
            else
            {
                return null;
            }
        }

        private WrappedResponse<TResponse> GetSuccessResponse<TResponse>(TResponse response)
        {
            return new WrappedResponse<TResponse>
            {
                Code = 1,
                Message = "Success",
                Payload = response,
            };
        }

        private WrappedResponse<TResponse> GetErrorResponse<TResponse>(Exception e)
        {
            return new WrappedResponse<TResponse>
            {
                Code = 2,
                Message = "Error Occured On Server.",
                Errors = GetErrorsIfAny(e),
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
                    CleanUp();
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
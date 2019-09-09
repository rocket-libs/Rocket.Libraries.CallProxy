using System;
using System.Threading.Tasks;

namespace src.Services
{
    public interface IProxyActions
    {
         Task OnBeforeCallAsync();

         Task OnSuccessAsync();

         Task OnFailureAsync(Exception exception = null);

         Task OnTerminatingAsync();

         
    }
}
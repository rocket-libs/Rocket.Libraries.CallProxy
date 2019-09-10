using System.Collections.Immutable;

namespace Rocket.Libraries.CallProxying.Models
{
    public class WrappedResponse<TSuccess>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public TSuccess Payload { get; set; }

        public ImmutableList<object> Errors { get; set; }
    }
}
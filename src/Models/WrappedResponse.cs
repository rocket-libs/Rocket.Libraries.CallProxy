using System.Collections.Immutable;

namespace Rocket.Libraries.CallProxying.Models
{
    public class WrappedResponse<TResponse>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public TResponse Payload { get; set; }

        public ImmutableList<object> Errors { get; set; }
    }
}
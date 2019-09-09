using System.Collections.Generic;
using System.Collections.Immutable;
using Rocket.Libraries.Validation.Models;

namespace Rocket.Libraries.CallProxying.Models
{
    public class WrappedResponse<TResponse>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public TResponse Payload { get; set; }

        public ImmutableList<Error> Errors { get; set; }
    }
}
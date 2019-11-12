using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.CallProxy.Models
{
    public class BadRequestException<TSuccess> : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BadRequestException()
        {
        }

        public BadRequestException(TSuccess payload)
        {
            Payload = payload;
        }

        public TSuccess Payload { get; }
    }
}
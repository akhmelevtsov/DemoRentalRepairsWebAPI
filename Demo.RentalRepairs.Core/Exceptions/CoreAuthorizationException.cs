using System;

namespace Demo.RentalRepairs.Core.Exceptions
{
    public class CoreAuthorizationException : Exception
    {
        public string Code { get; }

        public CoreAuthorizationException(string code, string message) : base ($"{code}:{message}")
        {
            Code = code;
        }
    }
}

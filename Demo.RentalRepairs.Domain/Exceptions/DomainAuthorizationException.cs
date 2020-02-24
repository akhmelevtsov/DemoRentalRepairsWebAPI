using System;

namespace Demo.RentalRepairs.Domain.Exceptions
{
    public class DomainAuthorizationException : Exception
    {
        public string Code { get; }

        public DomainAuthorizationException(string code, string message) : base ($"{code}:{message}")
        {
            Code = code;
        }
    }
}

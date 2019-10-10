using System;

namespace Demo.RentalRepairs.Core.Exceptions
{
    public class CoreNotFoundException : Exception
    {
        public string Code { get; }

        public CoreNotFoundException(string code, string message) : base ($"{code}:{message}")
        {
            Code = code;
        }
    }
}

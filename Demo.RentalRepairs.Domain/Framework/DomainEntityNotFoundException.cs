using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Domain.Framework
{
    public class DomainEntityNotFoundException : DomainException  
    {
        public DomainEntityNotFoundException(string code, string message) : base(code, message)
        {
        }
    }
}

using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.Exceptions
{
    public class DomainEntityNotFoundException : DomainException  
    {
        public DomainEntityNotFoundException(string code, string message) : base(code, message)
        {
        }
    }
}

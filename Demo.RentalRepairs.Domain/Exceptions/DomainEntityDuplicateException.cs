namespace Demo.RentalRepairs.Domain.Exceptions
{
    public class DomainEntityDuplicateException : DomainException  
    {
        public DomainEntityDuplicateException(string code, string message) : base(code, message)
        {
        }
    }
}

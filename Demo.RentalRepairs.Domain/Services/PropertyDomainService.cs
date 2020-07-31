using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.Services
{
    public class PropertyDomainService
    {
        public static  IDateTimeProvider DateTimeProvider { get;  set; } = new DateTimeProvider();
    
    }
}

using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.Services
{
    public class PropertyDomainService
    {
        public static  IDateTimeProvider DateTimeProvider { get;  set; } = new DateTimeProvider();



        //public PropertyDomainService(IDateTimeProvider dateTimeProvider = null)
        //{

        //    DateTimeProvider = dateTimeProvider ?? new DateTimeProvider();
        //}

     

    
    }
}

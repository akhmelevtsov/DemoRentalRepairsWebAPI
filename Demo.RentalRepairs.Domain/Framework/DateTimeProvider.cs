using System;

namespace Demo.RentalRepairs.Domain.Framework
{
    public class DateTimeProvider :IDateTimeProvider 
    {
        public DateTime GetDateTime()
        {
            return DateTime.Now;
        }
    }
}

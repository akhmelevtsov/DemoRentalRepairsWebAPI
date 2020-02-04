using System;
using System.Collections.Generic;
using System.Text;

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

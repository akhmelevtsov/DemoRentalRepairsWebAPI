using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Core.Tests.Integration
{
    public class DateTimeProviderMock :IDateTimeProvider
    {
        public static DateTime CurrentDate { get; set; } = new DateTime(2020, 1, 10);
        public DateTime GetDateTime()
        {
            return CurrentDate;
        }
    }
}

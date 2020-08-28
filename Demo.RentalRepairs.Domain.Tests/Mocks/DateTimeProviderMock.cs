using System;
using Demo.RentalRepairs.Domain.Framework;

namespace Demo.RentalRepairs.Domain.Tests.Mocks
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

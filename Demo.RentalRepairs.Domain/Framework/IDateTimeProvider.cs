using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Domain.Framework
{
    public interface IDateTimeProvider
    {
        DateTime GetDateTime();
    }
}

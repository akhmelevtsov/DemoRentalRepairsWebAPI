using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Infrastructure.Email
{
    public enum EmailServiceTypeEnum
    {
        NoEmail,
        DebugEmailSlurper,
        AzureSendGrid
    }
}

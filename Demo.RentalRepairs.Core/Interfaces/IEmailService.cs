using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface  IEmailService
    {
        void SendEmail(EmailInfo email);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Interfaces;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class EmailServiceMock : IEmailService
    {
        private readonly List<EmailInfo> _emails = new List<EmailInfo>();
        
        public List<EmailInfo> Emails => _emails;
        public EmailInfo LastSentEmail { get; private set; }
       

        public void SendEmail(EmailInfo email)
        {
            _emails.Add(email);
            LastSentEmail = email;
        }
    }
}

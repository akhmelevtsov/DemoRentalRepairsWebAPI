using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class EmailServiceMock : IEmailService
    {
        private readonly List<EmailInfo> _emails = new List<EmailInfo>();
        
        public List<EmailInfo> Emails => _emails;
        public EmailInfo LastSentEmail { get; private set; }
       

        public async Task  SendEmailAsync(EmailInfo email)
        {
            await Task.CompletedTask;
            if (email!= null)
             _emails.Add(email);
            LastSentEmail = email;
        }
    }
}

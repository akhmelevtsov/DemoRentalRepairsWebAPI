using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class MockEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // In a real implementation, you would send the email here
            // For demo purposes, we just complete the task
            return Task.CompletedTask;
        }
    }
}
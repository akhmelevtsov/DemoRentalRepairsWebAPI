using System.Threading.Tasks;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface  IEmailService
    {
        Task SendEmailAsync(EmailInfo email);
    }
}

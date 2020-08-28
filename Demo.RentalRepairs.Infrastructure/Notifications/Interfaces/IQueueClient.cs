using System.Threading.Tasks;

namespace Demo.RentalRepairs.Infrastructure.Notifications.Interfaces
{
    public interface IQueueClient
    {
        Task SendMessage(string message);
    }
}
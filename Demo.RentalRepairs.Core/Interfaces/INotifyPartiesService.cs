using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Core.Interfaces
{
    public interface INotifyPartiesService
    {
        void NotifyRequestStatusChange(RequestStatusMessage message);
    }
}

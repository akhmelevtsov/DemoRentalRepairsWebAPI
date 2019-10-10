using System.Collections.Generic;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class NotifyPartiesServiceMock : INotifyPartiesService
    {
        public RequestStatusMessage LastMessage { get; set; }
        public List<RequestStatusMessage > AllMessages { get; set;  } = new List<RequestStatusMessage>();
        public void NotifyRequestStatusChange(RequestStatusMessage message)
        {
            LastMessage = message;
            AllMessages.Add(message);
        }
    }
}

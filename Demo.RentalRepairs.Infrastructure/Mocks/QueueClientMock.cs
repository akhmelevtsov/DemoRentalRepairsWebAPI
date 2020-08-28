using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Demo.RentalRepairs.Infrastructure.Notifications;
using Demo.RentalRepairs.Infrastructure.Notifications.Interfaces;

namespace Demo.RentalRepairs.Infrastructure.Mocks
{
    public class QueueClientMock : IQueueClient
    {
        private readonly List<string> _messages = new List<string>();

        public List<string> Messages
        {
            get => _messages;
         
        }

        public async Task SendMessage(string message)
        {
            await Task.CompletedTask;
            _messages.Add(message);
        }
    }
}

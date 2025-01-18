using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public interface IQueueService
    {
        event Func<string, Task> MessageReceived;
        Task SendMessageAsync<T>(T serviceBusMessage, string queueName);
        Task RecevieMessagesAsync(string queueName);
    }
}

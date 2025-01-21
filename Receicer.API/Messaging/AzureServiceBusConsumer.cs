
using Azure.Messaging.ServiceBus;
using Common.Model;
using Common.Service;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading.Tasks;

namespace Receicer.API.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _emailServiceBusProcessor;
        private readonly string serviceBusConnectionString;

        private readonly string subscriptionQueueName;

        private readonly IEmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionQueueName = _configuration.GetValue<string>("TopicAndQueueNames:Queue");
            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailServiceBusProcessor = client.CreateProcessor(subscriptionQueueName);
            _emailService = emailService;
        }
        public async Task Start()
        {
            _emailServiceBusProcessor.ProcessMessageAsync += OnEmailRequestReceived;
            _emailServiceBusProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailServiceBusProcessor.StartProcessingAsync();

        }

        public async Task Stop()
        {
            await _emailServiceBusProcessor.StartProcessingAsync();
            await _emailServiceBusProcessor.DisposeAsync();
        }

        private async Task OnEmailRequestReceived(ProcessMessageEventArgs args)
        {
            string messageBody = args.Message.Body.ToString();
            Product receivedMessage = JsonSerializer.Deserialize<Product>(messageBody);
            try
            {
                await _emailService.SendEmail(receivedMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

    }
}

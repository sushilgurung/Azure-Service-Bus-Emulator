using Azure.Messaging.ServiceBus;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Service
{
    public class ServiceBusReceiverService
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        public event Func<string, Task> MessageReceived;
        public ServiceBusReceiverService(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;

            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor(queueName);

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
        }

        /// <summary>
        /// Receive message on Pull Model
        /// </summary>
        /// <returns></returns>
        public async Task<Product> ReceiveMessageAsync()
        {
            await using var client = new ServiceBusClient(_connectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(_queueName);
            Product body = null;
            try
            {
                ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));

                if (message is not null)
                {
                    string jsonBody = message.Body.ToString();
                    body = JsonSerializer.Deserialize<Product>(jsonBody);
                }
                await receiver.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return body;
        }



        public async Task StartProcessingAsync()
        {
            await _processor.StartProcessingAsync();
        }

        public async Task StopProcessingAsync()
        {
            await _processor.StopProcessingAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var messageBody = args.Message.Body.ToString();
            if (MessageReceived != null)
            {
                await MessageReceived.Invoke(messageBody);
            }
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
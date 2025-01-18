using Azure.Messaging.ServiceBus;
using Common.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Service
{
    public class QueueService : IQueueService
    {
        const string ConnectionString = "Endpoint=sb://localhost:5672;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
        public event Func<string, Task> MessageReceived;
        private readonly ConcurrentQueue<Product> _receivedMessages = new();

        List<Product> products = new List<Product>();
        public async Task SendMessageAsync<T>(T serviceBusMessage, string queueName)
        {
            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            try
            {
                string messageBody = JsonSerializer.Serialize(serviceBusMessage);
                ServiceBusMessage message = new ServiceBusMessage(messageBody);
                await sender.SendMessageAsync(message);
                Console.WriteLine($"Sending message: {messageBody}");
                Console.WriteLine("Message sent successfully!");
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

        public async Task RecevieMessagesAsync(string queueName)
        {

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusProcessor processor = client.CreateProcessor(queueName);
            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;
                await processor.StartProcessingAsync();
                Console.WriteLine("Press any key to stop processing...");
                Console.ReadKey();

                Console.WriteLine("Stopping message processing...");
                await processor.StopProcessingAsync();
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private Task MessageHandler(ProcessMessageEventArgs args)
        {
            string messageBody = args.Message.Body.ToString();
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(messageBody);
            }
            Product receivedMessage = JsonSerializer.Deserialize<Product>(messageBody);
            HandleTable(receivedMessage);
            return args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error occurred: {args.Exception.Message}");
            return Task.CompletedTask;
        }


        private void HandleTable(Product product)
        {
            products.Add(product);
            PrintTableHeader();
            foreach (var item in products)
            {
                PrintTableRow(item.Name, item.Description, item.Price, item.Category);
            }
        }
        static void PrintTableHeader()
        {
            Console.WriteLine("+-------------------+-----------------------------------------------+--------+----------------+");
            Console.WriteLine("| Name              | Description                                   | Price  | Category       |");
            Console.WriteLine("+-------------------+-----------------------------------------------+--------+----------------+");
        }

        static void PrintTableRow(string name, string description, decimal price, string category)
        {
            Console.WriteLine($"| {name,-15} | {description,-26} | {price,6:C2} | {category,-14} |");
        }
    }


}

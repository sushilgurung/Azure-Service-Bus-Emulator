using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public class ServiceBusProcessorService
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public ServiceBusProcessorService(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        //public async Task StartProcessingMessagesAsync()
        //{
        //    await using var client = new ServiceBusClient(_connectionString);
        //    ServiceBusProcessor processor = client.CreateProcessor(_queueName);

        //    try
        //    {
        //        processor.ProcessMessageAsync += MessageHandler;
        //        processor.ProcessErrorAsync += ErrorHandler;
        //        await processor.StartProcessingAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Received message: {ex.Message}");
        //    }
        //    finally
        //    {
        //        await processor.DisposeAsync();
        //        await client.DisposeAsync();
        //    }
        //}


        public async Task StartProcessingMessagesAsync()
        {
            var options = new ServiceBusClientOptions
            {
                RetryOptions = new ServiceBusRetryOptions
                {
                    TryTimeout = TimeSpan.FromSeconds(60), // Increase timeout to 60 seconds
                    MaxRetries = 5,                       // Set retry attempts
                    Delay = TimeSpan.FromSeconds(2)       // Delay between retries
                }
            };
            await using var client = new ServiceBusClient(_connectionString, options);
            ServiceBusProcessor processor = client.CreateProcessor(_queueName);

            try
            {
                // Attach handlers
                processor.ProcessMessageAsync += async args =>
                {
                    try
                    {
                        // Process the message
                        await MessageHandler(args);
                    }
                    catch (Exception ex)
                    {
                        // Log error during message processing
                        Console.WriteLine($"Error in MessageHandler: {ex.Message}");
                    }
                };

                processor.ProcessErrorAsync += args =>
                {
                    // Log errors reported by the Service Bus
                    Console.WriteLine($"Error in ErrorHandler: {args.Exception.Message}");
                    return Task.CompletedTask; // Since this is a non-blocking callback
                };

                // Start processing
                await processor.StartProcessingAsync();
            }
            catch (Exception ex)
            {
                // Catch errors during processor setup or start
                Console.WriteLine($"Error while starting processor: {ex.Message}");
            }
            finally
            {
                // Ensure resources are released
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        // Sample MessageHandler for processing messages
        //private async Task MessageHandler(ProcessMessageEventArgs args)
        //{
        //    // Simulate message processing logic
        //    Console.WriteLine($"Received message: {args.Message.Body.ToString()}");
        //    await args.CompleteMessageAsync(args.Message);
        //}



        private Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received message: {body}");

            // Complete the message after processing
            return args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error occurred: {args.Exception}");
            return Task.CompletedTask;
        }
    }
}

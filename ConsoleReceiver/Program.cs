using Common.Service;

try
{
    string queueName = "queue.1";
    QueueService queueService = new();
    await queueService.RecevieMessagesAsync(queueName);
}
catch (Exception ex)
{
    Console.WriteLine($"Error sending message: {ex.Message}");
}
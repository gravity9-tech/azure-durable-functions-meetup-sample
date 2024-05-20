using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Notification.App.Entities;
using Notification.App.Models;

namespace Notification.App.Acitvities;

public class SendNotificationActivity(ILogger<SendNotificationActivity> logger)
{
    [Function(nameof(SendNotificationActivity))]
    public void Run([ActivityTrigger] SendNotificationActivityInput input, [DurableClient] DurableTaskClient client)
    {
        logger.LogInformation($"Notification sent to {input.PhoneNumber} with message: {input.Message}");
    }
}
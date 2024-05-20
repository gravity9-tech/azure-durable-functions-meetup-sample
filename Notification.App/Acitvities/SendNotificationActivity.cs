using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notification.App.Models;

namespace Notification.App.Acitvities;

public class SendNotificationActivity(ILogger<SendNotificationActivity> logger)
{
    [Function(nameof(SendNotificationActivity))]
    public void Run([ActivityTrigger] SendNotificationActivityInput input)
    {
        logger.LogInformation($"Notification sent to {input.PhoneNumber} with message: {input.Message}");
    }
}
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Notification.App.Models;

namespace Notification.App.Acitvities;

public class SendNotificationActivity
{
    [FunctionName(nameof(SendNotificationActivity))]
    public void Run([ActivityTrigger] SendNotificationActivityInput input, ILogger logger)
    {
        logger.LogInformation($"Notification sent to {input.PhoneNumber} with message: {input.Message}");
    }
}
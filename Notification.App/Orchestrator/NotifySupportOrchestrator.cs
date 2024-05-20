using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Notification.App.Acitvities;
using Notification.App.Models;

namespace Notification.App.Orchestrator;

public class NotifySupportOrchestrator(ILogger<NotifySupportOrchestrator> logger)
{
    [Function(nameof(NotifySupportOrchestrator))]
    public async Task Run(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var input = context.GetInput<NotifySupportOrchestratorInput>();

        if (input.Contacts == null)
        {
            // Let's get the support contacts from storage.
            var supportContacts = await context.CallActivityAsync<IEnumerable<Contact>>(
                nameof(GetContactActivity),
                "Support");

            input.Contacts = supportContacts.ToArray();
        }

        var notificationOrchestratorInput = new SendNotificationOrchestratorInput 
        {
            MaxNotificationAttempts = input.MaxNotificationAttempts,
            Message = input.Message,
            NotificationAttemptCount = 1,
            SupportContact = input.Contacts[input.ContactIndex],
            WaitTimeForEscalationInSeconds = input.WaitTimeForEscalationInSeconds
        };
            
        var notificationResult = await context.CallSubOrchestratorAsync<SendNotificationOrchestratorResult>(
            nameof(SendNotificationOrchestrator),
            notificationOrchestratorInput);
            
        if (!notificationResult.CallbackReceived &&
            notificationOrchestratorInput.SupportContact != input.Contacts.Last())
        {
            // Calls have not been answered, let's try the next contact.
            input.ContactIndex++;
            logger.LogInformation($"=== Next Contact={input.Contacts[input.ContactIndex].PhoneNumber} ===");
            context.ContinueAsNew(input);
        }
        else
        {
            logger.LogInformation($"=== Completed {nameof(NotifySupportOrchestrator)} for {notificationResult.PhoneNumber} with callback received={notificationResult.CallbackReceived} on attempt={notificationResult.Attempt}. ===");
        }
    }
}
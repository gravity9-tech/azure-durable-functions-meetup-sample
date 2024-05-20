using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Notification.App.Models;
using Notification.App.Orchestrator;

namespace Notification.App.Clients;

public class NotifySupportHttpClient
{
    [FunctionName(nameof(NotifySupportHttpClient))]
    public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Post), Route = null)] HttpRequestMessage message, [DurableClient] IDurableClient client,
        ILogger logger)
    {
        var clientInput = await message.Content.ReadAsAsync<NotifySupportClientInput>();
        var waitTimeForEscalationInSeconds = 60;
        var maxNotificationAttempts = 3;

        var orchestratorInput = new NotifySupportOrchestratorInput
        {
            MaxNotificationAttempts = maxNotificationAttempts,
            Message = clientInput.Message,
            ContactIndex = 0,
            // The SupportContacts is *not* set here, they are added later.
            Severity = clientInput.Severity,
            WaitTimeForEscalationInSeconds = waitTimeForEscalationInSeconds
        };

        string instanceId = await client.StartNewAsync(
            nameof(NotifySupportOrchestrator),
            orchestratorInput);

        return client.CreateCheckStatusResponse(message, instanceId);
    }
}
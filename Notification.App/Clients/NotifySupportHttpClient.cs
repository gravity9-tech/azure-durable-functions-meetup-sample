using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Notification.App.Models;
using Notification.App.Orchestrator;

namespace Notification.App.Clients;

public class NotifySupportHttpClient
{
    [Function(nameof(NotifySupportHttpClient))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "POST", Route = null)] HttpRequestData message, [DurableClient] DurableTaskClient client)
    {
        var clientInput = await message.ReadFromJsonAsync<NotifySupportClientInput>();
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
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(NotifySupportOrchestrator),
            orchestratorInput);

        return await client.CreateCheckStatusResponseAsync(message, instanceId);
    }
}
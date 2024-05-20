using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Entities;
using Microsoft.Extensions.Logging;
using Notification.App.Entities;

namespace Notification.App.Clients;

public class CallbackHttpClient(ILogger<CallbackHttpClient> logger)
{
    [Function(nameof(CallbackHttpClient))]
    public async Task<IActionResult> Run([HttpTrigger(
            AuthorizationLevel.Function,
            "POST",
            Route = null)] HttpRequestData message, [DurableClient] DurableTaskClient client)
    {
        var phoneNumber = await message.ReadFromJsonAsync<string>();
        var entityId = new EntityInstanceId(nameof(NotificationOrchestratorInstanceEntity), phoneNumber);
        var a = client.Entities.GetAllEntitiesAsync().AsPages();
        await foreach (var page in a)
        {
            var pa = page.Values;
        }
        var instanceEntity = await client.Entities.GetEntityAsync<string>(entityId);
        if (instanceEntity?.State != null)
        {
            await client.RaiseEventAsync(
                instanceEntity.State,
                "Callback",
                true);
        }
        else
        {
            logger.LogError($"=== No instanceId found for {phoneNumber}. ===");
        }
            
        return new AcceptedResult();
    }
}
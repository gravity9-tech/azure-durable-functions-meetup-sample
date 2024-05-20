using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Newtonsoft.Json;

namespace Notification.App.Entities;

[JsonObject(MemberSerialization.OptIn)]
public class NotificationOrchestratorInstanceEntity : TaskEntity<string>
{
    [JsonProperty("instanceId")]
    public string InstanceId { get; set; }

    public Task Set(string instanceId)
    {
        InstanceId = instanceId;
        return Task.CompletedTask;
    }

    public void Reset() => InstanceId = string.Empty;
    
    public Task<string> Get() => Task.FromResult(InstanceId);

    [Function(nameof(NotificationOrchestratorInstanceEntity))]
    public static Task Run([EntityTrigger]TaskEntityDispatcher ctx) => ctx.DispatchAsync<NotificationOrchestratorInstanceEntity>();
}
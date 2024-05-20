using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Entities;
using Newtonsoft.Json;

namespace Notification.App.Entities;

public class NotificationOrchestratorInstanceEntity
{
    public string InstanceId { get; set; }

    public void Set(string instanceId)
    {
        InstanceId = instanceId;
    }

    public void Reset() => InstanceId = string.Empty;

    public string Get() => InstanceId;

    [Function(nameof(NotificationOrchestratorInstanceEntity))]
    public static Task RunEntityAsync([EntityTrigger]TaskEntityDispatcher ctx) => ctx.DispatchAsync<NotificationOrchestratorInstanceEntity>();
}
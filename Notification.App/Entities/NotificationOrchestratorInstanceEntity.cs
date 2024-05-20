using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace Notification.App.Entities;

public class NotificationEntity
{
    public string InstanceId { get; set; }

    public void Set(string instanceId)
    {
        InstanceId = instanceId;
    }

    public void Reset() => InstanceId = string.Empty;

    public string Get() => InstanceId;

    [Function(nameof(NotificationEntity))]
    public static Task RunEntityStaticAsync([EntityTrigger]TaskEntityDispatcher ctx) => ctx.DispatchAsync<NotificationEntity>();
}
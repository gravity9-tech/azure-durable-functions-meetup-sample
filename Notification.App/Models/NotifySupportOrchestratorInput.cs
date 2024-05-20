namespace Notification.App.Models;

public class NotifySupportOrchestratorInput
{
    public int MaxNotificationAttempts { get; set; }
    public string Message { get; set; }
    public int Severity { get; set; }
    public int ContactIndex { get; set; }
    public Contact[] Contacts { get; set; }
    public int WaitTimeForEscalationInSeconds { get; set; }
}
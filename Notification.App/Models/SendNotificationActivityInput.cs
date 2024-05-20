namespace Notification.App.Models;

public class SendNotificationActivityInput
{
    public int Attempt { get; set; }
    public string Message { get; set; }
    public string PhoneNumber { get; set; }
}
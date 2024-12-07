namespace cinemate.Models.Notification
{
    public class UpdateNotificationStatusModel
    {
        public Guid NotificationId { get; set; }
        public bool IsRead { get; set; }
    }
}

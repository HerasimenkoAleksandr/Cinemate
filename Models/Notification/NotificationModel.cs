namespace cinemate.Models.Notification
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public String SentAt { get; set; }
        public Guid MovieId { get; set; }  // ID фильма, связанного с уведомлением
    }
}

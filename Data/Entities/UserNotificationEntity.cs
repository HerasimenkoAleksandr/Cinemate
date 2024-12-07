namespace cinemate.Data.Entities
{
    public class UserNotificationEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid NotificationId { get; set; }
        public NotificationEntity Notification { get; set; }

        public bool IsRead { get; set; } = false;  // Маячок для отслеживания прочитанности
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}

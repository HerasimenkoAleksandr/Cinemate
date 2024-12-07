namespace cinemate.Models.Notification
{
    public class NotificationResponseModel
    {
        public int TotalCount { get; set; }         // Общее количество уведомлений
        public int UnreadCount { get; set; }        // Количество непрочитанных уведомлений
        public string Url { get; set; }             // URL для получения уведомлений
        public List<NotificationModel> Notifications { get; set; }  // Список уведомлений
    }
}

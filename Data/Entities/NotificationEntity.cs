namespace cinemate.Data.Entities
{
    public class NotificationEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public String Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Уведомление будет связано с добавлением фильма
        public Guid MovieId { get; set; }
        public MoviesEntities Movie { get; set; }
    }
}

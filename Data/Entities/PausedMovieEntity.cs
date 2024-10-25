namespace cinemate.Data.Entities
{
    public class PausedMovieEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }  // Идентификатор пользователя, который поставил фильм на паузу
        public Guid MovieId { get; set; }  // Идентификатор фильма
        public TimeSpan PauseTime { get; set; }  // Время паузы в фильме
        public DateTime PausedAt { get; set; } = DateTime.Now;  // Дата и время, когда фильм был поставлен на паузу

        // Навигационные свойства
        public User User { get; set; }
        public MoviesEntities Movie { get; set; }
    }
}

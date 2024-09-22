namespace cinemate.Data.Entities
{
    public class FavoriteMovieEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }  // Ссылка на пользователя
        public Guid MovieId { get; set; }  // Ссылка на фильм
        public User User { get; set; }    // Навигационное свойство
        public MoviesEntities Movie { get; set; }  // Навигационное свойство
    }
}

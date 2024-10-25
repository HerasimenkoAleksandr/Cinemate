namespace cinemate.Models.Movie
{
    public class SavePauseRequest
    {
        public Guid MovieId { get; set; }  // Идентификатор фильма
        public TimeSpan PauseTime { get; set; }  // Время паузы в фильме
    }
}

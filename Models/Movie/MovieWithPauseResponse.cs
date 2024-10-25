namespace cinemate.Models.Movie
{
    public class MovieWithPauseResponse
    {
        public Guid UserId { get; set; }  
        public MoviesModel? MoviePause { get; set; }
        public TimeSpan PauseTime { get; set; }
        public DateTime PausedAt { get; set; }
    }
}

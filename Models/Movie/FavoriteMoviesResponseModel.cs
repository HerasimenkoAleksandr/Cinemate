namespace cinemate.Models.Movie
{
    public class FavoriteMoviesResponseModel
    {
        public Guid UserId { get; set; }  // ID пользователя
        public int FavoriteMoviesCount { get; set; }  // Количество избранных фильмов
        public List<MoviesModel> FavoriteMovies { get; set; } = new List<MoviesModel>();  // Список избранных фильмов
    }
}

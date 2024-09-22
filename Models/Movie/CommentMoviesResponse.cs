namespace cinemate.Models.Movie
{
    public class CommentMoviesResponse
    {
        public Guid IdMovie { get; set; }  // ID фильма
        public List<CommentDetails> Comments { get; set; }  // Список комментариев
        public class CommentDetails
        {
            public Guid Id { get; set; }  // ID комментария
            public Guid IdUsers { get; set; }  // ID пользователя
            public String Comment { get; set; }  // Текст комментария
            public String CommentDt { get; set; }
        }
    }
}

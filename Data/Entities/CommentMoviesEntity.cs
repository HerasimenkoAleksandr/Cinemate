namespace cinemate.Data.Entities
{
    public class CommentMoviesEntity
    {
        public Guid Id { get; set; }
        public Guid IdUsers { get; set; }  
        public Guid IdMovie { get; set; }  
        public String Comment { get; set; }
        public DateTime CommentDt { get; set; } = DateTime.Now;
        public DateTime? DeleteCommentDt { get; set; }
        public User User { get; set; }
        public MoviesEntities Movie { get; set; }
        
    }
}

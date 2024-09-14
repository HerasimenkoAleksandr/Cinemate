namespace cinemate.Data.Entities
{
    public class LikeForMovie
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } 
        public Guid MovieId { get; set; } 
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public User User { get; set; } = null!;
        public MoviesEntities Movie { get; set; } = null!;
    }
}

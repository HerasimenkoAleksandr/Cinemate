namespace cinemate.Models.Movie
{
    public class MoviesModel
    {
        public Guid Id { get; set; }            
        public String? Title { get; set; }       
        public String? Description { get; set; } 
        public String? Picture { get; set; }
        public String? URL { get; set; } = null!;
        public int ReleaseYear { get; set; }    
        public String? Director { get; set; }    
        public String[]? Actors { get; set; }
        public String? Duration { get; set; }
        public int? likeCount { get; set; }
        public int? dislikeCount { get; set; }
        public Guid CategoryId { get; set; }    
        public Guid SubCategoryId { get; set; } 
      
    }
}

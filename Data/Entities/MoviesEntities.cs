namespace cinemate.Data.Entities
{
    public class MoviesEntities
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public String Title { get; set; } = null!;
        public String? Description { get; set; }
        public String Picture { get; set; } = null!;
        public String URL { get; set; } = null!;
        public int ReleaseYear { get; set; }
        public String? Director { get; set; }
        public String[]? Actors { get; set; }
        public int? likeCount { get; set; }
        public int? dislikeCount { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }

        public Gategories Category { get; set; }
        public SubCategoriesEntity SubCategory { get; set; }
    }

}

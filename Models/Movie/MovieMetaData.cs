namespace cinemate.Models.Movie
{
    public class MovieMetaData
    {
        public string Service { get; set; }
        public string Endpoint { get; set; }
        public string CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public string? StatusMovies { get; set; } = "unblocked";
        public int TotalSubCategory { get; set; }
        public int TotalMovies { get; set; }
    }
}

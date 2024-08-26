using cinemate.Data.Entities;
using cinemate.Models.Category;

namespace cinemate.Models.Movie
{
    public class MoviesResponse
    {

        public MovieMetaData Meta { get; set; } = new MovieMetaData();
        public  List<MoviesModel> Data { get; set; } =new List <MoviesModel>();
    }
}

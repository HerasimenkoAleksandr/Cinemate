using Azure;
using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.Category;
using cinemate.Models.Movie;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public MoviesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult GetMovies([FromQuery] string? categoryId, [FromQuery] string? subcategoryId)
        {

            

            if (subcategoryId != null)
            {
                if ( !Guid.TryParse(subcategoryId, out Guid subcategoryGuid))
                {
                    return BadRequest("Invalid  subcategoryID");
                }

                // Запрос к базе данных для получения фильмов по ID категории и подкатегории
                var MoviesFromSubCat = _dataContext.MoviesEntities
                                .Where(m => m.SubCategoryId == subcategoryGuid)
                                .ToList();
               
                if (MoviesFromSubCat == null || !MoviesFromSubCat.Any())
                {
                    return NotFound("No movies found for the given category and subcategory.");
                }

                // Получаем категорию и подкатегорию на основе первого фильма в списке
                var category = _dataContext.Gategories
                    .SingleOrDefault(c => c.Id == MoviesFromSubCat.First().CategoryId);

                var subcategory = _dataContext.SubCategoriesEntity
                    .SingleOrDefault(sc => sc.Id == MoviesFromSubCat.First().SubCategoryId);

                var movieModel = MoviesFromSubCat.Select(mfsc => new MoviesModel
                {
                    Id = mfsc.Id,
                    Title = mfsc.Title,
                    Description = mfsc.Description,
                    Picture = mfsc.Picture,
                    URL = mfsc.URL,
                    ReleaseYear = mfsc.ReleaseYear,
                    Director = mfsc.Director,
                    Actors = mfsc.Actors,
                    likeCount = mfsc.likeCount,
                    dislikeCount = mfsc.dislikeCount,
                    CategoryId = mfsc.CategoryId,
                    SubCategoryId = mfsc.SubCategoryId
                }).ToList();


                var response = new MoviesResponse
                {
                    Meta = new MovieMetaData
                    {
                        Service = "Movies",

                        Endpoint = $"/api/movies?{subcategoryId}",

                        CategoryName = category.Name,

                        SubCategoryName = subcategory.Name,

                        TotalMovies = subcategory.ContentCount,
                    },
                    Data= movieModel
                };
             


                // Возвращаем найденные фильмы
                return Ok(response);


            }
            else
            {
                if (categoryId != null)
                {

                    if (!Guid.TryParse(categoryId, out Guid categoryGuid) )
                    {
                        return BadRequest("Invalid categoryID");
                    }
                    var MoviesFromCat = _dataContext.MoviesEntities
                                   .Where(m => m.CategoryId == categoryGuid)
                                   .ToList();
                    // Если фильмы не найдены, возвращаем 404
                    if (MoviesFromCat == null || !MoviesFromCat.Any())
                    {
                        return NotFound("No movies found for the given category and subcategory.");
                    }

                    // Получаем категорию и подкатегорию на основе первого фильма в списке
                    var category = _dataContext.Gategories
                        .SingleOrDefault(c => c.Id == MoviesFromCat.First().CategoryId);

                    var movieModel = MoviesFromCat.Select(mfsc => new MoviesModel
                    {
                        Id = mfsc.Id,
                        Title = mfsc.Title,
                        Description = mfsc.Description,
                        Picture = mfsc.Picture,
                        URL = mfsc.URL,
                        ReleaseYear = mfsc.ReleaseYear,
                        Director = mfsc.Director,
                        Actors = mfsc.Actors,
                        likeCount = mfsc.likeCount,
                        dislikeCount = mfsc.dislikeCount,
                        CategoryId = mfsc.CategoryId,
                        SubCategoryId = mfsc.SubCategoryId
                    }).ToList();


                        var totalMoviesCount = _dataContext.MoviesEntities
                    .Where(m => m.CategoryId == category.Id)
                    .Count();

                    var response = new MoviesResponse
                    {
                        Meta = new MovieMetaData
                        {
                            Service = "Movies",

                            Endpoint = $"/api/movies?{categoryId}",

                            CategoryName = category.Name,

                            TotalSubCategory = category.ContentCount,

                            TotalMovies = totalMoviesCount,
                        },
                        Data = movieModel
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest("Invalid CategoryId and subcategoryId ");
                }
            }
        }


        [HttpGet("{movieId}")]
        public IActionResult GetMovieById(Guid movieId)
        {
           
            var movie = _dataContext.MoviesEntities
                            .SingleOrDefault(m => m.Id == movieId);

            var totalMoviesCount = _dataContext.MoviesEntities.Count();
            // If no movie found, return 404
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }
            var category = _dataContext.Gategories
                    .SingleOrDefault(c => c.Id == movie.CategoryId);


            var subcategory = _dataContext.SubCategoriesEntity
                    .SingleOrDefault(c => c.Id == movie.SubCategoryId);
            
            var movieModel = new MoviesModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Picture = movie.Picture,
                URL = movie.URL,
                ReleaseYear = movie.ReleaseYear,
                Director = movie.Director,
                Actors = movie.Actors,
                likeCount = movie.likeCount,
                dislikeCount = movie.dislikeCount,
                CategoryId = movie.CategoryId,
                SubCategoryId = movie.SubCategoryId
            };

            var response = new MoviesResponse
            {
                Meta = new MovieMetaData
                {
                    Service = "Movies",

                    Endpoint = $"/api/movies/{movieId}",

                    CategoryName = category.Name,

                    SubCategoryName = subcategory.Name,

                    TotalMovies = totalMoviesCount
                },
               
            };
            response.Data.Add(movieModel);
            // Return found movie
            return Ok(response);
        }
    }
}

using Azure.Core;
using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.Movie;
using cinemate.Services.TokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteMoviesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        private readonly TokenValidationService _tokenValidationService;

        public FavoriteMoviesController(DataContext dataContext, TokenValidationService tokenValidationService)
        {
            _dataContext = dataContext;
            _tokenValidationService = tokenValidationService;
        }

        [HttpPost]  
        public IActionResult AddToFavorites([FromForm] Guid movieId)
        {
            var userIdClaims = HttpContext
                             .User
                             .Claims
                             .FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?
                             .Value;

            if (userIdClaims == null)
            {
                userIdClaims = _tokenValidationService.ValidateToken();
                if (userIdClaims == null)
                {
                    return Unauthorized();
                }

            }
            if (!Guid.TryParse(userIdClaims, out var userIdGuid))
            {
                return BadRequest("Invalid user ID format"); // Если не удалось преобразовать в Guid, вернуть ошибку
            }

            if (!_dataContext.MoviesEntities.Any(m => m.Id == movieId))
            {
                return NotFound(new { message = "movie not found" });
            }

            var existfavorite = _dataContext.FavoriteMovies
              .FirstOrDefault(f => f.MovieId == movieId && f.UserId == userIdGuid);

            if (existfavorite != null)
            {
                return Conflict(new { message = "Фільм вже додано в обрані" });
            }

            var favorite = new FavoriteMovieEntity
            {
                UserId = userIdGuid,
                MovieId = movieId
            };

            _dataContext.FavoriteMovies.Add(favorite);
            _dataContext.SaveChanges();

            return Ok(new { message = "The movie is added to favorites" });
        }
             
        [HttpDelete("{movieId:guid}")]
        public IActionResult RemoveFromFavorites(Guid movieId)
        {
            var userIdClaims = HttpContext
                            .User
                            .Claims
                            .FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?
                            .Value;

            if (userIdClaims == null)
            {
                userIdClaims = _tokenValidationService.ValidateToken();
                if (userIdClaims == null)
                {
                    return Unauthorized();
                }

            }
            if (!Guid.TryParse(userIdClaims, out var userIdGuid))
            {
                return BadRequest("Invalid user ID format"); // Если не удалось преобразовать в Guid, вернуть ошибку
            }


            var favorite = _dataContext.FavoriteMovies
                .FirstOrDefault(f => f.MovieId == movieId && f.UserId == userIdGuid);

            if (favorite == null)
            {
                return NotFound(new { message = "Фільм не знайдено в обраних" });
            }

            _dataContext.FavoriteMovies.Remove(favorite);
            _dataContext.SaveChanges();

            return Ok(new { message = "Фільм видалено з обраних" });
        }
        // Новый метод для получения списка избранных фильмов
        [HttpGet]
        public IActionResult GetFavoriteMovies()
        {

            var userIdClaims = HttpContext
                              .User
                              .Claims
                              .FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?
                              .Value;

            if (userIdClaims == null)
            {
                userIdClaims = _tokenValidationService.ValidateToken();
                if (userIdClaims == null)
                {
                    return Unauthorized();
                }

            }
            if (!Guid.TryParse(userIdClaims, out var userIdGuid))
            {
                return BadRequest("Invalid user ID format"); // Если не удалось преобразовать в Guid, вернуть ошибку
            }

            // Получаем все фильмы, которые добавлены в избранное текущим пользователем
            var favoriteMovies = _dataContext.FavoriteMovies
                .Where(f => f.UserId == userIdGuid)
                .Select(f => new MoviesModel
                {
                    Id = f.Movie.Id,
                    Title = f.Movie.Title,
                    Description = f.Movie.Description,
                    Picture = f.Movie.Picture,
                    URL = f.Movie.URL,
                    ReleaseYear = f.Movie.ReleaseYear,
                    Director = f.Movie.Director,
                    Actors = f.Movie.Actors,
                    likeCount = f.Movie.likeCount,
                    dislikeCount = f.Movie.dislikeCount,
                    CategoryId = f.Movie.CategoryId,
                    SubCategoryId = f.Movie.SubCategoryId
                })
                .ToList();

            if (favoriteMovies.Count > 0)
            {
                var responseModel = new FavoriteMoviesResponseModel
                {
                    UserId = userIdGuid,
                    FavoriteMoviesCount = favoriteMovies.Count,
                    FavoriteMovies = favoriteMovies
                };
                return Ok(responseModel);
            }
            else
            {
                var responseModel = new FavoriteMoviesResponseModel
                {
                    UserId = userIdGuid,
                    FavoriteMoviesCount = favoriteMovies.Count,
                };
                return Ok(responseModel);

            }
            // Создаем модель ответа с метаданными
          

           
        }
    }
}

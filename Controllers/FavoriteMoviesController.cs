using Azure.Core;
using cinemate.Data;
using cinemate.Data.Entities;
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

        public FavoriteMoviesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        
        public IActionResult AddToFavorites([FromForm] Guid movieId)
        {
            String userId = HttpContext
                      .User
                      .Claims
                      .First(claim => claim.Type == ClaimTypes.Sid)
                      .Value;

            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                return Unauthorized();
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
                UserId = Guid.Parse(userId),
                MovieId = movieId
            };

            _dataContext.FavoriteMovies.Add(favorite);
            _dataContext.SaveChanges();

            return Ok(new { message = "The movie is added to favorites" });
        }
        
        
        [HttpDelete("{movieId:guid}")]
        public IActionResult RemoveFromFavorites(Guid movieId)
        {
            String userId = HttpContext
                     .User
                     .Claims
                     .First(claim => claim.Type == ClaimTypes.Sid)
                     .Value;

            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                return Unauthorized();
            }
            

            var favorite = _dataContext.FavoriteMovies
                .FirstOrDefault(f => f.MovieId == movieId && f.UserId == Guid.Parse(userId));

            if (favorite == null)
            {
                return NotFound(new { message = "Фільм не знайдено в обраних" });
            }

            _dataContext.FavoriteMovies.Remove(favorite);
            _dataContext.SaveChanges();

            return Ok(new { message = "Фільм видалено з обраних" });
        }
    }
}

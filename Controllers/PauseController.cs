using cinemate.Data.Entities;
using cinemate.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cinemate.Models.Movie;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PauseController : ControllerBase
    {
        private readonly DataContext _context;

        public PauseController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePause([FromBody] SavePauseRequest request)
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

            var pausedMovie = await _context.PausedMovies
                .FirstOrDefaultAsync(p => p.UserId == userIdGuid && p.MovieId == request.MovieId);

            if (pausedMovie == null)
            {
                pausedMovie = new PausedMovieEntity
                {
                    UserId = userIdGuid,
                    MovieId = request.MovieId,
                    PauseTime = request.PauseTime
                };
                _context.PausedMovies.Add(pausedMovie);
            }
            else
            {
                pausedMovie.PauseTime = request.PauseTime;
                pausedMovie.PausedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetPause(Guid movieId)
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
            // Получаем информацию о паузе
            var pausedMovie = await _context.PausedMovies
                .Include(p => p.Movie) // Включаем информацию о фильме
                .FirstOrDefaultAsync(p => p.UserId == userIdGuid && p.MovieId == movieId);

            if (pausedMovie == null)
                return NotFound();

            // Создаем MoviesModel из сущности MoviesEntities
            var movieModel = new MoviesModel
            {
                Id = pausedMovie.Movie.Id,
                Title = pausedMovie.Movie.Title,
                Description = pausedMovie.Movie.Description,
                Picture = pausedMovie.Movie.Picture,
                URL = pausedMovie.Movie.URL,
                ReleaseYear = pausedMovie.Movie.ReleaseYear,
                Director = pausedMovie.Movie.Director,
                Actors = pausedMovie.Movie.Actors,
                likeCount = pausedMovie.Movie.likeCount,
                dislikeCount = pausedMovie.Movie.dislikeCount,
                CategoryId = pausedMovie.Movie.CategoryId,
                SubCategoryId = pausedMovie.Movie.SubCategoryId
            };

            // Создаем MovieWithPauseResponse
            var response = new MovieWithPauseResponse
            {
                UserId = userIdGuid,
                MoviePause = movieModel,
                PauseTime = pausedMovie.PauseTime,
                PausedAt = pausedMovie.PausedAt
            };

            return Ok(response);
        }
    }
}

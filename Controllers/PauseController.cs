using cinemate.Data.Entities;
using cinemate.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cinemate.Models.Movie;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using cinemate.Services.TokenValidation;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PauseController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly TokenValidationService _tokenValidationService;

        public PauseController(DataContext context, TokenValidationService tokenValidationService)
        {
            _context = context;
            _tokenValidationService = tokenValidationService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePause([FromBody] SavePauseRequest request)
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

        [HttpGet]
        public async Task<IActionResult> GetPauses()
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
            // Получаем информацию о паузе
            var pausedMovies = await _context.PausedMovies
                       .Include(p => p.Movie) // Включаем информацию о фильме
                       .Where(p => p.UserId == userIdGuid) // Фильтруем по UserId
                       .ToListAsync(); // Получаем все записи для данного пользователя

            if (pausedMovies == null || !pausedMovies.Any())
                return NotFound();

            // Создаем список MoviesModel из сущностей MoviesEntities
            var movieModels = pausedMovies.Select(pausedMovie => new MoviesModel
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
            }).ToList();

            // Создаем MovieWithPauseResponse
            var response = new
            {
                UserId = userIdGuid,
                MoviePauses = movieModels, // Измените свойство на MoviePauses для списка
                PausedMoviesInfo = pausedMovies.Select(pm => new { pm.PauseTime, pm.PausedAt }).ToList() // Или добавьте другую логику для информации о паузах
            };

            return Ok(response);
        }
        }
}

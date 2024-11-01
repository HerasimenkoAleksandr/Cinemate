using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.Movie;
using cinemate.Services.TokenValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriorityCategoriesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        private readonly TokenValidationService _tokenValidationService;

        public PriorityCategoriesController(DataContext dataContext, TokenValidationService tokenValidationService)
        {
            _dataContext = dataContext;
            _tokenValidationService = tokenValidationService;
        }

        [HttpPost]
        public IActionResult SetPriorityCategories([FromBody] List<Guid> categoryIds)
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

            // Удаляем предыдущие приоритетные категории пользователя
            var existingCategories = _dataContext.PriorityCategories
                .Where(uc => uc.UserId == userIdGuid)
            .ToList();
            if(existingCategories.Any())
            {
                _dataContext.PriorityCategories.RemoveRange(existingCategories);
            }
           

            // Добавляем новые приоритетные категории
            var newPriorityCategories = categoryIds.Select(catId => new PriorityCategoriesEntity
            {
                Id = Guid.NewGuid(),
                UserId = userIdGuid,
                CategoryId = catId
            }).ToList();

            _dataContext.PriorityCategories.AddRange(newPriorityCategories);
            _dataContext.SaveChanges();

            return Ok(new { Message = "Priority categories set successfully." });
        }

        [HttpGet]
        public IActionResult GetMoviesFromPriorityCategories()
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

            // Получаем приоритетные категории пользователя
            var priorityCategories = _dataContext.PriorityCategories
                .Where(uc => uc.UserId == userIdGuid)
                .Select(uc => uc.CategoryId)
                .ToList();

            if (priorityCategories == null || !priorityCategories.Any())
            {
                return NotFound("No priority categories set.");
            }

            var groupedMovies = _dataContext.MoviesEntities
                .Where(m => priorityCategories.Contains(m.CategoryId))
                .Where(mov => mov.IsBanned == false)
                .GroupBy(m => m.CategoryId)
                .ToList();

            // Список для хранения отсортированных фильмов
            var orderedMovies = new List<MoviesEntities>();

            // Находим максимальное количество фильмов в любой из категорий
            int maxCount = groupedMovies.Max(g => g.Count());

            // Идем по порядку и берем по одному фильму из каждой категории
            for (int i = 0; i < maxCount; i++)
            {
                foreach (var group in groupedMovies)
                {
                    if (group.ElementAtOrDefault(i) != null)
                    {
                        orderedMovies.Add(group.ElementAt(i));  // Добавляем фильм из текущей категории
                    }
                }
            }

            if (orderedMovies == null || !orderedMovies.Any())
            {
                return NotFound("No movies found for your priority categories.");
            }

            var movieModel = orderedMovies.Select(m => new MoviesModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Picture = m.Picture,
                URL = m.URL,
                ReleaseYear = m.ReleaseYear,
                Director = m.Director,
                Actors = m.Actors,
                likeCount = m.likeCount,
                dislikeCount = m.dislikeCount,
                CategoryId = m.CategoryId,
                SubCategoryId = m.SubCategoryId
            }).ToList();

            var response = new MoviesResponse
            {
                Meta = new MovieMetaData
                {
                    Service = "PriorityCategories",
                    Endpoint = "/api/prioritycategories",
                    TotalMovies = movieModel.Count
                },
                Data = movieModel
            };
            return Ok(response);
        }
    }
}

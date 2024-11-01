using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.Movie;
using cinemate.Models.User;
using cinemate.Services.TokenValidation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ILogger<LikesController> _logger;
        private readonly DataContext _context;
        private readonly TokenValidationService _tokenValidationService;


        public LikesController(DataContext context, ILogger<LikesController> logger, DataContext dataContext, TokenValidationService tokenValidationService)
        {
            _context = context;
            _logger = logger;
            _tokenValidationService = tokenValidationService;
        }

        [HttpGet("get-status")]
        public IActionResult GetStatus(Guid movieId)
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

            var like = _context.LikeForMovie
                .FirstOrDefault(l => l.UserId == userIdGuid && l.MovieId == movieId);

            return Ok(new
            {
                IsLiked = like?.IsLiked ?? false,
                IsDisliked = like?.IsDisliked ?? false
            });
        }

        [HttpPost("toggle-like")]
        public IActionResult ToggleLike([FromBody] ToggleLikeRequestModel request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request first");
            }
            // Если пользователь не аутентифицирован, вернуть ошибку
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
            if (!Guid.TryParse(userIdClaims, out var userId))
            {
                return BadRequest("Invalid user ID format"); // Если не удалось преобразовать в Guid, вернуть ошибку
            }

            if (!Guid.TryParse(request.MovieId, out var movieId))
                {
                    return BadRequest("Invalid Movie ID"); // Если не удалось преобразовать в Guid, вернуть ошибку
                }


                var existingLike = _context.LikeForMovie
                    .FirstOrDefault(l => l.UserId == userId && l.MovieId == movieId);

               
                var movie = _context.MoviesEntities
                    .FirstOrDefault(m => m.Id == movieId);

                if (movie == null)
                {
                    return NotFound("Movie not found"); // Если фильм не найден, вернуть ошибку
                }

                if (existingLike != null)
                {
                    
                    // Запись существует, проверяем и обновляем
                    if (existingLike.IsLiked == request.IsLiked && existingLike.IsDisliked == request.IsDisliked)
                    {
                        // Если состояние совпадает, удаляем запись и уменьшаем счетчики
                        if (existingLike.IsLiked)
                        {
                            movie.likeCount = (movie.likeCount ?? 0) - 1;
                        }
                        if (existingLike.IsDisliked)
                        {
                            movie.dislikeCount = (movie.dislikeCount ?? 0) - 1;
                        }

                        _context.LikeForMovie.Remove(existingLike);
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Если состояние изменилось, обновляем
                        if (existingLike.IsLiked && request.IsLiked)
                        {
                            movie.likeCount = (movie.likeCount ?? 0) - 1;
                            existingLike.IsLiked = false;
                        }
                        else
                        {
                            if (!existingLike.IsLiked && request.IsLiked)
                            {
                                movie.likeCount = (movie.likeCount ?? 0) + 1;
                                existingLike.IsLiked = request.IsLiked;

                            }
                        }
                        if (existingLike.IsDisliked && request.IsDisliked)
                        {
                            movie.dislikeCount = (movie.dislikeCount ?? 0) - 1;
                            existingLike.IsDisliked = false;
                        }
                        else
                        {
                            if (!existingLike.IsDisliked && request.IsDisliked)
                            {
                                movie.dislikeCount = (movie.dislikeCount ?? 0) + 1;
                                existingLike.IsDisliked = request.IsDisliked;
                            }
                        }
                                     _context.SaveChanges();
                    }
                    return Ok(new
                    {
                        MovieId = request.MovieId,
                        likeCount = movie.likeCount,
                        dislikeCount = movie.dislikeCount

                    });
                }
                else
                {
                    _context.LikeForMovie.Add(new()
                      {
                          Id = Guid.NewGuid(),
                          UserId = userId,
                          MovieId = movieId,
                          IsLiked = request.IsLiked,
                          IsDisliked = request.IsDisliked
                      });
                      
                    
                      if (request.IsLiked)
                      {
                          movie.likeCount = (movie.likeCount ?? 0) + 1;
                      }
                      if (request.IsDisliked)
                      {
                          movie.dislikeCount = (movie.dislikeCount ?? 0) + 1;
                      }
                    _context.SaveChanges();
                    return Ok(new 
                    {
                        MovieId = request.MovieId,
                        likeCount = movie.likeCount,
                        dislikeCount = movie.dislikeCount

                    });
                }
        }
   
    }
}

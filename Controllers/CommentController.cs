using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.Movie;
using cinemate.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public CommentController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        public object AddComment([FromForm] CommentMoviesModel request)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {

                // Если пользователь не аутентифицирован, вернуть ошибку
                String userIdClaims = HttpContext
                      .User
                      .Claims
                      .First(claim => claim.Type == ClaimTypes.Sid)
                      .Value;

                if (userIdClaims == null)
                {
                    return NotFound("Элемент не найден");
                }
                if (!Guid.TryParse(userIdClaims, out var userId))
                {
                    return BadRequest("Invalid user ID format"); // Если не удалось преобразовать в Guid, вернуть ошибку
                }
                if (!Guid.TryParse(request.IdMovie.ToString(), out var movieId))
                {
                    return BadRequest(new { message = "Invalid Movie ID format" });
                }
             
                // Логика добавления комментария
                var user = _dataContext.Users.FirstOrDefault(u => u.Id == userId);
                var movie = _dataContext.MoviesEntities.FirstOrDefault(m => m.Id == movieId);
              
                if ( movie == null)
                {
                    return BadRequest(new { message = "Invalid movie ID" });
                }
                if (user == null )
                {
                    return BadRequest(new { message = "Invalid user  ID" });
                }

                var commentEntity = new CommentMoviesEntity
                {
                    Id = Guid.NewGuid(),
                    IdUsers = userId,
                    IdMovie = movieId,
                    Comment = request.Comment,
                    CommentDt = DateTime.Now,
                    User = user,
                    Movie = movie
                };

                _dataContext.CommentMovies.Add(commentEntity);
                _dataContext.SaveChanges();

                return Ok(new { message = "Comment added!", commentId = commentEntity.Id, userId= userId });
            }


            return Unauthorized(new { message = "User is not authenticated." });

        }

        // Метод для получения комментариев по ID фильма
        [HttpGet("{idMovie:guid}")]
        public IActionResult GetCommentsByMovieId(Guid idMovie)
        {
            var movie = _dataContext.MoviesEntities.Include(m => m.CommentMovies)
                          .FirstOrDefault(m => m.Id == idMovie);

            if (movie == null)
            {
                return NotFound(new { message = "Фильм не найден" });
            }

            var comments = _dataContext.CommentMovies
                .Where(c => c.IdMovie == idMovie)
                .Select(c => new CommentMoviesResponse.CommentDetails
                {
                    Id = c.Id,
                    IdUsers = c.IdUsers,
                    Comment = c.Comment,
                    CommentDt = c.CommentDt.ToString("HH:mm dd.MM.yyyy")
                }).ToList();

            var response = new CommentMoviesResponse
            {
                IdMovie = idMovie,
                Comments = comments
            };

            return Ok(response);
        }

        [HttpDelete("{idComment:guid}")]
        public IActionResult DeleteComment(Guid idComment)
        {
            // Найти комментарий по ID
            var comment = _dataContext.CommentMovies.FirstOrDefault(c => c.Id == idComment);
          
            if (comment == null)
            {
                return NotFound(new { message = "Комментарий не найден" });
            }

            // Удаление комментария
            _dataContext.CommentMovies.Remove(comment);
            _dataContext.SaveChanges();

            return Ok(new { message = "Комментарий успешно удалён" });
        }
    }
}

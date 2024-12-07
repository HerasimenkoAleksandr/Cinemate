using cinemate.Data;
using cinemate.Models.Notification;
using cinemate.Services.TokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly TokenValidationService _tokenValidationService;

        public NotificationController(DataContext context, TokenValidationService tokenValidationService)
        {
            _context = context;
            _tokenValidationService = tokenValidationService;
        }

        // Возвращаем уведомления для текущего пользователя
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userIdClaims = HttpContext
                           .User
                           .Claims
                           .FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;

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

            // Получаем все уведомления для пользователя
            var notifications = _context.UserNotifications
                .Where(un => un.UserId == userIdGuid)
                .Select(un => new NotificationModel
                {
                    Id = un.NotificationId,
                    Message = un.Notification.Message,
                    IsRead = un.IsRead,
                    SentAt = un.SentAt.ToString("HH:mm dd.MM.yyyy"),
                    MovieId = un.Notification.MovieId
                }).ToList();

            // Подсчет общего количества уведомлений и количества непрочитанных
            var totalCount = notifications.Count;
            var unreadCount = notifications.Count(n => !n.IsRead);

            // Возвращаем мета-данные и список уведомлений
            var response = new NotificationResponseModel
            {
                TotalCount = totalCount,
                UnreadCount = unreadCount,
                Url = "http://backend.cinemate.pp.ua/api/Notification", // URL для получения уведомлений
                Notifications = notifications
            };

            return Ok(response);
        }

        // Изменение статуса уведомления на "прочитано"
        [HttpPatch("mark")]
        public async Task<IActionResult> Mark([FromBody] UpdateNotificationStatusModel notificationStatus)
        {
            var userIdClaims = HttpContext
                            .User
                            .Claims
                            .FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;

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

            var userNotification = _context.UserNotifications
                .FirstOrDefault(un => un.UserId == userIdGuid && un.NotificationId == notificationStatus.NotificationId);

            if (userNotification == null)
            {
                return NotFound("notification not found");
            }

            if (userNotification.IsRead==true)
            {
                userNotification.IsRead=false;
            }else
            {
                userNotification.IsRead = notificationStatus.IsRead;
            }
            
            await _context.SaveChangesAsync();

            return Ok("Notification status updated successfully");
        }
    }
}

using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.User;
using cinemate.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly DataContext _dataContext;

        private readonly IHashService _hashService;
        public AuthController(DataContext dataContext, IHashService hashService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
        }
       

        [HttpPost]
        public object Authenticate([FromForm] AuthenticateUserModel request)
        {
            // Ищем пользователя с указанным email
            User user = _dataContext.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                HttpContext.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                return new { status = "Credentials rejected" };
            }
            // Пользователь найден, формируем DK из пароля, который был отправлен, и соли

            String dk = _hashService.HexString(user.PasswordSalt +request.Password);
            if (user.PasswordDk != dk)
            {
                HttpContext.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                return new { status = "Credentials rejected" };
            }
            // Сохраняем в сессии факт успешной аутентификации
            HttpContext.Session.SetString("AuthUserId", user.Id.ToString());
            return new
            {
                status = "OK",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.UserName,
                    firstName = user.FirstName,
                    surname = user.Surname,
                    phoneNumber = user.PhoneNumber,
                    avatar = user.Avatar,
                    reistrerDt = user.RegistrerDt
                }
            };
        }

        [HttpDelete]
        public object SignOut()
        {
            // Получаем идентификатор пользователя из сессии
            var userId = HttpContext.Session.GetString("AuthUserId");

            if (userId == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return new { status = "User not authenticated" };
            }

            // Очищаем сессию для пользователя
            HttpContext.Session.Remove("AuthUserId");

            return new { status = "OK" };
        }


    }
}

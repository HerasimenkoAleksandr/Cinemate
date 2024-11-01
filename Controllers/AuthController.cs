using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.User;
using cinemate.Services.Hash;
using cinemate.Services.TokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cinemate.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly DataContext _dataContext;

        private readonly IHashService _hashService;

        private readonly IConfiguration _configuration;

        private readonly TokenValidationService _tokenValidationService;


        public AuthController(DataContext dataContext, IHashService hashService, IConfiguration configuration, TokenValidationService tokenValidationService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
            _configuration = configuration;
            _tokenValidationService = tokenValidationService;
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

            var createToken = GenerateJwtToken(user);

            return new
            {
                status = "OK",
                token = createToken,  // Возвращаем JWT-токен
                user = new
                {
                    id = user.Id,
                    Role = user.Role,
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

        //[HttpDelete]
        //public object SignOut()
        //{
        //    // Получаем идентификатор пользователя из сессии
        //    var userId = HttpContext.Session.GetString("AuthUserId");

        //    if (userId == null)
        //    {
        //        HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //        return new { status = "User not authenticated" };
        //    }

        //    // Очищаем сессию для пользователя
        //    HttpContext.Session.Remove("AuthUserId");

        //    return new { status = "OK" };
        //}



        [HttpDelete]
        [Authorize]
        public IActionResult SignOut()
        {
            // Получаем идентификатор пользователя из сессии
            var userIdFromSession = HttpContext.Session.GetString("AuthUserId");

            if (userIdFromSession != null)
            {
                // Очищаем сессию для пользователя
                HttpContext.Session.Remove("AuthUserId");
                return Ok(new { status = "Session cleared, user signed out" });
            }
            else
            {
                userIdFromSession = _tokenValidationService.ValidateToken();
                if (userIdFromSession != null)
                {
                    return Ok(new { status = $"Token, user signed out" });
                }
               
            }
            return Unauthorized(new { status = "User not authenticated" });
        }

        

//    // Если сессии нет, проверяем токен
//    var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

//            if (authHeader != null && authHeader.StartsWith("Bearer "))
//            {
//                var token = authHeader.Substring("Bearer ".Length).Trim();
               
//                if (token!=null) 
//                {
//                    var jwtHandler = new JwtSecurityTokenHandler();
//    var jwtToken = jwtHandler.ReadJwtToken(token);

//    // Извлекаем необходимые данные из токена
//    var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
//                    return Ok(new { status = $"Token, user signed out - {userId}" });
//                }
                    
                
              
//            }
//            else
//{
//    return Unauthorized(new { status = "No token provided" });
//}

//Если не удалось найти пользователя ни в сессии, ни по токену
//            return Unauthorized(new { status = "User not authenticated" });
//        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
      //  new Claim(ClaimTypes.Name, user.UserName)
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["TokenExpirationMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

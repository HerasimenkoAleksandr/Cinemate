using cinemate.Data;
using cinemate.Data.Entities;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Security.Claims;

namespace cinemate.Middleware
{
    public class AuthSessionMiddleware
    {
        // ланцюг Middleware утворюється при інсталяції проєкту
        // і кожен учасник (ланка) Middleware одержує при створенні
        // посилання на наступну ланку (_next). Підключення Middleware
        // здійснюється у Program.cs
        private readonly RequestDelegate _next;

        public AuthSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // є дві схеми включення Middleware - синхронна та асинхронна
        // Для них передбачені методи Invoke або InvokeAsync
        public async Task InvokeAsync(HttpContext context,
            DataContext _dataContext, IConfiguration _configuration// інжекція у Middleware іде через метод
        )
        {
            // Задача - перевірити наявність у сесії збереженого AuthUserId
            // за наявності - перевірити валідність шляхом пошуку у БД
            if (context.Session.Keys.Contains("AuthUserId"))
            {
                string userIdString = context.Session.GetString("AuthUserId")!; // Замените на фактический ID пользователя
                
                var userId= Guid.Parse(userIdString);
                User user = _dataContext.Users.FirstOrDefault(u => u.Id == userId);
              


                if (user != null)
                {
                    // перекладаємо відомості про користувача до 
                    // контексту НТТР у формалізмі Claims
                    Claim[] claims = new Claim[]
                    {
                       new Claim(ClaimTypes.Sid, user.Id.ToString()), // Идентификатор пользователя
                       new Claim(ClaimTypes.Name, user.UserName ?? ""), // Имя пользователя
                       new Claim(ClaimTypes.Email, user.Email ?? ""), // Электронная почта
                       new Claim(ClaimTypes.UserData, user.Avatar ?? ""), // Аватар
                       new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""), // Телефонный номе
                       new Claim("FirstName", user.FirstName ?? ""), // Имя (пользовательское поле)
                       new Claim("Surname", user.Surname ?? ""), // Фамилия (пользовательское поле)
                    };
                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            claims,
                            nameof(AuthSessionMiddleware)
                        )
                    );
                }
            }
            // тіло Middleware ділиться на дві частини:
            // "прямий" хід (до виклику наступної ланки) ...
            await _next(context);
            // ... та зворотній хід - після виклику.
        }
    }
}

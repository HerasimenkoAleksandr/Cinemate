using Microsoft.AspNetCore.Mvc;

namespace cinemate.Models.User
{
    public class AuthenticateUserModel
    {
        [FromForm(Name = "email")]
        public string Email { get; set; }

        [FromForm(Name = "password")]
        public string Password { get; set; }
    }
}

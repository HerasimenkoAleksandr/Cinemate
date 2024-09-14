using Microsoft.AspNetCore.Mvc;

namespace cinemate.Models.User
{
    public class AuthenticateUserModel
    {
        [FromForm(Name = "email")]
        public String Email { get; set; }

        [FromForm(Name = "password")]
        public String Password { get; set; }
    }
}

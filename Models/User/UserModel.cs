using Microsoft.AspNetCore.Mvc;

namespace cinemate.Models.User
{
    public class UserModel
    {
        [FromForm(Name = "userName")]
        public string? UserName { get; set; }

        [FromForm(Name = "firstName")]
        public string? FirstName { get; set; }

        [FromForm(Name = "surname")]
        public string? Surname { get; set; }

        [FromForm(Name = "email")]
        public string? Email { get; set; }

        [FromForm(Name = "phoneNumber")]
        public string? PhoneNumber { get; set; }

        [FromForm(Name = "password")]
        public string? Password { get; set; }

        [FromForm(Name = "avatar")]
        public IFormFile? Avatar { get; set; }
    }
}

using cinemate.Data;
using cinemate.Models.User;
using cinemate.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupUserController : ControllerBase
    {

        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;

        public SignupUserController(DataContext dataContext, IHashService hashService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UserModel userModel)
        {
            SignupUserValidation results = new SignupUserValidation();
            bool isFormValid = true;


            // Проверка, email 
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (String.IsNullOrEmpty(userModel.Email))
            {
                results.EmailErrorMessage = "Email cannot be empty";
                isFormValid = false;
            }
            else 
                if(!Regex.IsMatch(userModel.Email, emailPattern))
                {
                results.EmailErrorMessage = "Invalid email format";
                isFormValid = false;
            }
                else
                {
                // Проверка, существует ли email в базе данных
                bool emailExists = await _dataContext.Users.AnyAsync(u => u.Email == userModel.Email);
                if (emailExists)
                {
                    results.EmailErrorMessage = "Email already exists";
                    isFormValid = false;
                }
                }
            
            // Проверка, Password
            if (String.IsNullOrEmpty(userModel.Password))
            {
                results.EmailErrorMessage = "Password cannot be empty!";
                isFormValid = false;
            }

            // Проверка, Avatar
            String? fileName=null;
            if (isFormValid && userModel.Avatar != null && userModel.Avatar.Length > 0)
            {
                //изменяем имя файла на уникальное и потом сохраняем файл 
                int dotPosition = userModel.Avatar.FileName.LastIndexOf('.');
                if (dotPosition ==-1) 
                {
                    results.AvataErrorMessager = "Files without extension are not accepted";
                    isFormValid = false;
                }
                else 
                {
                    String ext = userModel.Avatar.FileName.Substring(dotPosition);
                    String directory =Directory.GetCurrentDirectory();
                    String savedName;
                   // String fileName;
                    do
                    {
                        fileName = Guid.NewGuid() + ext;
                        savedName = Path.Combine(directory, "wwwroot", "avatars", fileName);
                       
                    } while (System.IO.File.Exists(savedName));
                    using Stream stream = System.IO.File.OpenWrite(savedName);
                    userModel.Avatar.CopyTo(stream);
                }
               
            }
            if(isFormValid)
            {
                String salt = _hashService.HexString(Guid.NewGuid().ToString());
                String dk = _hashService.HexString(salt+userModel.Password);
                _dataContext.Users.Add(new()
                {
                    Id = Guid.NewGuid(),
                    UserName = userModel.UserName,
                    FirstName = userModel.FirstName,
                    Surname=userModel.Surname,
                    Email = userModel.Email,
                    PhoneNumber=userModel.PhoneNumber,
                    PasswordSalt=salt,
                    PasswordDk=dk,
                    Avatar = fileName,
                    RegistrerDt = DateTime.Now,
                    DeleteDt = null
                 });
                _dataContext.SaveChanges();
                return Ok(new { message = "User registered successfully!" });
            }


            return BadRequest(results);
        }
    }
}



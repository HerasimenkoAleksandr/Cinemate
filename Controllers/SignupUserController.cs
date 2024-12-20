﻿using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.User;
using cinemate.Services.Hash;
using cinemate.Services.TokenValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupUserController : ControllerBase
    {

        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;
        private readonly TokenValidationService _tokenValidationService;
        public SignupUserController(DataContext dataContext, IHashService hashService, ILogger<SignupUserController> logger, TokenValidationService tokenValidationService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
            _tokenValidationService = tokenValidationService;
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
                results.PasswordErrorMessage = "Password cannot be empty!";
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

        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromForm] UserModel userModel)
        {

            // Если сессии нет, проверяем токен
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {


                if (userModel == null)
                {
                    return BadRequest("User model is null.");
                }
              

                var userIdClaims = HttpContext
                                .User
                                .Claims
                                .FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?
                                .Value;

                if (userIdClaims == null)
                {
                    userIdClaims = _tokenValidationService.ValidateToken();
                    if(userIdClaims == null)
                    {
                        return Unauthorized();
                    }

                }
                if (!Guid.TryParse(userIdClaims, out var userId))
                {
                    return BadRequest("Invalid user ID format"); // Если не удалось преобразовать в Guid, вернуть ошибку
                }

                // Проверка, существует ли пользователь
                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Обновляем имя пользователя, если передано
                if (!string.IsNullOrEmpty(userModel.UserName))
                {
                    user.UserName = userModel.UserName;
                }

                // Обновляем имя (FirstName), если передано
                if (!string.IsNullOrEmpty(userModel.FirstName))
                {
                    user.FirstName = userModel.FirstName;
                }

                // Обновляем фамилию (Surname), если передано
                if (!string.IsNullOrEmpty(userModel.Surname))
                {
                    user.Surname = userModel.Surname;
                }

                // Обновляем номер телефона (PhoneNumber), если передано
                if (!string.IsNullOrEmpty(userModel.PhoneNumber))
                {
                    user.PhoneNumber = userModel.PhoneNumber;
                }

                // Обновляем пароль, если передано
                if (!string.IsNullOrEmpty(userModel.Password))
                {
                    // Хешируем новый пароль
                    string salt = _hashService.HexString(Guid.NewGuid().ToString());
                    string dk = _hashService.HexString(salt + userModel.Password);
                    user.PasswordSalt = salt;
                    user.PasswordDk = dk;
                }

                // Обновляем аватар, если передан
                if (userModel.Avatar != null && userModel.Avatar.Length > 0)
                {
                    // Изменяем имя файла на уникальное и сохраняем файл
                    int dotPosition = userModel.Avatar.FileName.LastIndexOf('.');
                    if (dotPosition == -1)
                    {
                        throw new Exception("Files without extension are not accepted");
                        //return BadRequest(new { message = "Files without extension are not accepted" });
                    }
                    else
                    {
                        string ext = userModel.Avatar.FileName.Substring(dotPosition);
                        string directory = Directory.GetCurrentDirectory();
                        string fileName;
                        string savedName;

                        do
                        {
                            fileName = Guid.NewGuid() + ext;
                            savedName = Path.Combine(directory, "wwwroot", "avatars", fileName);
                        } while (System.IO.File.Exists(savedName));

                        using Stream stream = System.IO.File.OpenWrite(savedName);
                        await userModel.Avatar.CopyToAsync(stream);

                        // Удаление старого аватара
                        if (!string.IsNullOrEmpty(user.Avatar))
                        {
                            string oldAvatarPath = Path.Combine(directory, "wwwroot", "avatars", user.Avatar);
                            if (System.IO.File.Exists(oldAvatarPath))
                            {
                                System.IO.File.Delete(oldAvatarPath);
                            }
                        }

                        user.Avatar = fileName;
                    }
                }

                // Сохраняем изменения в базе данных
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();

                return Ok(new
                {   
                    message = "User updated successfully!",
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
                });

                
            }



            return Unauthorized(new { message = "User is not authenticated." });
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
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
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userIdGuid);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(new
            {
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
            });
        }
    }
}



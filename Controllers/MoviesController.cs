﻿using Azure;
using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.Category;
using cinemate.Models.Movie;
using cinemate.Services.TokenValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        private readonly TokenValidationService _tokenValidationService;

        public MoviesController(DataContext dataContext, TokenValidationService tokenValidationService)
        {
            _dataContext = dataContext;
            _tokenValidationService = tokenValidationService;
        }

        [HttpGet]
        public IActionResult GetMovies([FromQuery] string? categoryId, [FromQuery] string? subcategoryId)
        {

            

            if (subcategoryId != null)
            {
                if ( !Guid.TryParse(subcategoryId, out Guid subcategoryGuid))
                {
                    return BadRequest("Invalid  subcategoryID");
                }

                // Запрос к базе данных для получения фильмов по ID категории и подкатегории
                var MoviesFromSubCat = _dataContext.MoviesEntities
                                .Where(m => m.SubCategoryId == subcategoryGuid)
                                .Where(mov=>mov.IsBanned==false)
                                .ToList();
               
                if (MoviesFromSubCat == null || !MoviesFromSubCat.Any())
                {
                    return NotFound("No movies found for the given category and subcategory.");
                }

                // Получаем категорию и подкатегорию на основе первого фильма в списке
                var category = _dataContext.Gategories
                    .SingleOrDefault(c => c.Id == MoviesFromSubCat.First().CategoryId);

                var subcategory = _dataContext.SubCategoriesEntity
                    .SingleOrDefault(sc => sc.Id == MoviesFromSubCat.First().SubCategoryId);

                var movieModel = MoviesFromSubCat.Select(mfsc => new MoviesModel
                {
                    Id = mfsc.Id,
                    Title = mfsc.Title,
                    Description = mfsc.Description,
                    Picture = mfsc.Picture,
                    URL = mfsc.URL,
                    ReleaseYear = mfsc.ReleaseYear,
                    Director = mfsc.Director,
                    Actors = mfsc.Actors,
                    Duration = mfsc.Duration,
                    likeCount = mfsc.likeCount,
                    dislikeCount = mfsc.dislikeCount,
                    CategoryId = mfsc.CategoryId,
                    SubCategoryId = mfsc.SubCategoryId
                }).ToList();


                var response = new MoviesResponse
                {
                    Meta = new MovieMetaData
                    {
                        Service = "Movies",

                        Endpoint = $"/api/movies?{subcategoryId}",

                        CategoryName = category.Name,

                        SubCategoryName = subcategory.Name,

                        TotalMovies = subcategory.ContentCount,
                    },
                    Data= movieModel
                };
             


                // Возвращаем найденные фильмы
                return Ok(response);


            }
            else
            {
                if (categoryId != null)
                {

                    if (!Guid.TryParse(categoryId, out Guid categoryGuid) )
                    {
                        return BadRequest("Invalid categoryID");
                    }
                    var MoviesFromCat = _dataContext.MoviesEntities
                                   .Where(m => m.CategoryId == categoryGuid)
                                   .Where(mov => mov.IsBanned == false)
                                   .ToList();
                    // Если фильмы не найдены, возвращаем 404
                    if (MoviesFromCat == null || !MoviesFromCat.Any())
                    {
                        return NotFound("No movies found for the given category and subcategory.");
                    }

                    // Получаем категорию и подкатегорию на основе первого фильма в списке
                    var category = _dataContext.Gategories
                        .SingleOrDefault(c => c.Id == MoviesFromCat.First().CategoryId);

                    var movieModel = MoviesFromCat.Select(mfsc => new MoviesModel
                    {
                        Id = mfsc.Id,
                        Title = mfsc.Title,
                        Description = mfsc.Description,
                        Picture = mfsc.Picture,
                        URL = mfsc.URL,
                        ReleaseYear = mfsc.ReleaseYear,
                        Director = mfsc.Director,
                        Actors = mfsc.Actors,
                        Duration = mfsc.Duration,
                        likeCount = mfsc.likeCount,
                        dislikeCount = mfsc.dislikeCount,
                        CategoryId = mfsc.CategoryId,
                        SubCategoryId = mfsc.SubCategoryId
                    }).ToList();


                        var totalMoviesCount = _dataContext.MoviesEntities
                    .Where(m => m.CategoryId == category.Id)
                    .Count();

                    var response = new MoviesResponse
                    {
                        Meta = new MovieMetaData
                        {
                            Service = "Movies",

                            Endpoint = $"/api/movies?{categoryId}",

                            CategoryName = category.Name,

                            TotalSubCategory = category.ContentCount,

                            TotalMovies = totalMoviesCount,
                        },
                        Data = movieModel
                    };

                    return Ok(response);
                }
                else
                {
                    return BadRequest("Invalid CategoryId and subcategoryId ");
                }
            }
        }


        [HttpGet("{movieId}")]
        public IActionResult GetMovieById(Guid movieId)
        {

            var movie = _dataContext.MoviesEntities
                            .SingleOrDefault(m => m.Id == movieId);
                            

            var totalMoviesCount = _dataContext.MoviesEntities.Count();
            // If no movie found, return 404
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }
            if (movie.IsBanned == true)
            {
                return NotFound("Movie is blocked");
            }
            var category = _dataContext.Gategories
                    .SingleOrDefault(c => c.Id == movie.CategoryId);


            var subcategory = _dataContext.SubCategoriesEntity
                    .SingleOrDefault(c => c.Id == movie.SubCategoryId);
            
            var movieModel = new MoviesModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Picture = movie.Picture,
                URL = movie.URL,
                ReleaseYear = movie.ReleaseYear,
                Director = movie.Director,
                Actors = movie.Actors,
                Duration= movie.Duration,
                likeCount = movie.likeCount,
                dislikeCount = movie.dislikeCount,
                CategoryId = movie.CategoryId,
                SubCategoryId = movie.SubCategoryId
            };

            var response = new MoviesResponse
            {
                Meta = new MovieMetaData
                {
                    Service = "Movies",

                    Endpoint = $"/api/movies/{movieId}",

                    CategoryName = category.Name,

                    SubCategoryName = subcategory.Name,

                    TotalMovies = totalMoviesCount
                },
               
            };
            response.Data.Add(movieModel);
            // Return found movie
            return Ok(response);
        }
        [HttpPatch("{movieId}/ban")]
        public IActionResult BanMovie(Guid movieId)
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
           
            var user = _dataContext.Users.SingleOrDefault(u => u.Id == userIdGuid);

            if (user == null || user.Role != "Admin")
            {
                return Forbid("You do not have permission to ban this movie!!!!!!!!.");
            }

            var movie = _dataContext.MoviesEntities.SingleOrDefault(m => m.Id == movieId);

            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            movie.IsBanned = true;
            _dataContext.SaveChanges();

            return Ok(new { Message = "Movie banned successfully." });
        }


        [HttpPatch("{movieId}/unban")]
        public IActionResult UnbanMovie(Guid movieId)
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

            var user = _dataContext.Users.SingleOrDefault(u => u.Id == userIdGuid);

            if (user == null || user.Role != "Admin")
            {
                return Forbid("You do not have permission to unban this movie.");
            }

            var movie = _dataContext.MoviesEntities.SingleOrDefault(m => m.Id == movieId);

            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            movie.IsBanned = false;
            _dataContext.SaveChanges();

            return Ok(new { Message = "Movie unbanned successfully." });
        }

        [HttpGet("banned")]
        public IActionResult GetBannedMovies()
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

            var user = _dataContext.Users.SingleOrDefault(u => u.Id == userIdGuid);

            if (user == null || user.Role != "Admin")
            {
                return Forbid("You do not have permission to view the list of blocked movies.");
            }
            // Query the database for banned movies
            var bannedMovies = _dataContext.MoviesEntities
                .Where(m => m.IsBanned == true)
                .ToList();

            // If no banned movies found, return a 404
            if (bannedMovies == null || !bannedMovies.Any())
            {
                return NotFound("No banned movies found.");
            }

            // Map the banned movies to the MoviesModel
            var movieModel = bannedMovies.Select(m => new MoviesModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Picture = m.Picture,
                URL = m.URL,
                ReleaseYear = m.ReleaseYear,
                Director = m.Director,
                Actors = m.Actors,
                likeCount = m.likeCount,
                dislikeCount = m.dislikeCount,
                CategoryId = m.CategoryId,
                SubCategoryId = m.SubCategoryId
            }).ToList();
            var totalMoviesCount = _dataContext.MoviesEntities.Count();
            // Prepare the response with metadata
            var response = new MoviesResponse
            {
                Meta = new MovieMetaData
                {
                    Service = "Movies",
                    Endpoint = "/api/movies/banned",
                    TotalMovies = movieModel.Count,
                    StatusMovies = "blocke",
                },
                Data = movieModel
            };

            // Return the response with the list of banned movies
            return Ok(response);
        }

        [HttpGet("search")]
        public IActionResult SearchMovies([FromQuery] string? search)
        {

            if (string.IsNullOrEmpty(search))
            {
                return BadRequest("Необходимо указать поисковый запрос.");
            }

            // Очистка и обрезка пробелов в поисковом запросе
            search = search.Trim();
            Console.WriteLine($"Searching for: {search}"); 

            bool isReleaseYearSearch = int.TryParse(search, out int releaseYear);

            var searchResults = _dataContext.MoviesEntities
             .Where(m => !m.IsBanned &&
             (m.Title.ToLower().Contains(search.ToLower()) ||
             (m.Description != null && m.Description.ToLower().Contains(search.ToLower())) ||
             (m.Director != null && m.Director.ToLower().Contains(search.ToLower())) ||
             (isReleaseYearSearch && m.ReleaseYear == releaseYear)))
             .ToList();

            // Если фильмы не найдены, вернуть 404
            if (searchResults == null || !searchResults.Any())
            {
                return NotFound("Фильмы по данному запросу не найдены.");
            }

            // Преобразование результатов поиска в модель для ответа
            var movieModel = searchResults.Select(m => new MoviesModel
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Picture = m.Picture,
                URL = m.URL,
                ReleaseYear = m.ReleaseYear,
                Director = m.Director,
                Actors = m.Actors,
                Duration = m.Duration,
                likeCount = m.likeCount,
                dislikeCount = m.dislikeCount,
                CategoryId = m.CategoryId,
                SubCategoryId = m.SubCategoryId
            }).ToList();

            var response = new MoviesResponse
            {
                Meta = new MovieMetaData
                {
                    Service = "Movies",
                    Endpoint = $"/api/movies/search?query={search}",
                    TotalMovies = movieModel.Count
                },
                Data = movieModel
            };

            // Возвращаем найденные фильмы
            return Ok(response);
        }

    }
}

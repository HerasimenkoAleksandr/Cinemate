using cinemate.Data;
using cinemate.Data.Entities;
using cinemate.Models.Category;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace cinemate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")] 
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public CategoriesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var totalSubcategoriesCount = _dataContext.Gategories
    .SelectMany(g => g.SubCategories) // Выбираем все подкатегории
    .Count();
            var meta = new MetaData
            {
                Service = "Categories",
                Endpoint = "/api/categories",
                Total = 13,
                PerPage = 4,
                Page = 1,
                LastPage = 4,
                CountSubcategories = totalSubcategoriesCount,
                Locale = "en-US"
            };

            var categories = await _dataContext.Gategories.Select(g => new Category
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Picture = g.Picture,
                ContentCount = g.ContentCount
            }).ToListAsync();

            var response = new CategoryResponse
            {
                Meta = meta,
                Data = categories
            };

            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {

            var category = await _dataContext.Gategories
                .Where(c => c.Id == id)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Picture = c.Picture,
                    ContentCount = c.ContentCount,
                    SubCategories =c.SubCategories.Select(sc => new SubCategories
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        Description = sc.Description,
                        ParentCategoryId = sc.ParentCategoryId,
                        Picture = sc.Picture,
                        ContentCount = sc.ContentCount,
                        ParentCategoryName = c.Name

                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            var meta = new MetaData
            {
                Service = "Categories",
                Endpoint = $"/api/categories/{id}",
                Total = category.ContentCount,
                PerPage = 3, 
                Page = 1,     
                LastPage = (int)Math.Ceiling((double)category.ContentCount / 3), // Adjust per page count
                CountSubcategories = category.ContentCount, // Total number of subcategories in the category
                Locale = "en-US"
            };

            var response = new SubCategoriesResponse
            {
                Meta = meta,
                Data = category
            };
            return Ok(response);
        }
    }
}



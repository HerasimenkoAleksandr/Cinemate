using static cinemate.Controllers.CategoriesController;

namespace cinemate.Models.Category
{

    public class CategoryResponse
    {
        public MetaData Meta { get; set; }
        public List<Category> Data { get; set; }
    }
}

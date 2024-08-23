using cinemate.Data.Entities;
using cinemate.Migrations;

namespace cinemate.Models.Category
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public int ContentCount { get; set; }
        public List<SubCategories> SubCategories { get; set; }

        public static implicit operator List<object>(Category v)
        {
            throw new NotImplementedException();
        }
    }
}

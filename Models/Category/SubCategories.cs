using cinemate.Data.Entities;

namespace cinemate.Models.Category
{
    public class SubCategories
    {
   
            public Guid Id { get; set; }
            public String Name { get; set; }
            public String Description { get; set; }
            public Guid ParentCategoryId { get; set; } 
            public String Picture { get; set; }
            public int ContentCount { get; set; }
            public String ParentCategoryName { get; set; } 
        
    }
}

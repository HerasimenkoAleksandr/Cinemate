namespace cinemate.Data.Entities
{
    public class SubCategoriesEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public String Name { get; set; }
        public String Description { get; set; }
        public Guid ParentCategoryId { get; set; } 
        public String Picture { get; set; }
        public int ContentCount { get; set; }

        public Gategories ParentCategory { get; set; }

        public ICollection<MoviesEntities> Movies { get; set; }
    }
}

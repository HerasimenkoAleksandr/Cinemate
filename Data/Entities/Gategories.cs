namespace cinemate.Data.Entities
{
    public class Gategories
    {
        public Guid Id { get; set; } 
        public String Name { get; set; } = null!;
        public String Description { get; set; } = null!;
        public String Picture { get; set; } = null!;
        public int ContentCount { get; set; }

        public ICollection<SubCategoriesEntity> SubCategories { get; set; }
       
        public ICollection<MoviesEntities> Movies { get; set; }

    }
}

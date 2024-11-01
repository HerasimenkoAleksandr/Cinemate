namespace cinemate.Data.Entities
{
    public class PriorityCategoriesEntity
    {
        public Guid Id { get; set; } 
        public Guid UserId { get; set; } 
        public Guid CategoryId { get; set; } 

        public User User { get; set; } = null!;
        public Gategories Category { get; set; } = null!;
    }
}

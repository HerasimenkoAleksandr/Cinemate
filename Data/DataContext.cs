using cinemate.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace cinemate.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Gategories> Gategories { get; set; }
        public DbSet<SubCategoriesEntity> SubCategoriesEntity { get; set; }
        public DbSet<Entities.MoviesEntities> MoviesEntities { get; set; } // Проверьте правильность имени

        public DataContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MoviesEntities>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MoviesEntities>()
                .HasOne(m => m.SubCategory)
                .WithMany(sc => sc.Movies)
                .HasForeignKey(m => m.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

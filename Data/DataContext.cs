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
        public DbSet<LikeForMovie> LikeForMovie { get; set; }
        public DbSet<CommentMoviesEntity> CommentMovies { get; set; }
        public DbSet<FavoriteMovieEntity> FavoriteMovies { get; set; }
        public DbSet<PausedMovieEntity> PausedMovies { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи между User и PausedMovieEntity (один ко многим)
            modelBuilder.Entity<User>()
                .HasMany(u => u.PausedMovies)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Ограничиваем удаление

            // Настройка связи между MoviesEntities и PausedMovieEntity (один ко многим)
            modelBuilder.Entity<MoviesEntities>()
                .HasMany(m => m.PausedMovies)
                .WithOne(p => p.Movie)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.Restrict);  // Ограничиваем удаление

            modelBuilder.Entity<MoviesEntities>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // не дозволить видалити Category, якщо звязок з існуючим Movies

            modelBuilder.Entity<MoviesEntities>()
                .HasOne(m => m.SubCategory)
                .WithMany(sc => sc.Movies)
                .HasForeignKey(m => m.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);  // не дозволить видалити SubCategory, якщо звязок з існуючим Movies

            modelBuilder.Entity<LikeForMovie>()
                 .HasOne(ul => ul.User)
                 .WithMany()
                 .HasForeignKey(ul => ul.UserId)
                 .OnDelete(DeleteBehavior.Cascade);  // Видалення пов'язане з користувачем

            modelBuilder.Entity<LikeForMovie>()
                .HasOne(ul => ul.Movie)
                .WithMany()
                .HasForeignKey(ul => ul.MovieId)
                .OnDelete(DeleteBehavior.Cascade);  // Видалення пов'язане з фільмом

            modelBuilder.Entity<CommentMoviesEntity>()
                .HasOne(c => c.User)
                .WithMany(u => u.CommentMovies)
                .HasForeignKey(c => c.IdUsers);

            // Связь между MoviesEntities и CommentEntity (один ко многим)
            modelBuilder.Entity<CommentMoviesEntity>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.CommentMovies)
                .HasForeignKey(c => c.IdMovie);

            // Настройка каскадного удаления для FavoriteMovie
            modelBuilder.Entity<FavoriteMovieEntity>()
                .HasOne(f => f.Movie)
                .WithMany(m => m.FavoriteMovies)  // Связь "Один ко многим"
                .HasForeignKey(f => f.MovieId)
                .OnDelete(DeleteBehavior.Cascade);  // Удаление избранных фильмов при удалении фильма

            modelBuilder.Entity<FavoriteMovieEntity>()
                .HasOne(f => f.User)
                .WithMany(u => u.FavoriteMovies)  // Связь "Один ко многим"
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Удаление избранных фильмов при удалении пользователя
        
    }
    }
}

namespace cinemate.Data.Entities
{
    public class User
    {
        internal readonly object UserNotifications;

        public Guid Id { get; set; }
        public String? UserName { get; set; } 
        public String? FirstName { get; set; }
        public String? Surname { get; set; }
        public String? Email { get; set; }
        public String? PhoneNumber { get; set; }
        public String? PasswordSalt { get; set; }
        public String? PasswordDk { get; set; }
        public String? Avatar { get; set; }
        public string Role { get; set; } = "User";  // По умолчанию обычный пользователь
        public DateTime RegistrerDt { get; set; } = DateTime.Now;
        public DateTime? DeleteDt { get; set; }

        public ICollection<CommentMoviesEntity> CommentMovies { get; set; } = new List<CommentMoviesEntity>();

        public ICollection<FavoriteMovieEntity> FavoriteMovies { get; set; } = new List<FavoriteMovieEntity>();
        public ICollection<UserNotificationEntity> UserNotification { get; set; } = new List<UserNotificationEntity>();
        public ICollection<PausedMovieEntity> PausedMovies { get; set; } = new List<PausedMovieEntity>();
        public ICollection<PriorityCategoriesEntity> PriorityCategories { get; set; } = new List<PriorityCategoriesEntity>();
    }
}


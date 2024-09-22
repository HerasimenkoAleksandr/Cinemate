using Microsoft.AspNetCore.Mvc;

namespace cinemate.Models.Movie
{
    public class CommentMoviesModel
    {
        [FromForm(Name = "IdMovie")]
        public Guid IdMovie { get; set; }

        [FromForm(Name = "IdUsers")]
        public Guid? IdUsers { get; set; }

        [FromForm(Name = "Comment")]
        public String Comment { get; set; }  
    }
}

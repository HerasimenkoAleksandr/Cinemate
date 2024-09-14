using Microsoft.AspNetCore.Mvc;

namespace cinemate.Models.Movie
{
    public class ToggleLikeRequestModel
    {

        public String MovieId { get; set; }

        public bool IsLiked { get; set; }

        public bool IsDisliked { get; set; }
    }
}

using cinemate.Data.Entities;
using cinemate.Data;
using Newtonsoft.Json.Linq;

namespace cinemate.Services.YouTubeService
{
    public class AddFromYouTubeService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiKey = "AIzaSyAQ2JIQgMv0660U_lrAQt--OBvTrh6gvyU"; // Ваш API ключ
        private const string apiUrl = "https://www.googleapis.com/youtube/v3/videos";
        
      

        public static async Task<MoviesEntities> GetMovieFromYouTubeAsync(DataContext context, String videoId, Guid categoryId, Guid subCategoryId)
        {
            var url = $"{apiUrl}?part=snippet,contentDetails,statistics&id={videoId}&key={apiKey}";
            var response = await client.GetStringAsync(url);
            var data = JObject.Parse(response);

            var video = data["items"][0];

            var movie = new MoviesEntities
            {
                Title = video["snippet"]["title"].ToString(),
                Description = video["snippet"]["description"]?.ToString(),
                Picture = video["snippet"]["thumbnails"]["high"]["url"].ToString(),  // Используем URL миниатюры как изображение
                URL = $"https://www.youtube.com/embed/{videoId}",  // Встроенный проигрыватель
                ReleaseYear = DateTime.Parse(video["snippet"]["publishedAt"].ToString()).Year,
                Director = null,  // YouTube не предоставляет информацию о режиссере
                Actors = null,  // YouTube не предоставляет информацию об актерах
                likeCount = (int?)video["statistics"]["likeCount"],
                dislikeCount = (int?)video["statistics"]["dislikeCount"],
                CategoryId = categoryId,
                SubCategoryId = subCategoryId
            };
            if (movie != null)
            {
                context.MoviesEntities.Add(movie);
                await context.SaveChangesAsync();
            }
            return movie;
        }
    }
}

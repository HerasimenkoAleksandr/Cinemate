using cinemate.Data;
using Newtonsoft.Json.Linq;

namespace cinemate.Services.MovieDurationsService
{
    public class AddDurationService
    {
        private readonly DataContext _context;
        private readonly string _youtubeApiKey;
        private readonly HttpClient _httpClient;
        private const string apiKey = "AIzaSyAQ2JIQgMv0660U_lrAQt--OBvTrh6gvyU";
        public AddDurationService(DataContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
           // _youtubeApiKey = configuration["YouTubeApi"];
            _httpClient = httpClient;  
        }

        public async Task UpdateMovieDurationsAsync()
        {
            await UpdateAllMovieDurationsAsync(apiKey, _context);
        }

        public async Task UpdateAllMovieDurationsAsync(string apiKey, DataContext context)
        {
            var movies = context.MoviesEntities.ToList();

            foreach (var movie in movies)
            {
                string? videoId = ExtractYouTubeVideoId(movie.URL);
                if (videoId != null)
                {
                    string? duration = await GetVideoDurationFromYouTubeAsync(videoId, apiKey);
                    if (duration != null)
                    {
                        movie.Duration = duration;
                    }
                }
            }

            // Сохранение изменений в базе данных
            await context.SaveChangesAsync();
        }

        public async Task<string?> GetVideoDurationFromYouTubeAsync(string videoId, string apiKey)
        {
            string apiUrl = $"https://www.googleapis.com/youtube/v3/videos?id={videoId}&part=contentDetails&key={apiKey}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl); // Используем HttpClient из DI
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject videoData = JObject.Parse(jsonResponse);

                    if (videoData["items"] != null && videoData["items"].HasValues)
                    {
                        string durationISO = videoData["items"][0]["contentDetails"]["duration"]?.ToString();
                        return ConvertISO8601ToDuration(durationISO);
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка при получении продолжительности: {ex.Message}");
            }

            return null;
        }

        public string ConvertISO8601ToDuration(string isoDuration)
        {
            if (string.IsNullOrEmpty(isoDuration))
                return "Неизвестная продолжительность";

            var duration = System.Xml.XmlConvert.ToTimeSpan(isoDuration);

            int hours = duration.Hours;
            int minutes = duration.Minutes;

            // Если часов нет, не отображаем их
            if (hours > 0)
            {
                return $"{hours} h {minutes} m";
            }
            else
            {
                return $"{minutes} m";
            }
        }

        public string? ExtractYouTubeVideoId(string url)
        {
            try
            {
                var uri = new Uri(url);
                if (uri.AbsolutePath.StartsWith("/embed/"))
                {
                    return uri.Segments.Last();
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка при извлечении videoId: {ex.Message}");
            }

            return null;
        }
    }
}

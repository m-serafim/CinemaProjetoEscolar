using System.Text.Json;
using CinemaGestao2223226.Models.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CinemaGestao2223226.Services
{
    public class MovieApiService : IMovieApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _tmdbApiKey;
        private readonly string _tmdbBaseUrl;
        private readonly string _tmdbImageBaseUrl;
        private readonly ILogger<MovieApiService> _logger;

        // Genre mapping from English to Portuguese
        private static readonly Dictionary<string, string> GenreTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Action", "Ação" },
            { "Adventure", "Aventura" },
            { "Animation", "Animação" },
            { "Biography", "Biografia" },
            { "Comedy", "Comédia" },
            { "Crime", "Crime" },
            { "Documentary", "Documentário" },
            { "Drama", "Drama" },
            { "Family", "Família" },
            { "Fantasy", "Fantasia" },
            { "Film-Noir", "Film Noir" },
            { "History", "História" },
            { "Horror", "Terror" },
            { "Music", "Música" },
            { "Musical", "Musical" },
            { "Mystery", "Mistério" },
            { "Romance", "Romance" },
            { "Sci-Fi", "Ficção Científica" },
            { "Science Fiction", "Ficção Científica" },
            { "Short", "Curta" },
            { "Sport", "Desporto" },
            { "Thriller", "Thriller" },
            { "War", "Guerra" },
            { "Western", "Western" }
        };

        public MovieApiService(HttpClient httpClient, IConfiguration configuration, ILogger<MovieApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["OmdbApi:ApiKey"];
            _baseUrl = configuration["OmdbApi:BaseUrl"] ?? "http://www.omdbapi.com/";
            _tmdbApiKey = configuration["TmdbApi:ApiKey"];
            _tmdbBaseUrl = configuration["TmdbApi:BaseUrl"] ?? "https://api.themoviedb.org/3/";
            _tmdbImageBaseUrl = configuration["TmdbApi:ImageBaseUrl"] ?? "https://image.tmdb.org/t/p/";

            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OMDb API key not configured in appsettings.json");
            }
            if (string.IsNullOrEmpty(_tmdbApiKey))
            {
                _logger.LogWarning("TMDB API key not configured in appsettings.json");
            }
        }

        private string TranslateGenres(string genres)
        {
            if (string.IsNullOrEmpty(genres)) return genres;
            
            var genreList = genres.Split(new[] { ',', '/' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(g => g.Trim());
            
            var translatedGenres = genreList.Select(g => 
                GenreTranslations.TryGetValue(g, out var translated) ? translated : g);
            
            return string.Join(", ", translatedGenres);
        }

        public async Task<OmdbSearchResponse> SearchMoviesAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return new OmdbSearchResponse 
                { 
                    Response = "False", 
                    Error = "Search title cannot be empty" 
                };
            }

            if (string.IsNullOrEmpty(_apiKey))
            {
                return new OmdbSearchResponse 
                { 
                    Response = "False", 
                    Error = "API key not configured" 
                };
            }

            try
            {
                var url = $"{_baseUrl}?apikey={_apiKey}&s={Uri.EscapeDataString(title)}&type=movie";
                _logger.LogInformation("Searching movies with title: {Title}", title);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OmdbSearchResponse>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Convert poster URLs to high resolution
                if (result?.Search != null)
                {
                    foreach (var movie in result.Search)
                    {
                        if (!string.IsNullOrEmpty(movie.Poster))
                        {
                            movie.Poster = GetHighResolutionPosterUrl(movie.Poster);
                        }
                    }
                }

                return result ?? new OmdbSearchResponse 
                { 
                    Response = "False", 
                    Error = "Failed to parse response" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies for title: {Title}", title);
                return new OmdbSearchResponse 
                { 
                    Response = "False", 
                    Error = $"API request failed: {ex.Message}" 
                };
            }
        }

        public async Task<OmdbMovieDetails> GetMovieDetailsAsync(string imdbId)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return new OmdbMovieDetails 
                { 
                    Response = "False", 
                    Error = "IMDb ID cannot be empty" 
                };
            }

            if (string.IsNullOrEmpty(_apiKey))
            {
                return new OmdbMovieDetails 
                { 
                    Response = "False", 
                    Error = "API key not configured" 
                };
            }

            try
            {
                var url = $"{_baseUrl}?apikey={_apiKey}&i={imdbId}&plot=full";
                _logger.LogInformation("Fetching movie details for IMDb ID: {ImdbId}", imdbId);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OmdbMovieDetails>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Convert poster URL to high resolution
                if (result != null && !string.IsNullOrEmpty(result.Poster))
                {
                    result.Poster = GetHighResolutionPosterUrl(result.Poster);
                }

                return result ?? new OmdbMovieDetails 
                { 
                    Response = "False", 
                    Error = "Failed to parse response" 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie details for IMDb ID: {ImdbId}", imdbId);
                return new OmdbMovieDetails 
                { 
                    Response = "False", 
                    Error = $"API request failed: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Converts OMDB poster URLs from low resolution (SX300) to high resolution (SX1000).
        /// This provides approximately 3x better image quality for display.
        /// </summary>
        /// <param name="posterUrl">Original OMDB poster URL</param>
        /// <returns>High resolution poster URL</returns>
        private string GetHighResolutionPosterUrl(string posterUrl)
        {
            if (string.IsNullOrEmpty(posterUrl) || posterUrl == "N/A")
            {
                return posterUrl;
            }

            try
            {
                // OMDB returns URLs like: https://m.media-amazon.com/images/M/...._V1_SX300.jpg
                // We want to change SX300 to SX1000 for better quality
                // Common patterns: SX300, SY300, UX300, UY300
                posterUrl = System.Text.RegularExpressions.Regex.Replace(
                    posterUrl,
                    @"_(SX|SY|UX|UY)\d+",
                    "_SX1000",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );

                _logger.LogDebug("Converted poster URL to high resolution: {PosterUrl}", posterUrl);
                return posterUrl;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to convert poster URL to high resolution, using original: {PosterUrl}", posterUrl);
                return posterUrl;
            }
        }

        /// <summary>
        /// Gets movie details from TMDB API using IMDB ID
        /// </summary>
        public async Task<TmdbMovieDetails> GetTmdbMovieDetailsAsync(string imdbId)
        {
            if (string.IsNullOrEmpty(_tmdbApiKey))
            {
                _logger.LogWarning("TMDB API key not configured");
                return null;
            }

            try
            {
                // First find the movie by IMDB ID
                var findUrl = $"{_tmdbBaseUrl}find/{imdbId}?api_key={_tmdbApiKey}&external_source=imdb_id&language=pt-PT";
                _logger.LogInformation("Finding movie on TMDB with IMDB ID: {ImdbId}", imdbId);

                var findResponse = await _httpClient.GetAsync(findUrl);
                findResponse.EnsureSuccessStatusCode();

                var findJson = await findResponse.Content.ReadAsStringAsync();
                var findResult = JsonSerializer.Deserialize<TmdbFindResponse>(findJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (findResult?.Movie_Results == null || !findResult.Movie_Results.Any())
                {
                    _logger.LogWarning("Movie not found on TMDB for IMDB ID: {ImdbId}", imdbId);
                    return null;
                }

                var tmdbId = findResult.Movie_Results.First().Id;

                // Get full movie details with credits in Portuguese
                var detailsUrl = $"{_tmdbBaseUrl}movie/{tmdbId}?api_key={_tmdbApiKey}&language=pt-PT&append_to_response=credits";
                var detailsResponse = await _httpClient.GetAsync(detailsUrl);
                detailsResponse.EnsureSuccessStatusCode();

                var detailsJson = await detailsResponse.Content.ReadAsStringAsync();
                var movieDetails = JsonSerializer.Deserialize<TmdbMovieDetails>(detailsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return movieDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TMDB movie details for IMDB ID: {ImdbId}", imdbId);
                return null;
            }
        }

        /// <summary>
        /// Gets the backdrop/banner image URL from TMDB
        /// </summary>
        public async Task<string> GetMovieBackdropAsync(string imdbId)
        {
            try
            {
                var tmdbDetails = await GetTmdbMovieDetailsAsync(imdbId);
                if (tmdbDetails != null && !string.IsNullOrEmpty(tmdbDetails.Backdrop_Path))
                {
                    // Return high resolution backdrop (original or w1280 for good quality)
                    return $"{_tmdbImageBaseUrl}original{tmdbDetails.Backdrop_Path}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting backdrop for IMDB ID: {ImdbId}", imdbId);
            }
            return null;
        }
    }

    // Helper class for TMDB find response
    public class TmdbFindResponse
    {
        public List<TmdbMovieResult> Movie_Results { get; set; }
    }
}

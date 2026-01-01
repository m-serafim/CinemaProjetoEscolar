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
        private readonly ILogger<MovieApiService> _logger;

        public MovieApiService(HttpClient httpClient, IConfiguration configuration, ILogger<MovieApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["OmdbApi:ApiKey"];
            _baseUrl = configuration["OmdbApi:BaseUrl"] ?? "http://www.omdbapi.com/";

            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OMDb API key not configured in appsettings.json");
            }
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
    }
}

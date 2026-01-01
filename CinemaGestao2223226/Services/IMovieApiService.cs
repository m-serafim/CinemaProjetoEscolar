using CinemaGestao2223226.Models.DTOs;

namespace CinemaGestao2223226.Services
{
    public interface IMovieApiService
    {
        Task<OmdbSearchResponse> SearchMoviesAsync(string title);
        Task<OmdbMovieDetails> GetMovieDetailsAsync(string imdbId);
    }
}

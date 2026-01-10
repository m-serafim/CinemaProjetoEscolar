namespace CinemaGestao2223226.Models.DTOs
{
    // OMDb API search result item
    public class OmdbSearchResult
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string ImdbID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
    }

    // OMDb API search response wrapper
    public class OmdbSearchResponse
    {
        public List<OmdbSearchResult> Search { get; set; }
        public string TotalResults { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
    }

    // OMDb API detailed movie information
    public class OmdbMovieDetails
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public string ImdbRating { get; set; }
        public string ImdbID { get; set; }
        public string Type { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
    }

    // TMDB API Models for backdrop/banner images
    public class TmdbSearchResponse
    {
        public int Page { get; set; }
        public List<TmdbMovieResult> Results { get; set; }
        public int Total_Results { get; set; }
        public int Total_Pages { get; set; }
    }

    public class TmdbMovieResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Original_Title { get; set; }
        public string Overview { get; set; }
        public string Poster_Path { get; set; }
        public string Backdrop_Path { get; set; }
        public string Release_Date { get; set; }
        public double Vote_Average { get; set; }
    }

    public class TmdbMovieDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Original_Title { get; set; }
        public string Overview { get; set; }
        public string Poster_Path { get; set; }
        public string Backdrop_Path { get; set; }
        public string Release_Date { get; set; }
        public int Runtime { get; set; }
        public string Imdb_Id { get; set; }
        public List<TmdbGenre> Genres { get; set; }
        public TmdbCredits Credits { get; set; }
    }

    public class TmdbGenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TmdbCredits
    {
        public List<TmdbCast> Cast { get; set; }
        public List<TmdbCrew> Crew { get; set; }
    }

    public class TmdbCast
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Character { get; set; }
        public int Order { get; set; }
    }

    public class TmdbCrew
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string Department { get; set; }
    }
}

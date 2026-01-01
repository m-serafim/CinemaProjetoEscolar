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
}

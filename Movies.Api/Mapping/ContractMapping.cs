using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapRequestToMovie(this CreateMovieRequest request) 
    {
        var movie = new Movie()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
        return movie;
    }

    public static Movie MapRequestToMovie(this UpdateMovieRequest request, Guid id)
    {
        var movie = new Movie()
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
        return movie;
    }

    public static MovieResponse MapMovieToResponse(this Movie movie)
    {
        var response = new MovieResponse()
        {
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres
        };
        return response;
    }

    public static MoviesReponse MapMoviesToMoviesResponse(this IEnumerable<Movie> movies) 
    {
        return new MoviesReponse()
        {
            Items = movies.Select(x => x.MapMovieToResponse())
        };
    }
}

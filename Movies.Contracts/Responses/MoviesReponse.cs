namespace Movies.Contracts.Responses;

public class MoviesReponse
{
    public required IEnumerable<MovieResponse> Items { get; init; } = Enumerable.Empty<MovieResponse>();
}

 
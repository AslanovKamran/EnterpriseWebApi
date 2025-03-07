using Movies.Application.Models;
using Movies.Application.Repositories.Abstract;

namespace Movies.Application.Repositories.Concrete;

public class MovieRepository : IMovieRepository
{
    //Fake DB for now
    private readonly List<Movie> _movies = new();

    #region Get
    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _movies.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(movie);
    }

    #endregion

    #region Create
    public Task<bool> CreateMovieAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    #endregion

    #region Update
    public Task<bool> UpdateAsync(Movie movie)
    {
        var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
        if (movieIndex == -1)
        {
            return Task.FromResult(false);
        }
        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }

    #endregion

    #region Delete
    public Task<bool> DeleteAsync(Guid id)
    {
        var removedCount = _movies.RemoveAll(x => x.Id == id);
        return Task.FromResult(removedCount > 0);
    }

    #endregion

}

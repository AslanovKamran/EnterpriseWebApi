using Movies.Application.Models;

namespace Movies.Application.Repositories.Abstract;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(Guid id);
    Task<bool> CreateMovieAsync(Movie movie);
    Task<bool> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid id);
}

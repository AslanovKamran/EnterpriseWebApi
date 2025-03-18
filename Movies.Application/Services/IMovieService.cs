using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(Guid id);
    Task<bool> CreateMovieAsync(Movie movie);
    Task<Movie?> UpdateAsync(Movie movie);
    Task<bool> DeleteByIdAsync(Guid id);
}

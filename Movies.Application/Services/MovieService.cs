using Movies.Application.Models;
using Movies.Application.Repositories.Abstract;
using System.Security.AccessControl;

namespace Movies.Application.Services; 

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository) => _movieRepository = movieRepository;

    public async Task<bool> CreateMovieAsync(Movie movie)
    {
        return await _movieRepository.CreateMovieAsync(movie);
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        return await _movieRepository.DeleteByIdAsync(id);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _movieRepository.GetAllAsync();
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        return await _movieRepository.GetByIdAsync(id);
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        //Business logic
        var exists = await _movieRepository.ExistsByIdAsync(movie.Id);
        
        if (!exists) 
            return null;

        await _movieRepository.UpdateAsync(movie);
        return movie;
    }
}

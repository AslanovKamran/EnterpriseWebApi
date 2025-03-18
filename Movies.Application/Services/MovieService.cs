using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories.Abstract;
using System.Security.AccessControl;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
    {
        _movieRepository = movieRepository;
        _movieValidator = movieValidator;
    }

    public async Task<bool> CreateMovieAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        return await _movieRepository.CreateMovieAsync(movie, cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _movieRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _movieRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        //Business logic
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        var exists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);

        if (!exists)
            return null;

        await _movieRepository.UpdateAsync(movie, cancellationToken);
        return movie;
    }
}

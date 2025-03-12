using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;
using Movies.Application.Repositories.Abstract;

namespace Movies.Application.Repositories.Concrete;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    => _dbConnectionFactory = dbConnectionFactory;

    #region Get
    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        const string query = @"
            SELECT 
                m.Id,
                m.Title,
                m.YearOfRelease,
                STRING_AGG(g.Name, ',') AS Genres
            FROM Movies m
            LEFT JOIN Genres g ON m.Id = g.MovieId
            GROUP BY m.Id, m.Title, m.YearOfRelease;
        ";

        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var moviesDto = await connection.QueryAsync(query);

        return moviesDto.Select(row => new Movie
        {
            Id = (Guid)row.Id,
            Title = (string)row.Title,
            YearOfRelease = (int)row.YearOfRelease,
            Genres = row.Genres != null ? ((string)row.Genres).Split(',').ToList() : new List<string>()
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        const string movieQuery = "SELECT * FROM Movies WHERE Id = @id";
        const string genresQuery = "SELECT Name FROM Genres WHERE MovieId = @id";

        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>
        (
            new CommandDefinition(movieQuery, new { id })
        );

        if (movie is null)
            return null;


        var relatedGenres = await connection.QueryAsync<string>(genresQuery, new { id });
        if (relatedGenres is not null)
        {
            foreach (var genre in relatedGenres)
            {
                movie.Genres.Add(genre);
            }
        }

        return movie;
    }

    #endregion

    #region Create
    public async Task<bool> CreateMovieAsync(Movie movie)
    {

        const string movieQuery = "INSERT INTO Movies (Id, Title, YearOfRelease) VALUES (@Id, @Title, @YearOfRelease)";
        const string genreQuery = "INSERT INTO Genres (MovieId, Name) VALUES (@MovieId, @Name)";


        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var result = await connection.ExecuteAsync(movieQuery, movie, transaction);

            if (result > 0 && movie.Genres.Any())
            {
                var genreParams = movie.Genres.Select(genre => new { MovieId = movie.Id, Name = genre });
                await connection.ExecuteAsync(
                    new CommandDefinition(genreQuery, genreParams, transaction)
                );
            }

            transaction.Commit();
            return result > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    #endregion

    #region Update
    public async Task<bool> UpdateAsync(Movie movie)
    {
        var queryUpdateMovie = @"
        UPDATE Movies 
        SET Title = @Title, YearOfRelease = @YearOfRelease
        WHERE Id = @Id";

        var queryDeleteGenres = @"
        DELETE FROM Genres 
        WHERE MovieId = @MovieId";

        var queryInsertGenre = @"
        INSERT INTO Genres (MovieId, Name) 
        VALUES (@MovieId, @Name)";

        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            //1Update a movie
            // 1. Update the movie
            int affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(queryUpdateMovie, movie, transaction)
            );

            if (affectedRows == 0)
            {
                transaction.Rollback();
                return false; // Movie not found or nothing changed
            }

            // 2. Delete existing genres
            await connection.ExecuteAsync(
                new CommandDefinition(queryDeleteGenres, new { MovieId = movie.Id }, transaction)
            );

            // 3. Insert new genres
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(
                    new CommandDefinition(queryInsertGenre, new { MovieId = movie.Id, Name = genre }, transaction)
                );
            }

            // 4. Commit transaction
            transaction.Commit();
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    #endregion

    #region Delete
    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var queryDeleteGenres = @"DELETE FROM Genres WHERE MovieId = @MovieId";
        var queryDeleteMovie = @"DELETE FROM Movies WHERE Id = @Id";

        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Delete related genres
            await connection.ExecuteAsync(
                new CommandDefinition(queryDeleteGenres, new { MovieId = id }, transaction)
            );

            // 2. Delete the movie
            int affectedRows = await connection.ExecuteAsync(
                new CommandDefinition(queryDeleteMovie, new { Id = id }, transaction)
            );

            if (affectedRows == 0)
            {
                transaction.Rollback();
                return false; // Movie not found
            }

            // 3. Commit transaction if everything succeeded
            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    #endregion

    #region Exists ?

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var query = @"SELECT COUNT(*) FROM Movies WHERE Id = @id";
        var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var count = await connection.ExecuteScalarAsync<int>(query, new { id });
        return count > 0;
    }

    #endregion

}

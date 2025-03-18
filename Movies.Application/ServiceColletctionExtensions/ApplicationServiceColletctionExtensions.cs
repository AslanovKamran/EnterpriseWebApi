using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories.Abstract;
using Movies.Application.Repositories.Concrete;
using Movies.Application.Services;

namespace Movies.Application.ServiceColletctionExtensions;

public static class ApplicationServiceColletctionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services) 
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>();
        return services;
    }
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory, MsSqlConnectionFactory>(provider => new MsSqlConnectionFactory(connectionString));
        return services;
    }
}

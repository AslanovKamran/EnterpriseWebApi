using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Repositories.Abstract;
using Movies.Application.Repositories.Concrete;

namespace Movies.Application.ServiceColletctionExtensions;

public static class ApplicationServiceColletctionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services) 
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        return services;
    }
}

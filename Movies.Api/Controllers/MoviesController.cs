using Microsoft.AspNetCore.Mvc;
using Movies.Api.EndPoints;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories.Abstract;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }
   




    #region Get

    [HttpGet]
    [Route(ApiEndPoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieService.GetAllAsync();
        return Ok(movies.MapMoviesToMoviesResponse());
    }

    [HttpGet]
    [Route(ApiEndPoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        if (movie is null) return NotFound();
        return Ok(movie.MapMovieToResponse());
    }

    #endregion

    #region Create

    [HttpPost]
    [Route(ApiEndPoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        //Accept Contract
        var movie = request.MapRequestToMovie();

        var result = await _movieService.CreateMovieAsync(movie);

        //Return contract
        var movieResponse = movie.MapMovieToResponse();

        return result
            ? CreatedAtAction(actionName: nameof(Get), routeValues: new { id = movieResponse.Id }, value: movieResponse)
            : BadRequest();
    }

    #endregion

    #region Update

    [HttpPut]
    [Route(ApiEndPoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapRequestToMovie(id);
        var result = await _movieService.UpdateAsync(movie);
        if (result is null) return NotFound();
        var response = movie.MapMovieToResponse();
        return Ok(response);
    }

    #endregion

    #region Delete

    [HttpDelete]
    [Route(ApiEndPoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _movieService.DeleteByIdAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    #endregion
}

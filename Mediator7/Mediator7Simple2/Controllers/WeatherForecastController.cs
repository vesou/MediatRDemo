using Mediator7Simple2.Queries;
using Mediator7Simple2.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mediator7Simple2.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetViaManager()
    {
        _logger.LogInformation("GetWeatherForecast called");

        var response = await _mediator.Send(new GetWeatherForecast.Query());

        return response.WeatherForecasts;
    }
}
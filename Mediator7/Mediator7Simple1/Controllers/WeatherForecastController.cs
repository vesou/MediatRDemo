using Mediator7Simple1.Queries;
using Mediator7Simple1.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mediator7Simple1.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
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

        var response = await _mediator.Send(new GetWeatherForecastQuery());

        return response.WeatherForecasts;
    }
}
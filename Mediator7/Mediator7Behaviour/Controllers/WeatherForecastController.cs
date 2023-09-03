using Mediator7Behaviour.Queries;
using Mediator7Behaviour.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mediator7Behaviour.Controllers;

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
    
    [HttpPost("hello", Name = "GetHello")]
    public async Task<string> GetHello([FromBody]HelloMessageDto helloMessageDto)
    {
        _logger.LogInformation("GetHello called");

        var response = await _mediator.Send(new SayHello.Query(helloMessageDto));

        return response.ResponseMessage;
    }
}
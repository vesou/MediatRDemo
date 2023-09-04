using Mediator7Hangfire.Hangfire;
using Mediator7Hangfire.Queries;
using Mediator7Hangfire.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mediator7Hangfire.Controllers;

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
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast()
    {
        _logger.LogInformation("GetWeatherForecast called");

        var response = await _mediator.Send(new GetWeatherForecast.Query());

        return response.WeatherForecasts;
    }
    
    [HttpPost("hello", Name = "SayHello")]
    public async Task<string> GetHello([FromBody]HelloMessageDto helloMessageDto)
    {
        _logger.LogInformation("SayHello called");

        var response = await _mediator.Send(new SayHello.Query(helloMessageDto));

        return response.ResponseMessage;
    }
    
    [HttpGet("slow", Name = "SlowQuery")]
    public async Task<string> GetSlowQuery()
    {
        _logger.LogInformation("SlowQuery called");

        await _mediator.Send(new Slow.Query());

        return "return from slow query";
    }
    
    [HttpGet("slowButFast", Name = "SlowButFastQuery")]
    public string GetSlowButFastQuery()
    {
        _logger.LogInformation("SlowButFastQuery called");

        _mediator.Enqueue("SlowButFast", new Slow.Query(), "default");

        return "return from slow query";
    }
}
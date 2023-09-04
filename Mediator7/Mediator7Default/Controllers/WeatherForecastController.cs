using Mediator7Default.Managers;
using Mediator7Default.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Mediator7Default.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherForecastManager _weatherForecastManager;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastManager weatherForecastManager)
    {
        _logger = logger;
        _weatherForecastManager = weatherForecastManager;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("GetWeatherForecast called");

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
    [HttpGet("getViaManager", Name = "GetWeatherForecastViaManager")]
    public async Task<IEnumerable<WeatherForecast>> GetViaManager()
    {
        _logger.LogInformation("GetWeatherForecast via manager called");

        return await _weatherForecastManager.GetWeatherForecastAsync();
    }
}
using Mediator7Default.ViewModels;

namespace Mediator7Default.Managers;

public class WeatherForecastManager : IWeatherForecastManager
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        return await Task.FromResult<IEnumerable<WeatherForecast>>(Enumerable.Range(1, 5)
            .Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray());
    }
}
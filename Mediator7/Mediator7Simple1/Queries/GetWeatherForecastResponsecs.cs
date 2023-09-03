using Mediator7Simple1.ViewModels;

namespace Mediator7Simple1.Queries;

public class GetWeatherForecastResponse
{
    public GetWeatherForecastResponse(IEnumerable<WeatherForecast> weatherForecasts)
    {
        WeatherForecasts = weatherForecasts;
    }

    public IEnumerable<WeatherForecast> WeatherForecasts { get; set; }
}
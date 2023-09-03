using Mediator7.ViewModels;

namespace Mediator7.Managers;

public interface IWeatherForecastManager
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
}
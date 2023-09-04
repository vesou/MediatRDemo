using Mediator7Default.ViewModels;

namespace Mediator7Default.Managers;

public interface IWeatherForecastManager
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
}
using Mediator7Simple1.ViewModels;
using MediatR;

namespace Mediator7Simple1.Queries;

public class GetWeatherForecastHandler : IRequestHandler<GetWeatherForecastQuery, GetWeatherForecastResponse>
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<GetWeatherForecastHandler> _logger;

    public GetWeatherForecastHandler(ILogger<GetWeatherForecastHandler> logger)
    {
        _logger = logger;
    }

    public async Task<GetWeatherForecastResponse> Handle(GetWeatherForecastQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetWeatherForecastHandler called");
        var weatherForecasts = await Task.FromResult<IEnumerable<WeatherForecast>>(Enumerable.Range(1, 5)
            .Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray());

        return new GetWeatherForecastResponse(weatherForecasts);
    }
}
using Mediator7Behaviour.Interfaces;
using Mediator7Behaviour.ViewModels;
using MediatR;

namespace Mediator7Behaviour.Queries;

public static class GetWeatherForecast
{
    public class Query : IRequest<Response>, IRetryable
    {
        public int MaxRetryCount { get; } = 3;
    }

    public class Handler : IRequestHandler<Query, Response>
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<Handler> _logger;

        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }

        public async Task<Response> Handle(Query request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetWeatherForecastHandler called");
            var random = Random.Shared.Next(1, 5);
            if (random == 1)
                throw new Exception("Random exception thrown");

            var weatherForecasts = await Task.FromResult<IEnumerable<WeatherForecast>>(Enumerable.Range(1, 5)
                .Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    })
                .ToArray());

            return new Response(weatherForecasts);
        }
    }

    public class Response
    {
        public Response(IEnumerable<WeatherForecast> weatherForecasts)
        {
            WeatherForecasts = weatherForecasts;
        }

        public IEnumerable<WeatherForecast> WeatherForecasts { get; set; }
    }
}
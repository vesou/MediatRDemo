using Mediator7Hangfire.Interfaces;
using MediatR;

namespace Mediator7Hangfire.Behaviours;

public class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ILoggable
{
    private readonly ILogger _logger;

    public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestLog = request.ToLogMessage();
        _logger.LogInformation("Request info: {@Request}", requestLog);

        var response = await next();
        if(response is ILoggable resp)
        {
            var responseLog = resp.ToLogMessage();
            _logger.LogInformation("Response info: {@Request}", responseLog);
        }

        return response;
    }
}
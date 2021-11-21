using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mediator.Behaviours
{
    public class LoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ILoggable req)
            {
                var logInfo = req.ToLogMessage();
                _logger.LogInformation("Request info: {@Request}", logInfo.Data);
            }

            var response = await next();
            if (response is ILoggable res)
            {
                var logInfo = res.ToLogMessage();
                _logger.LogInformation("Response info: {@Request}", logInfo.Data);
            }

            return response;
        }
    }
}
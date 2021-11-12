using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OneLink.Shared.Const;
using OneLink.Shared.Enums;
using OneLink.Shared.Helpers.Logging;
using OneLink.Shared.Interfaces;

namespace OneLink.Shared.MediatR
{
    public class LoggingPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public LoggingPipeline(ILogger<LoggingPipeline<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ILoggable req)
            {
                _logger.LogInfo(EventTypeEnum.RequestLog, request.GetType().FullName, new Dictionary<string, object>
                {
                    {LoggingPropertyNames.Request, req.ToLogMessage()}
                });
            }

            var response = await next();
            if (response is ILoggable res)
            {
                _logger.LogInfo(EventTypeEnum.ResponseLog, response.GetType().FullName, new Dictionary<string, object>
                {
                    {LoggingPropertyNames.Response, res.ToLogMessage()}
                });
            }

            return response;
        }
    }
}
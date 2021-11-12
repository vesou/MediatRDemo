using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OneLink.Shared.Const;
using OneLink.Shared.Enums;
using OneLink.Shared.Helpers.Logging;

namespace OneLink.Shared.MediatR
{
    public class TimerPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public TimerPipeline(ILogger<TimerPipeline<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var response = await next();
            sw.Stop();
            _logger.LogInfo(EventTypeEnum.CallEnded, request.GetType().FullName, new Dictionary<string, object>
            {
                {LoggingPropertyNames.CallTime, sw.ElapsedMilliseconds}
            });

            return response;
        }
    }
}
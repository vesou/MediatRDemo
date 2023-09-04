using Mediator7Hangfire.Interfaces;
using Mediator7Hangfire.ViewModels;
using MediatR;

namespace Mediator7Hangfire.Queries;

public static class SayHello
{
    public class Query : IRequest<Response>, ILoggable
    {
        public readonly HelloMessageDto HelloMessageDto;

        public Query(HelloMessageDto helloMessageDto)
        {
            HelloMessageDto = helloMessageDto;
        }

        public string ToLogMessage()
        {
            return $"Message: {HelloMessageDto.Message}, Date: {HelloMessageDto.Date}";
        }
    }

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly ILogger<Handler> _logger;

        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }

        public Task<Response> Handle(Query request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("SayHelloHandler called");
            return Task.FromResult(new Response(request.HelloMessageDto.Message));
        }
    }

    public class Response : ILoggable
    {
        public Response(string message)
        {
            ResponseMessage = message + " - Response";
        }

        public string ResponseMessage { get; set; }
        public string ToLogMessage()
        {
            return ResponseMessage;
        }
    }
}
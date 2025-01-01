using MediatR;
namespace Account.API.Application.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest
    : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("----- Handling command {CommandName} ({@Command})", typeof(TResponse).Name, request);

        TResponse response = await next();

        _logger.LogInformation("----- Command {CommandName} handled - response: {@Response}", typeof(TResponse).Name, response);

        return response;
    }
}
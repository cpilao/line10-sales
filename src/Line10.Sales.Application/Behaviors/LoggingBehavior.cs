using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Line10.Sales.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : BaseResponse
{
    private readonly ILogger _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            var stopWatch = Stopwatch.StartNew();
            _logger.LogInformation("Application request {@Request}", request);

            var response = await next();
            stopWatch.Stop();

            if (response.IsSuccess)
            {
                _logger.LogInformation("Application response {@Response} in {TotalMs}ms successfully",
                    response,
                    stopWatch.Elapsed.TotalMilliseconds);
            }
            else
            {
                _logger.LogInformation("Application response {@Response} in {TotalMs}ms with errors : {@Errors}",
                    response,
                    stopWatch.Elapsed.TotalMilliseconds,
                    response.Errors);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on processing request {@ActionName}", request.GetType().Name);
            throw;
        }
    }
}
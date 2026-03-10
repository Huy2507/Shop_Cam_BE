using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Shop_Cam_BE.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Shop_Cam_BE Request: {Name} {@Request}", requestName, request);
        return Task.CompletedTask;
    }
}

using System.Text.Json;
using Microsoft.Extensions.Hosting;

public class FeedbackListenerService : BackgroundService
{
    private readonly ServiceBusService _serviceBusService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FeedbackListenerService> _logger;

    public FeedbackListenerService(ServiceBusService serviceBusService, IConfiguration configuration, ILogger<FeedbackListenerService> logger)
    {
        _serviceBusService = serviceBusService;
        _configuration = configuration;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting feedback listener service...");

        var queueName = _configuration["ServiceBus:FeedbackQueueName"];
        _serviceBusService.StartFeedbackListener(queueName);

        return Task.CompletedTask;
    }
}

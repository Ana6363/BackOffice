using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;

public class ServiceBusService
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<ServiceBusService> _logger;
    private readonly IConfiguration _configuration;
    
private TaskCompletionSource<FeedbackMessage> _feedbackTaskCompletionSource;

    public ServiceBusService(ServiceBusClient serviceBusClient, ILogger<ServiceBusService> logger, IConfiguration configuration)
    {
        _serviceBusClient = serviceBusClient;
        _logger = logger;
        _configuration = configuration;
    }


    public void InitializeFeedbackTask()
    {
        _feedbackTaskCompletionSource = new TaskCompletionSource<FeedbackMessage>();
    }

    public Task<FeedbackMessage> WaitForFeedbackAsync()
    {
        return _feedbackTaskCompletionSource.Task;
    }
    public async Task SendMessageAsync(string queueName, object messageBody)
{
    try
    {
        var sender = _serviceBusClient.CreateSender(queueName); // Specify queue name
        var message = new ServiceBusMessage(JsonSerializer.Serialize(messageBody));

        await sender.SendMessageAsync(message);
        await sender.DisposeAsync();

        _logger.LogInformation("Message sent to queue '{QueueName}': {MessageBody}", queueName, messageBody);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error sending message to queue '{QueueName}'", queueName);
        throw;
    }
}

public void StartFeedbackListener(string queueName)
{
    var processor = _serviceBusClient.CreateProcessor(queueName);

    processor.ProcessMessageAsync += async args =>
    {
        try
        {
            _logger.LogInformation("Received feedback message: {MessageBody}", args.Message.Body.ToString());

            // Deserialize feedback message
            var feedback = JsonSerializer.Deserialize<FeedbackMessage>(args.Message.Body.ToString(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (feedback == null)
            {
                _logger.LogError("Deserialized feedback message is null.");
                return;
            }

            _logger.LogInformation("Processed feedback: StatusCode={StatusCode}, Message={Message}",
                feedback.StatusCode, feedback.Message);

            // Set the feedback in TaskCompletionSource and signal waiting code
            _feedbackTaskCompletionSource?.TrySetResult(feedback);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing feedback message.");
            await args.DeadLetterMessageAsync(args.Message, "ProcessingError", ex.Message);
        }
    };

    processor.ProcessErrorAsync += args =>
    {
        _logger.LogError(args.Exception, "Error in feedback listener.");
        return Task.CompletedTask;
    };

    processor.StartProcessingAsync();
    _logger.LogInformation("Feedback listener started for queue '{QueueName}'", queueName);
}
}

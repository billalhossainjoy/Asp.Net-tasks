namespace Asp.Net_task3.Services.Email;

public sealed class EmailBackgroundService
    : BackgroundService
{
    private readonly IEmailQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(
        IEmailQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<EmailBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message =
                await _queue.DequeueAsync(stoppingToken);

            try
            {
                await using var scope =
                    _scopeFactory.CreateAsyncScope();

                var emailSender =
                    scope.ServiceProvider
                        .GetRequiredService<IEmailSender>();

                await emailSender.SendVerificationMailAsync(
                    message,
                    stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unable to send confirmation email to {Email}",
                    message.To);
            }
        }
    }
}
public interface IEmailQueue
{
    ValueTask QueueAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);

    ValueTask<EmailMessage> DequeueAsync(
        CancellationToken cancellationToken);
}
using System.Threading.Channels;

namespace Asp.Net_task3.Services.Email;

public sealed class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _queue =
        Channel.CreateUnbounded<EmailMessage>();

    public ValueTask QueueAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        return _queue.Writer.WriteAsync(
            message,
            cancellationToken);
    }

    public ValueTask<EmailMessage> DequeueAsync(
        CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAsync(
            cancellationToken);
    }
}
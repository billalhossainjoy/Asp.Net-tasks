public interface IEmailSender
{
    Task SendVerificationMailAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}

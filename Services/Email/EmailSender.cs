using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Asp.Net_tasks.Services.Email;

public sealed class EmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    public EmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendVerificationMailAsync(EmailMessage message,
            CancellationToken cancellationToken
        )
    {
        var mail = new MimeMessage();

        mail.From.Add(
            new MailboxAddress(
                _options.FromName,
                _options.FromEmail));

        mail.To.Add(
            MailboxAddress.Parse(message.To)
        );

        mail.Subject = message.Subject;

        mail.Body = new TextPart("html")
        {
            Text = message.HtmlBody
        };


        using var client = new SmtpClient();


        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await client.AuthenticateAsync(
            _options.Username,
            _options.Password,
            cancellationToken);

        await client.SendAsync(
            mail,
            cancellationToken);

        await client.DisconnectAsync(
            true,
            cancellationToken);
    }
}
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Asp.Net_tasks.Services.Email;

public sealed class EmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly SmtpOptions _options;

    public EmailSender(
        HttpClient httpClient,
        IOptions<SmtpOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendVerificationMailAsync(
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        var from = string.IsNullOrWhiteSpace(_options.FromName)
            ? _options.FromEmail
            : $"{_options.FromName} <{_options.FromEmail}>";

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
        {
            Content = JsonContent.Create(new
            {
                from,
                to = new[] { message.To },
                subject = message.Subject,
                html = message.HtmlBody
            })
        };

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.Password);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Resend API returned {(int)response.StatusCode}: {error}");
        }
    }
}

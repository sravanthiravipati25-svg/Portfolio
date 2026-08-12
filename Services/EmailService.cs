using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Portfolio.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> settings,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }
    public async Task SendAsync(string name, string email, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Portfolio Website", _settings.From));
        emailMessage.To.Add(MailboxAddress.Parse(_settings.To));
        emailMessage.ReplyTo.Add(MailboxAddress.Parse(email));

        emailMessage.Subject = $"New Portfolio Contact from {name}";

        var safeMessage = System.Net.WebUtility.HtmlEncode(message)
            .Replace(Environment.NewLine, "<br/>");

        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
            <h2>New Portfolio Contact</h2>
            <p><strong>Name:</strong> {System.Net.WebUtility.HtmlEncode(name)}</p>
            <p><strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(email)}</p>
            <hr/>
            <p><strong>Message:</strong></p>
            <p>{safeMessage}</p>
        "
        };

        using var smtp = new SmtpClient();

        // 15 second timeout
        smtp.Timeout = 15000;

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _settings.UserName,
            _settings.Password);

        await smtp.SendAsync(emailMessage);

        await smtp.DisconnectAsync(true);
    }
    /*public async Task SendAsync(string name, string email, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Portfolio Website", _settings.From));
        emailMessage.To.Add(MailboxAddress.Parse(_settings.To));
        emailMessage.ReplyTo.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = $"New Portfolio Contact from {name}";

        var safeMessage = System.Net.WebUtility.HtmlEncode(message)
            .Replace(Environment.NewLine, "<br />")
            .Replace("    ", " < br /> ");
    

        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
            <h2>New Portfolio Contact</h2>
            <p><strong>Name:</strong> {System.Net.WebUtility.HtmlEncode(name)}</p>
            <p><strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(email)}</p>
            <hr/>
            <p><strong>Message:</strong></p>
            <p>{safeMessage}</p>
        "
        };

        try
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.UserName,
                _settings.Password);

            await smtp.SendAsync(emailMessage);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Portfolio email sent successfully from {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send portfolio email from {Email}", email);
            throw;
        }
    }*/
}
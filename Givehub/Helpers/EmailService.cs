using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var settings = _configuration.GetSection("EmailSettings");

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(settings["FromName"], settings["FromEmail"]));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = body
        };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            settings["Host"],
            int.Parse(settings["Port"]),
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            settings["Username"],
            settings["Password"]
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}

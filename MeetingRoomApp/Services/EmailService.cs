using System.Net;
using System.Net.Mail;
using MeetingRoomApp.Interfaces;

namespace MeetingRoomApp.Services;


public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                    "mustafaatayikilmaz@gmail.com",
                    Environment.GetEnvironmentVariable("GOOGLE_MAIL_PASSWORD")),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("mustafaatayikilmaz@gmail.com", "Meeting Room App"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(new MailAddress(to));

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending email to {to}");
            throw;
        }
    }
}
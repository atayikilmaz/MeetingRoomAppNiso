using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;
using MeetingRoomApp.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MeetingRoomApp.Services
{
    public class SendReminderEmailsService
    {
        private readonly ILogger<SendReminderEmailsService> _logger;

        public SendReminderEmailsService(ILogger<SendReminderEmailsService> logger)
        {
            _logger = logger;
        }

        public async Task SendReminderEmailsAsync(Meeting meeting)
        {
            _logger.LogInformation($"Sending reminder emails for meeting: {meeting.Name}");
            _logger.LogInformation($"Meeting start time: {meeting.StartDateTime}");
            _logger.LogInformation($"Meeting participants: {string.Join(", ", meeting.MeetingParticipants.Select(p => p.Email))}");

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("mustafaatayikilmaz@gmail.com", Environment.GetEnvironmentVariable("GOOGLE_MAIL_PASSWORD")),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("mustafaatayikilmaz@gmail.com", "Meeting Room App"),
                    Subject = $"Reminder: Meeting '{meeting.Name}' starts in 15 minutes",
                    Body = $"Dear Participant,<br><br>This is a reminder that the meeting '{meeting.Name}' " +
                           $"is scheduled to start at {meeting.StartDateTime:yyyy-MM-dd HH:mm:ss} UTC " +
                           $"({meeting.StartDateTime.AddHours(3):yyyy-MM-dd HH:mm:ss} Turkish time).<br><br>" +
                           $"Please be prepared to join the meeting.<br><br>Best regards,<br>Meeting Room App",
                    IsBodyHtml = true,
                };

                if (meeting.MeetingParticipants != null && meeting.MeetingParticipants.Any())
                {
                    foreach (var participant in meeting.MeetingParticipants)
                    {
                        mailMessage.To.Add(new MailAddress(participant.Email, participant.User.Name));
                    }

                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Reminder emails sent for meeting '{meeting.Name}'");
                }
                else
                {
                    _logger.LogWarning($"No participants found for meeting '{meeting.Name}'");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending reminder emails for meeting '{meeting.Name}'");
            }
        }
    }
}
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
    if (meeting == null)
    {
        _logger.LogError("Meeting object is null");
        return;
    }

    _logger.LogInformation(meeting.MeetingRoom.Name);
    _logger.LogInformation($"Sending reminder emails for meeting: {meeting.Name ?? "Unnamed meeting"}");
    _logger.LogInformation($"Meeting start time: {meeting.StartDateTime}");
    _logger.LogInformation($"Meeting participants: {string.Join(", ", meeting.MeetingParticipants?.Select(p => p.Email) ?? Enumerable.Empty<string>())}");

    try
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("mustafaatayikilmaz@gmail.com", Environment.GetEnvironmentVariable("GOOGLE_MAIL_PASSWORD")),
            EnableSsl = true,
        };

        var turkishTime = meeting.StartDateTime.AddHours(3);
        var mailMessage = new MailMessage
        {
            From = new MailAddress("mustafaatayikilmaz@gmail.com", "Meeting Room App"),
            Subject = $"Reminder: {meeting.Name ?? "Unnamed meeting"} Meeting Starts in 15 Minutes",
            Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #f0f0f0; padding: 20px; text-align: center;'>
                        <h1 style='color: #4a4a4a;'>Meeting Reminder</h1>
                    </div>
                    <div style='padding: 20px;'>
                        <p>Dear Participant,</p>
                        <p>This email is a reminder for your upcoming meeting.</p>
                        <div style='background-color: #e9f7fe; border-left: 4px solid #4a90e2; padding: 15px; margin: 20px 0;'>
                            <h2 style='color: #4a90e2; margin-top: 0;'>{meeting.Name ?? "Unnamed meeting"}</h2>
                            <p><strong>Date and Time:</strong> {turkishTime:dd MMMM yyyy, dddd, HH:mm} (Turkish Time)</p>
                            <p><strong>Meeting Room:</strong> {meeting.MeetingRoom?.Name ?? "Unspecified room"}</p>
                        </div>
                        <p>Please be prepared to join the meeting on time.</p>
                        <p>Best regards,<br>Meeting Room App Team</p>
                    </div>
                    <div style='background-color: #f0f0f0; padding: 10px; text-align: center; font-size: 12px;'>
                        <p>This is an automated notification. Please do not reply to this email.</p>
                    </div>
                </body>
                </html>
            ",
            IsBodyHtml = true,
        };

        if (meeting.MeetingParticipants != null && meeting.MeetingParticipants.Any())
        {
            foreach (var participant in meeting.MeetingParticipants)
            {
                if (participant?.Email != null && participant?.User?.Name != null)
                {
                    mailMessage.To.Add(new MailAddress(participant.Email, participant.User.Name));
                }
                else
                {
                    _logger.LogWarning($"Invalid participant data for meeting '{meeting.Name}'");
                }
            }

            if (mailMessage.To.Any())
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Reminder emails sent for meeting '{meeting.Name}'");
            }
            else
            {
                _logger.LogWarning($"No valid participants found for meeting '{meeting.Name}'");
            }
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
}    }
}
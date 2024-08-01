using MeetingRoomApp.Interfaces;
using MeetingRoomApp.Models;

public class SendReminderEmailsService
{
    private readonly ILogger<SendReminderEmailsService> _logger;
    private readonly IEmailService _emailService;

    public SendReminderEmailsService(ILogger<SendReminderEmailsService> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task SendReminderEmailsAsync(Meeting meeting)
    {
        if (meeting == null)
        {
            _logger.LogError("Meeting object is null");
            return;
        }

        _logger.LogInformation($"Sending reminder emails for meeting: {meeting.Name ?? "Unnamed meeting"}");

        var turkishTime = meeting.StartDateTime.AddHours(3);
        var emailBody = $@"
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
        ";

        if (meeting.MeetingParticipants != null && meeting.MeetingParticipants.Any())
        {
            foreach (var participant in meeting.MeetingParticipants)
            {
                if (!string.IsNullOrEmpty(participant?.Email))
                {
                    await _emailService.SendEmailAsync(participant.Email, 
                        $"Reminder: {meeting.Name ?? "Unnamed meeting"} Meeting Starts in 15 Minutes", 
                        emailBody);
                }
                else
                {
                    _logger.LogWarning($"Invalid participant email for meeting '{meeting.Name}'");
                }
            }
        }
        else
        {
            _logger.LogWarning($"No participants found for meeting '{meeting.Name}'");
        }
    }
}
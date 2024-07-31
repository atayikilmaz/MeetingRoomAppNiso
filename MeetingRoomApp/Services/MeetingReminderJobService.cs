using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeetingRoomApp.Interfaces;
using MeetingRoomApp.Models;
using MeetingRoomApp.Services;

public class MeetingReminderJobService : BackgroundService
{
    private readonly ILogger<MeetingReminderJobService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MeetingReminderJobService(ILogger<MeetingReminderJobService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var meetingRepository = scope.ServiceProvider.GetRequiredService<IMeetingRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<SendReminderEmailsService>();
                await CheckAndSendReminders(meetingRepository, emailService);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);        }
    }

    // MeetingReminderJobService.cs
    private async Task CheckAndSendReminders(IMeetingRepository meetingRepository, SendReminderEmailsService emailService)
    {
        var now = DateTime.UtcNow;
        var fifteenMinutesFromNow = now.AddMinutes(15);

        var upcomingMeetings = await meetingRepository.GetUpcomingMeetingsAsync(now, fifteenMinutesFromNow);

        if (!upcomingMeetings.Any())
        {
            _logger.LogInformation($"No upcoming meetings found in the next 15 minutes. Current UTC time: {now}");
            return;
        }

        _logger.LogInformation($"Found {upcomingMeetings.Count()} upcoming meetings. Current UTC time: {now}");

        foreach (var meeting in upcomingMeetings)
        {
            var timeUntilMeeting = meeting.StartDateTime - now;
            _logger.LogInformation($"Meeting '{meeting.Name}' is starting at {meeting.StartDateTime:yyyy-MM-dd HH:mm:ss} UTC " +
                                   $"(in {timeUntilMeeting.TotalMinutes:F1} minutes).");

            if (meeting.MeetingParticipants == null)
            {
                _logger.LogWarning($"Meeting '{meeting.Name}' has no participants (MeetingParticipants is null).");
                continue;
            }

            _logger.LogInformation($"Meeting participants: {string.Join(", ", meeting.MeetingParticipants.Select(p => p.Email))}");

            // Convert UTC time to Turkish time (UTC+3)
            var turkishTime = meeting.StartDateTime.AddHours(3);
            _logger.LogInformation($"Meeting '{meeting.Name}' Turkish time: {turkishTime:yyyy-MM-dd HH:mm:ss}");

            // Send reminder email if not already sent
            if (!meeting.ReminderSent)
            {
                await SendReminderEmail(meeting, emailService, meetingRepository);
            }
        }
    }
    private async Task SendReminderEmail(Meeting meeting, SendReminderEmailsService emailService, IMeetingRepository meetingRepository)
    {
        try
        {
            await emailService.SendReminderEmailsAsync(meeting);
            
            // Update the meeting to mark reminder as sent
            meeting.ReminderSent = true;
            await meetingRepository.UpdateMeetingAsync(meeting);
            // Log the meeting participants and their email addresses
            
            var participantInfo = string.Join(", ", meeting.MeetingParticipants.Select(p => $"{p.User} ({p.Email})"));
            _logger.LogInformation($"Reminder email sent for meeting '{meeting.Name}'. Participants: {participantInfo}");

            _logger.LogInformation($"Reminder email sent for meeting '{meeting.Name}'");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send reminder email for meeting '{meeting.Name}'");
        }
    }
}
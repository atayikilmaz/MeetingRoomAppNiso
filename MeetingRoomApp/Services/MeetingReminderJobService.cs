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
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var meetingRepository = scope.ServiceProvider.GetRequiredService<IMeetingRepository>();
                    var emailService = scope.ServiceProvider.GetRequiredService<SendReminderEmailsService>();
                    await CheckAndSendReminders(meetingRepository, emailService);
                    Console.Out.WriteLine("Checking and sending reminders...");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking and sending reminders.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CheckAndSendReminders(IMeetingRepository meetingRepository, SendReminderEmailsService emailService)
    {
        var now = DateTime.UtcNow;
        var fifteenMinutesFromNow = now.AddMinutes(15);

        var upcomingMeetings = await meetingRepository.GetUpcomingMeetingsAsync(now, fifteenMinutesFromNow);

        _logger.LogInformation($"Found {upcomingMeetings.Count()} upcoming meetings. Current UTC time: {now}");

        foreach (var meeting in upcomingMeetings)
        {
            var timeUntilMeeting = meeting.StartDateTime - now;
            _logger.LogInformation($"Meeting '{meeting.Name}' is starting at {meeting.StartDateTime:yyyy-MM-dd HH:mm:ss} UTC " +
                                   $"(in {timeUntilMeeting.TotalMinutes:F1} minutes).");

            if (meeting.MeetingParticipants == null || !meeting.MeetingParticipants.Any())
            {
                _logger.LogWarning($"Meeting '{meeting.Name}' has no participants.");
                continue;
            }

            var turkishTime = meeting.StartDateTime.AddHours(3);
            _logger.LogInformation($"Meeting '{meeting.Name}' Turkish time: {turkishTime:yyyy-MM-dd HH:mm:ss}");

            if (!meeting.ReminderSent && timeUntilMeeting <= TimeSpan.FromMinutes(15))
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
            
            meeting.ReminderSent = true;
            await meetingRepository.UpdateMeetingAsync(meeting);
            
            var participantInfo = string.Join(", ", meeting.MeetingParticipants.Select(p => $"{p.User?.Name ?? "Unknown"} ({p.Email})"));
            _logger.LogInformation($"Reminder email sent for meeting '{meeting.Name}'. Participants: {participantInfo}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send reminder email for meeting '{meeting.Name}'");
        }
    }
}
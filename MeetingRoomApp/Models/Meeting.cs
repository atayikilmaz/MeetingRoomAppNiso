namespace MeetingRoomApp.Models;

public class Meeting
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MeetingRoomId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool ReminderSent { get; set; }

    // Navigation properties
    public MeetingRoom MeetingRoom { get; set; }
    public ICollection<MeetingParticipant> MeetingParticipants { get; set; }

    // Helper property to easily access participant emails
    public IEnumerable<string> ParticipantEmails => MeetingParticipants?.Select(mp => mp.Email);
}
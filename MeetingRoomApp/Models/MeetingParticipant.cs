namespace MeetingRoomApp.Models;

public class MeetingParticipant
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public string ParticipantId { get; set; }

    // Navigation properties
    public Meeting Meeting { get; set; }
    public User User { get; set; }
}
namespace MeetingRoomApp.Models;

public class Meeting
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int MeetingRoomId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    // Navigation properties
    public MeetingRoom MeetingRoom { get; set; }
    public ICollection<MeetingParticipant> MeetingParticipants { get; set; }
}
namespace MeetingRoomApp.Models;

public class MeetingRoom
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Meeting>? Meetings { get; set; }
}

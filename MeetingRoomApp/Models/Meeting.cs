namespace MeetingRoomApp.Models;

public class Meeting
{
    public int Id  { get; set; }
    public string Name { get; set; }
    public ICollection<User> Participants { get; set; }
    public int MeetingRoomId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
}
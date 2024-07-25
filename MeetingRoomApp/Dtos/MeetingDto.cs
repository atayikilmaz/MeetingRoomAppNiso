namespace MeetingRoomApp.Dtos;

public class MeetingDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public List<string> Participants { get; set; }
    public string MeetingRoom { get; set; }
}
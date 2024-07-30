namespace MeetingRoomApp.Dtos;

public class UpdateMeetingDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public List<string> ParticipantIds { get; set; }
    public int MeetingRoomId { get; set; }
}
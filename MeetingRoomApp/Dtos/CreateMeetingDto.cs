namespace MeetingRoomApp.Dtos;

public class CreateMeetingDto
{
    public string Name { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    public List<int> ParticipantIds { get; set; }
    public int MeetingRoomId { get; set; }
}
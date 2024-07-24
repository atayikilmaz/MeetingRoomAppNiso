namespace MeetingRoomApp.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }

    public ICollection<MeetingParticipant> MeetingParticipants { get; set; }
}
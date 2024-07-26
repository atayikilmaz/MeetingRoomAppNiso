using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MeetingRoomApp.Models;

public class User: IdentityUser
{
    public string? Name { get; set; }

    public ICollection<MeetingParticipant> MeetingParticipants { get; set; }
}
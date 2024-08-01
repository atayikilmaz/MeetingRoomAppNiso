
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomApp.Models
{
    public class TwoFactorAuthModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "The 2FA token must be 6 characters long.")]
        public string Token { get; set; }
    }
}
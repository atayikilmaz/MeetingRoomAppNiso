using MeetingRoomApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRoomApp.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        
        Task<string> GetEmailByUserIdAsync(string userId);
        Task<User> GetUserByIdAsync(string id);
        Task DeleteUserAsync(string id);
        Task<User> ChangeUserRoleAsync(string id, string newRole);    }
}
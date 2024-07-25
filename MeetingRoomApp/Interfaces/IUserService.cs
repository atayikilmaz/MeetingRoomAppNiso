using MeetingRoomApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRoomApp.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task DeleteUserAsync(int id);
        Task<User> ChangeUserRoleAsync(int id, string newRole);    }
}
using MeetingRoomApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRoomApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task DeleteAsync(User user);
        Task UpdateAsync(User user);
    }
}
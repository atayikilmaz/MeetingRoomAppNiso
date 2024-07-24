using MeetingRoomApp.Models;

namespace MeetingRoomApp.Interfaces;

public interface IUserRepository
{
    Task<User> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);


    
    Task<IEnumerable<User>> GetAllAsync();
}

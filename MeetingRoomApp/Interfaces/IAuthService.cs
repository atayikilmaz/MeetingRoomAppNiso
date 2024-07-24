using MeetingRoomApp.Dtos;

namespace MeetingRoomApp.Interfaces;

public interface IAuthService
{
    Task<UserAuthDto> RegisterAsync(UserAuthDto registerDto);
    Task<string> LoginAsync(UserAuthDto loginDto);
}
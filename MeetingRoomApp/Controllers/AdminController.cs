using MeetingRoomApp.Dtos;
using MeetingRoomApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomApp.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AdminController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserAuthDto>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users.Select(u => new UserAuthDto { Id = u.Id, Email = u.Email, Role = u.Role }));
    }


}
using AutoMapper;
using MeetingRoomApp.Dtos;
using MeetingRoomApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = $"User with id {id} has been deleted." });
        }

        [HttpPut("{id}/role")]
        public async Task<ActionResult<UserDto>> ChangeUserRole(int id, ChangeRoleDto changeRoleDto)
        {
            if (id != changeRoleDto.Id)
            {
                return BadRequest("Id in the URL and Id in the body do not match.");
            }

            var updatedUser = await _userService.ChangeUserRoleAsync(id, changeRoleDto.Role);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDto>(updatedUser));
        }
    }
}
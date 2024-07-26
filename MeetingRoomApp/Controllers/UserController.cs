using AutoMapper;
using MeetingRoomApp.Dtos;
using MeetingRoomApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeetingRoomApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MeetingRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUserService userService, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

       



       
      [HttpPost("{id}/make-admin")]
        [Authorize(Roles = "Admin")] // Only allow admins to perform this action
        public async Task<IActionResult> MakeUserAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return BadRequest($"User {user.UserName} is already an Admin.");
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (result.Succeeded)
            {
                return Ok($"User {user.UserName} has been made an Admin.");
            }
            else
            {
                return BadRequest($"Failed to make user {user.UserName} an Admin. Errors: {string.Join(", ", result.Errors)}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = $"User with id {id} has been deleted." });
        }

        [HttpPut("{id}/role")]
        public async Task<ActionResult<UserDto>> ChangeUserRole(string id, ChangeRoleDto changeRoleDto)
        {
            if (id != changeRoleDto.Id.ToString())
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
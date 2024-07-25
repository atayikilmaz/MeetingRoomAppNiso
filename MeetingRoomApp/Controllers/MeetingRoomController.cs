using System.Threading.Tasks;
using MeetingRoomApp.Models;
using MeetingRoomApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MeetingRoomApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeetingRoomController : ControllerBase
    {
        private readonly IMeetingRoomService _meetingRoomService;

        public MeetingRoomController(IMeetingRoomService meetingRoomService)
        {
            _meetingRoomService = meetingRoomService;
        }

        [HttpPost]
        public async Task<ActionResult<MeetingRoom>> Create(MeetingRoom meetingRoom)
        {
            var createdMeetingRoom = await _meetingRoomService.CreateMeetingRoomAsync(meetingRoom);
            return Ok(createdMeetingRoom);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _meetingRoomService.DeleteMeetingRoomAsync(id);
            return Ok();
        }
    }
}
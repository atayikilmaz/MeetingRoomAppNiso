using MeetingRoomApp.Dtos;
using MeetingRoomApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingsController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MeetingDto>>> GetAllMeetings()
    {
        var meetings = await _meetingService.GetAllMeetingsAsync();
        return Ok(meetings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MeetingDto>> GetMeeting(int id)
    {
        var meeting = await _meetingService.GetMeetingByIdAsync(id);
        if (meeting == null)
        {
            return NotFound();
        }
        return Ok(meeting);
    }

    [HttpPost]
    public async Task<ActionResult<MeetingDto>> CreateMeeting(CreateMeetingDto createMeetingDto)
    {
        var createdMeeting = await _meetingService.CreateMeetingAsync(createMeetingDto);
        return CreatedAtAction(nameof(GetMeeting), new { id = createdMeeting.Id }, createdMeeting);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MeetingDto>> UpdateMeeting(int id, UpdateMeetingDto updateMeetingDto)
    {
        if (id != updateMeetingDto.Id)
        {
            return BadRequest("Id in the URL and Id in the body do not match.");
        }

        var updatedMeeting = await _meetingService.UpdateMeetingAsync(updateMeetingDto);
        if (updatedMeeting == null)
        {
            return NotFound();
        }
        return Ok(updatedMeeting);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMeeting(int id)
    {
        await _meetingService.DeleteMeetingAsync(id);
        return Ok(new { message = $"Meeting with id {id} has been deleted." });
    }
}
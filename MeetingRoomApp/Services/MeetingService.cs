using AutoMapper;
using MeetingRoomApp.Dtos;
using MeetingRoomApp.Exceptions;
using MeetingRoomApp.Interfaces;
using MeetingRoomApp.Models;

namespace MeetingRoomApp.Services;

public class MeetingService : IMeetingService
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public MeetingService(IMeetingRepository meetingRepository, IUserService userService, IMapper mapper)
    {
        _meetingRepository = meetingRepository;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<List<MeetingDto>> GetAllMeetingsAsync()
    {
        var meetings = await _meetingRepository.GetAllMeetingsAsync();
        return _mapper.Map<List<MeetingDto>>(meetings);
    }

    public async Task<MeetingDto> GetMeetingByIdAsync(int id)
    {
        var meeting = await _meetingRepository.GetMeetingByIdAsync(id);
        return _mapper.Map<MeetingDto>(meeting);
    }

    public async Task<MeetingDto> CreateMeetingAsync(CreateMeetingDto createMeetingDto)
    {
        var meeting = _mapper.Map<Meeting>(createMeetingDto);

        bool isOverlapping = await _meetingRepository.IsMeetingOverlappingAsync(meeting.MeetingRoomId, meeting.StartDateTime, meeting.EndDateTime);
        if (isOverlapping)
        {
            throw new ConflictException("The meeting time overlaps with another meeting in the same room.");
        }

        meeting.MeetingParticipants = new List<MeetingParticipant>();
        foreach (var userId in createMeetingDto.ParticipantIds)
        {
            var email = await _userService.GetEmailByUserIdAsync(userId);
            meeting.MeetingParticipants.Add(new MeetingParticipant
            {
                ParticipantId = userId.ToString(),
                Email = email
            });
        }

        var createdMeeting = await _meetingRepository.CreateMeetingAsync(meeting);

        var fullMeeting = await _meetingRepository.GetMeetingByIdAsync(createdMeeting.Id);

        return _mapper.Map<MeetingDto>(fullMeeting);
    }    
    
    
    
    public async Task<MeetingDto> UpdateMeetingAsync(UpdateMeetingDto updateMeetingDto)
    {
        var existingMeeting = await _meetingRepository.GetMeetingByIdAsync(updateMeetingDto.Id);
        if (existingMeeting == null)
        {
            throw new NotFoundException($"Meeting with ID {updateMeetingDto.Id} not found.");
        }

        _mapper.Map(updateMeetingDto, existingMeeting);

        // Update participants
        existingMeeting.MeetingParticipants.Clear();
        foreach (var userId in updateMeetingDto.ParticipantIds)
        {
            var email = await _userService.GetEmailByUserIdAsync(userId);
            existingMeeting.MeetingParticipants.Add(new MeetingParticipant
            {
                ParticipantId = userId,
                Email = email
            });
        }

        var updatedMeeting = await _meetingRepository.UpdateMeetingAsync(existingMeeting);
        return _mapper.Map<MeetingDto>(updatedMeeting);
    }

    public async Task DeleteMeetingAsync(int id)
    {
        await _meetingRepository.DeleteMeetingAsync(id);
    }
}


using AutoMapper;
using MeetingRoomApp.Dtos;
using MeetingRoomApp.Exceptions;
using MeetingRoomApp.Interfaces;
using MeetingRoomApp.Models;

namespace MeetingRoomApp.Services;

public class MeetingService : IMeetingService
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IMapper _mapper;

    public MeetingService(IMeetingRepository meetingRepository, IMapper mapper)
    {
        _meetingRepository = meetingRepository;
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
    
        meeting.MeetingParticipants = createMeetingDto.ParticipantIds.Select(userId => new MeetingParticipant
        {
            UserId = userId
        }).ToList();

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
        var updatedMeeting = await _meetingRepository.UpdateMeetingAsync(existingMeeting);
        return _mapper.Map<MeetingDto>(updatedMeeting);
    }

    public async Task DeleteMeetingAsync(int id)
    {
        await _meetingRepository.DeleteMeetingAsync(id);
    }
}


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
    private readonly IMeetingRoomRepository _meetingRoomRepository;


    public MeetingService(IMeetingRepository meetingRepository, IUserService userService, IMapper mapper, IMeetingRoomRepository meetingRoomRepository)
    {
        _meetingRepository = meetingRepository;
        _userService = userService;
        _mapper = mapper;
        _meetingRoomRepository = meetingRoomRepository;
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
    
    public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(int roomId, DateTime date)
    {
        var room = await _meetingRoomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            throw new ArgumentException("Invalid room id");
        }

        var startOfDay = date.Date.AddHours(3); // Start at 03:00 UTC
        var endOfDay = date.Date.AddHours(21); // End at 21:00 UTC

        var existingMeetings = await _meetingRepository.GetMeetingsByRoomAndDateRangeAsync(roomId, startOfDay, endOfDay);
        var availableSlots = new List<TimeSlot>();

        var currentTime = startOfDay;
        while (currentTime < endOfDay)
        {
            var slotEnd = currentTime.AddMinutes(30);
            if (!existingMeetings.Any(m => m.StartDateTime < slotEnd && m.EndDateTime > currentTime))
            {
                availableSlots.Add(new TimeSlot { StartTime = currentTime, EndTime = slotEnd });
            }
            currentTime = slotEnd;
        }

        return availableSlots;
    }   
    
    
}


using AutoMapper;
using MeetingRoomApp.Dtos;
using MeetingRoomApp.Models;

namespace MeetingRoomApp.Profiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Meeting, MeetingDto>()
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.MeetingParticipants.Select(mp => mp.User.Name)))
            .ForMember(dest => dest.MeetingRoom, opt => opt.MapFrom(src => src.MeetingRoom.Name));

        CreateMap<CreateMeetingDto, Meeting>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.StartDateTime))
            .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => src.EndDateTime))
            .ForMember(dest => dest.MeetingRoomId, opt => opt.MapFrom(src => src.MeetingRoomId))
            .ForMember(dest => dest.MeetingParticipants, opt => opt.Ignore());
        CreateMap<Meeting, MeetingDto>()
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => 
                src.MeetingParticipants.Select(mp => mp.User.Name).ToList()))
            .ForMember(dest => dest.MeetingRoom, opt => opt.MapFrom(src => src.MeetingRoom.Name));
      
        CreateMap<UpdateMeetingDto, Meeting>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.StartDateTime))
            .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => src.EndDateTime))
            .ForMember(dest => dest.MeetingRoomId, opt => opt.MapFrom(src => src.MeetingRoomId))
            .ForMember(dest => dest.MeetingParticipants, opt => opt.Ignore());
      

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

    }
}
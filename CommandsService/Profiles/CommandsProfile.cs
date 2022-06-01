using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;

namespace CommandsService.Profiles;


public class CommandsProfile : Profile{
    public CommandsProfile(){
        CreateMap<Platform, PlatformReadDTO>();
        CreateMap<CommandCreateDTO, Command>();
        CreateMap<Command, CommandReadDTO>();
        CreateMap<PlatformPublishedDTO, Platform>()
            .ForMember(
                dest => dest.ExternalId, 
                opt => opt.MapFrom(src => src.Id));
        
    }
}
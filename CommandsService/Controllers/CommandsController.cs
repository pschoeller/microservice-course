using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase{
    
    private readonly ICommandRepo repository;
    private readonly IMapper mapper;
    
    public CommandsController(ICommandRepo repository, IMapper mapper){
        this.repository = repository;
        this.mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDTO>> GetCommandsForPlatform(int platformId){
        System.Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");
        
        if(!repository.PlatformExists(platformId)){
            return NotFound("The platform was not found.");
        }
        
        var commands = repository.GetCommandsForPlatform(platformId);
        return Ok(mapper.Map<IEnumerable<CommandReadDTO>>(commands));
    }
    
    [HttpGet("{commandId}", Name="GetCommandForPlatform")]
    public ActionResult<CommandReadDTO> GetCommandForPlatform(int platformId, int commandId){
        System.Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");
        
        if(!repository.PlatformExists(platformId)){
            return NotFound("The platform was not found.");
        }
        
        var command = repository.GetCommand(platformId, commandId);
        if(command == null){
            return NotFound("The command was not found");
        }
        
        return Ok(mapper.Map<CommandReadDTO>(command));
    }
    
    [HttpPost]
    public ActionResult<CommandReadDTO> CreateCommandForPlatform(int platformId, CommandCreateDTO commandDTO){
        System.Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");
        
        if(!repository.PlatformExists(platformId)){
            return NotFound("The platform was not found.");
        }
        
        var command = mapper.Map<Command>(commandDTO);
        
        repository.CreateCommand(platformId, command);
        repository.SaveChanges();
        
        var commandReadDto = mapper.Map<CommandReadDTO>(command);
        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new {platformId = platformId,
            commandId = commandReadDto.Id}, 
            commandReadDto);
    }
}
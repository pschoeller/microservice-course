using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.ASyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase{
    
    private readonly IPlatformRepo repository;
    private readonly IMapper mapper;
    private readonly ICommandDataClient commandDataClient;
    private readonly IMessageBusClient messageBusClient;

    public PlatformsController(
            IPlatformRepo repository, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient){
        this.repository = repository;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
        this.messageBusClient = messageBusClient;
    }
    
    
    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms(){
        System.Console.WriteLine("--> Getting Platforms");
        var platformItems = repository.GetAllPlatforms();
        return Ok(mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems));
    }
    
    [HttpGet("{id}", Name="GetPlatformById")]
    public ActionResult<PlatformReadDTO> GetPlatformById(int id){
        System.Console.WriteLine("--> Getting Platform");
        var platformItem = repository.GetPlatformById(id);
        
        if(platformItem != null){
            return Ok(mapper.Map<PlatformReadDTO>(platformItem));
        }
        
        return NotFound("Unable to find the requested item.");
    }
    
    [HttpPost]
    public async Task<ActionResult<PlatformReadDTO>> CreatePlatform(PlatformCreateDTO platformCreateDTO){
        var platformModel = mapper.Map<Platform>(platformCreateDTO);
        
        repository.CreatePlatform(platformModel);
        repository.SaveChanges();
        
        var platformReadDTO = mapper.Map<PlatformReadDTO>(platformModel);
        
        // Send Sync message
        try{
            await commandDataClient.SendPlatformToCommand(platformReadDTO);
        }
        catch(Exception ex){
            System.Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }
        
        // Send ASync message
        try{
            var platformPublishedDTO = mapper.Map<PlatformPublishedDTO>(platformReadDTO);
            platformPublishedDTO.Event = "Platform_Published";
            messageBusClient.PublishNewPlatform(platformPublishedDTO);
        }
        catch(Exception ex){
            System.Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }
        
        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDTO.Id}, platformReadDTO);
    }
}
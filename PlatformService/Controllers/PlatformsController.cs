using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    public PlatformsController(
            IPlatformRepo repository, 
            IMapper mapper,
            ICommandDataClient commandDataClient){
        this.repository = repository;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
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
        
        try{
            await commandDataClient.SendPlatformToCommand(platformReadDTO);
        }
        catch(Exception ex){
            System.Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }
        
        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDTO.Id}, platformReadDTO);
    }
}
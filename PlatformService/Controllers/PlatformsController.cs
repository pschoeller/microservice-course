using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase{
    
    private readonly IPlatformRepo repository;
    private readonly IMapper mapper;

    public PlatformsController(IPlatformRepo repository, IMapper mapper){
        this.repository = repository;
        this.mapper = mapper;
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
    public ActionResult<PlatformReadDTO> CreatePlatform(PlatformCreateDTO platformCreateDTO){
        var platformModel = mapper.Map<Platform>(platformCreateDTO);
        
        repository.CreatePlatform(platformModel);
        repository.SaveChanges();
        
        var platformReadDTO = mapper.Map<PlatformReadDTO>(platformModel);
        
        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDTO.Id}, platformReadDTO);
    }
}
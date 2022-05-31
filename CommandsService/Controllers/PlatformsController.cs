using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;


[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase{

    private readonly ICommandRepo commandRepo;
    private readonly IMapper mapper;

    public PlatformsController(ICommandRepo commandRepo, IMapper mapper){
        this.commandRepo = commandRepo;
        this.mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms(){
        System.Console.WriteLine("--> Getting Platforms from CommandsService.");
        
        var platformsItems = commandRepo.GetAllPlatforms();
        return Ok(mapper.Map<IEnumerable<PlatformReadDTO>>(platformsItems));
    }
    
    [HttpPost]
    public ActionResult TestInboundConnection(){
        System.Console.WriteLine("--> Inbound POST @ Command Service");
        return Ok("Inboud Connection Test Succeded!");
    }
}
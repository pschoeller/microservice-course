using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMapper mapper;

    public EventProcessor(
        IServiceScopeFactory scopeFactory,
        IMapper mapper){
        this.scopeFactory = scopeFactory;
        this.mapper = mapper;
    }
    
    public void ProcessEvent(string message){
        var eventType = DetermineEvent(message);
        switch(eventType){
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }
    
    private EventType DetermineEvent(string notificationMessage){
        System.Console.WriteLine($"--> Determining Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage);
        
        switch(eventType.Event){
            case "Platform_Published":
                System.Console.WriteLine("--> Platform_Published Event Detected");
                return EventType.PlatformPublished;
            default:
                System.Console.WriteLine("--> Could not determine event type");
                return EventType.Undetermined;
        }
    }
    
    private void AddPlatform(string platformPublishedMessage){
        using(var scope = scopeFactory.CreateScope()){
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);
            
            try{
                var plat = mapper.Map<Platform>(platformPublishedDTO);
                if(!repo.ExternalPlatformExists(plat.ExternalId)){
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                    System.Console.WriteLine($"--> Platform added.");
                }
                else{
                    System.Console.WriteLine($"--> Platform already exists.");
                }
            }
            catch(Exception ex){
                System.Console.WriteLine($"--> Could not add platform to DB {ex.Message}");
            }
        }
    }
}

enum EventType{
    PlatformPublished,
    Undetermined
}
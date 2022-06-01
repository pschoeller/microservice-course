using CommandsService.Models;

namespace CommandsService.Data;


public class CommandRepo : ICommandRepo{
    
    private readonly AppDbContext dbContext;
    
    public CommandRepo(AppDbContext dbContext){
        this.dbContext = dbContext;
    }
    
    
    public void CreateCommand(int platformId, Command command){
        if(command == null){
            throw new ArgumentNullException(nameof(command));
        }
        
        command.PlatformId = platformId;
        dbContext.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform){
        if(platform == null){
            throw new ArgumentNullException(nameof(platform));
        }
        
        dbContext.Platforms.Add(platform);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        return (dbContext.Platforms.Any(p => p.ExternalId == externalPlatformId));
    }

    public IEnumerable<Platform> GetAllPlatforms(){
        return dbContext.Platforms.ToList();
    }

    public Command GetCommand(int platformId, int commandId){
        return dbContext
            .Commands
            .Where(c => c.PlatformId == platformId && c.Id == commandId)
            .FirstOrDefault();
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId){
        return dbContext
            .Commands
            .Where(c => c.Id == platformId)
            .OrderBy(c => c.Platform.Name);
    }

    public bool PlatformExists(int platformId){
        return (dbContext.Platforms.Any(p => p.Id == platformId));
    }

    public bool SaveChanges(){
        return (dbContext.SaveChanges() >= 0);
    }
}
using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepo : IPlatformRepo{
    
    private readonly AppDbContext context;
    
    public PlatformRepo(AppDbContext dbContext){
        this.context = dbContext;
    }
    
    public void CreatePlatform(Platform plat){
        if(plat == null){
            throw new ArgumentNullException(nameof(plat));
        }
        
        context.Platforms.Add(plat);
    }

    public IEnumerable<Platform> GetAllPlatforms(){
        return context.Platforms.ToList();
    }

    public Platform GetPlatformById(int id){
        return context.Platforms
            .FirstOrDefault(p => p.Id == id);
    }

    public bool SaveChanges(){
        return (context.SaveChanges() >= 0);
    }
}
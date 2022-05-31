using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public class AppDbContext : DbContext{
    
    public DbSet<Platform> Platforms { get; set; }  = null!;
    public DbSet<Command> Commands { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt){
        
    }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder){
        modelBuilder
            .Entity<Platform>()
            .HasMany(p => p.Commands)
            .WithOne(p => p.Platform!)
            .HasForeignKey(p => p.PlatformId);
            
        modelBuilder
            .Entity<Command>()
            .HasOne(p => p.Platform)
            .WithMany(p => p.Commands)
            .HasForeignKey(p => p.PlatformId);
    }
}
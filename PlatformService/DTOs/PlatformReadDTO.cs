namespace PlatformService.DTOs;


public class PlatformReadDTO{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Publisher { get; set; } = null!; 
    public string Cost { get; set; } = null!;
}
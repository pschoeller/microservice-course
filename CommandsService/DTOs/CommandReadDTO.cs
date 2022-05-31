namespace CommandsService.DTOs;


public class CommandReadDTO{
    
    public int Id { get; set; }
    public string HowTo { get; set; } = null!;
    public string CommandLine { get; set; } = null!;
    public int PlatformId { get; set; }
}
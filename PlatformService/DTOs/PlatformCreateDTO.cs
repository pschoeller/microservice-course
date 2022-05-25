using System.ComponentModel.DataAnnotations;

namespace PlatformService.DTOs;


public class PlatformCreateDTO{
    
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public string Publisher { get; set; } = null!; 
    
    [Required]
    public string Cost { get; set; } = null!;
}
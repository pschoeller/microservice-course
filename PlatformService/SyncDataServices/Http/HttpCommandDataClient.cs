using System.Text;
using System.Text.Json;
using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http;


public class HttpCommandDataClient : ICommandDataClient{
    
    private readonly HttpClient httpClient;
    private readonly IConfiguration config;
    
    public HttpCommandDataClient(HttpClient httpClient, IConfiguration config){
        this.httpClient = httpClient;
        this.config = config;
    }
    
    
    public async Task SendPlatformToCommand(PlatformReadDTO plat){
        var httpContent = new StringContent(
            JsonSerializer.Serialize(plat),
            Encoding.UTF8,
            "application/json"
        );
        
        var response = await httpClient
            .PostAsync($"{config["CommandService"]}", httpContent);
        
        if(response.IsSuccessStatusCode){
            System.Console.WriteLine("--> Sync POST to Command Service was OK");
        }
        else{
            System.Console.WriteLine("--> Sync POST to Command Service was NOT OK");
        }
    }
}
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc;


public class PlatformDataClient : IPlatformDataClient{
    
    private readonly IConfiguration config;
    private readonly IMapper mapper;

    public PlatformDataClient(IConfiguration config, IMapper mapper){
        this.config = config;
        this.mapper = mapper;
    }
    
    public IEnumerable<Platform> ReturnAllPlatforms(){
        System.Console.WriteLine($"--> Calling GRPC Service... {config["GrpcPlatform"]}");
        
        var channel = GrpcChannel.ForAddress(config["GrpcPlatform"]);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();
        
        try{
            var reply = client.GetAllPlatforms(request);
            return mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch(Exception ex){
            System.Console.WriteLine($"--> Could not call GRPC server {ex.Message}");
            return null;
        }
    }
}
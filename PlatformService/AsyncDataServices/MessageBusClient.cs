using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.ASyncDataServices;


public class MessageBusClient : IMessageBusClient{
    
    private readonly IConfiguration configuration;
    private readonly IConnection connection = null!;
    private readonly IModel channel = null!;

    public MessageBusClient(IConfiguration configuration){
        this.configuration = configuration;
        var factory = new ConnectionFactory(){
          HostName = configuration["RabbitMQHost"],
          Port = int.Parse(configuration["RabbitMQPort"])
        };
        
        try{
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            System.Console.WriteLine("--> Connected to Message Bus");
        }
        catch(Exception ex){
            System.Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        }
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e){
        System.Console.WriteLine("--> Connection has been shut down.");
    }

    public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO){
        var message = JsonSerializer.Serialize(platformPublishedDTO);
        
        if(connection.IsOpen){
            System.Console.WriteLine("--> RabbitMQ connection open, sending message...");
            SendMessage(message);
        }
        else{
            System.Console.WriteLine("--> RabbitMQ connection is closed, not sending.");
        }
    }

    private void SendMessage(string message){
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body);
        System.Console.WriteLine($"--> We have sent {message}");
    }
    
    public void Dispose(){
        System.Console.WriteLine("--> Message Bus disposed");
        if(channel.IsOpen){
            channel.Close();
            connection.Close();
        }
    }
}
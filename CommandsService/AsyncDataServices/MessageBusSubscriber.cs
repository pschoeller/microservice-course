using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices;


public class MessageBusSubscriber : BackgroundService{
    private readonly IConfiguration config = null!;
    private readonly IEventProcessor eventProcessor = null!;
    private IConnection connection = null!;
    private IModel channel = null!;
    private string queueName = null!;

    public MessageBusSubscriber(
        IConfiguration config, 
        IEventProcessor eventProcessor){
        this.config = config;
        this.eventProcessor = eventProcessor;
        InitializeRabbitMQ();
    }
    
    private void InitializeRabbitMQ(){
        var factory = new ConnectionFactory(){
          HostName = config["RabbitMQHost"],
          Port = int.Parse(config["RabbitMQPort"])
        };
        
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(
            exchange: "trigger", 
            type: ExchangeType.Fanout);
        queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(
            queue: queueName, 
            exchange: "trigger", 
            routingKey: "");
        
        System.Console.WriteLine($"--> Listening on the Message Bus...");
        
        connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e){
        System.Console.WriteLine("--> Connection Shutdown");
    }
    
    public override void Dispose(){
        if(channel.IsOpen){
            channel.Close();
        }
        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken){
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);
        
        consumer.Received += (ModuleHandle, ea) => {
            System.Console.WriteLine("--> Event Received.");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            
            eventProcessor.ProcessEvent(notificationMessage);
        };
        
        channel.BasicConsume(
            queue: queueName, 
            autoAck: true, 
            consumer: consumer);
        
        return Task.CompletedTask;
    }
}
using Microsoft.EntityFrameworkCore;
using PlatformService.ASyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();

if(builder.Environment.IsProduction()){
    System.Console.WriteLine("--> Using Production Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection"))
    );
}
else{
    System.Console.WriteLine("--> Using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem")
    );
}
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();
// builder.WebHost.ConfigureKestrel(
//     options =>{
//         options.ListenAnyIP(
//             7059, /*Int32.Parse(builder.Configuration[""])*/
//             listenoptions =>{
//                 listenoptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
//             });
//     }
// );

System.Console.WriteLine($"--> Command Service Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context => {
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});


PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();

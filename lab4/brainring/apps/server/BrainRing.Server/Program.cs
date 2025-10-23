using BrainRing.Application.Extensions;
using BrainRing.DbAdapter;
using BrainRing.DbAdapter.Extensions;
using BrainRing.Domain;
using BrainRing.Server.Middlewares;
using BrainRing.Server.WebSockets;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .WithOrigins("http://localhost:3000")
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

// ----- DB ADAPTER -----
builder.Services.AddDBService(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterEntityMapper();

builder.Services.RegisterServices();


// ----- WEB SOCKETS -----
builder.Services.AddSingleton<WebSocketConnectionManager>();
builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

app.UsePathBase("/api");

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseMiddleware<AuthMiddleware>();

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.MapControllers();

app.MigrateDatabase<BrainRingDbContext>();

app.Run();
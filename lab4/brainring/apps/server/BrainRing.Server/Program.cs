using BrainRing.Application.Extensions;
using BrainRing.DbAdapter;
using BrainRing.DbAdapter.Extensions;
using BrainRing.Server.Middleware;
using BrainRing.Server.WebSockets;

var builder = WebApplication.CreateBuilder(args);

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

app.MigrateDatabase<BrainRingDbContext>();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.UseHttpsRedirection();

app.UsePathBase("/api");

app.UseRouting();

app.MapControllers();

app.Run();

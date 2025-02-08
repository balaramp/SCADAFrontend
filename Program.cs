using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SCADAFrontend.Data;
using SCADAFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"))
);

// Add Services
builder.Services.AddScoped<DataAcquisitionService>();
builder.Services.AddScoped<HistoricalDataService>();
builder.Services.AddScoped<WebSocketService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Enable WebSockets
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2) // Keeps connection alive
};
app.UseWebSockets(webSocketOptions);

// Map WebSocket connection handler
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketService = context.RequestServices.GetRequiredService<WebSocketService>();

            // Start WebSocket connection handling
            await webSocketService.HandleWebSocketConnection(webSocket, "172.16.0.39", 502);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();


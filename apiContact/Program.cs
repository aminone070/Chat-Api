using apiContact.Data;
using apiContact.Hubs;
using apiContact.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chat API",
        Version = "v1",
        Description = "Realtime chat API with WebSocket (SignalR), MongoDB, Redis, and Blob Storage support.",
        Contact = new OpenApiContact { Name = "Chat API Team", Email = "api@chat.io" }
    });
    c.EnableAnnotations();
});

builder.Services.AddSignalR();

builder.Services.AddSingleton<ChatDbContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ChatPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Chat API — Swagger UI";
    c.InjectStylesheet("/css/swagger-custom.css");
});

app.UseCors("ChatPolicy");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    services = new { websocket = "SignalR", database = "MongoDB (in-memory fallback)", cache = "Redis (optional)", storage = "Blob (local)" }
});

app.Run();

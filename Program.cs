using MessagingQueueApp.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MessageRepository>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new MessageRepository(cfg.GetConnectionString("DefaultConnection"));
});

// Select queue provider
if (builder.Configuration.GetValue<string>("QueueProvider") == "InMemory")
    builder.Services.AddSingleton<IQueueProvider, InMemoryQueue>();
else
    builder.Services.AddSingleton<IQueueProvider, InMemoryQueue>(); // Replace with RedisQueueProvider if implemented

builder.Services.AddSingleton<MessageProcessor>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

var proc = app.Services.GetRequiredService<MessageProcessor>();
proc.Start();

app.Run();
using MessagingQueueApp.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register message repository and queue
builder.Services.AddSingleton<MessageRepository>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new MessageRepository(cfg.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSingleton<IQueueProvider, InMemoryQueue>();
builder.Services.AddSingleton<MessageProcessor>();
builder.Services.AddSingleton<Producer>();
builder.Services.AddHostedService<Consumer>();
var app = builder.Build();

// Use Swagger for API docs
app.UseSwagger();
app.UseSwaggerUI();

// Serve static files and default files (for /Dashboard)
app.UseDefaultFiles(); // Serves index.html by default
app.UseStaticFiles();


app.MapControllers();

// Start background message processor
app.Services.GetRequiredService<MessageProcessor>().Start();

app.Run();
public partial class Program { }

using Serilog;
using SPS.Application;
using SPS.Extensions;
using SPS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, services, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// Shared layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Authentication & Authorization (نفس الامتدادات السابقة)
builder.Services.AddJwtAuthentication(builder.Configuration); // سنأخذها من الامتداد المعدل الذي سننشئه في API.Extensions
builder.Services.AddDefaultAuthorizationPolicies();

// Swagger
builder.Services.AddSwaggerWithJwt("SPS API", "v1");

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseSwaggerWithUI("SPS API");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
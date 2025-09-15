using FCG.MS.Payments.Application.Interfaces;
using FCG.MS.Payments.Infrastructure.Configuration;
using FCG.MS.Payments.Infrastructure.Services;
using FCG.MS.Payments.API.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Stripe settings
builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));

// Configure API Key settings
builder.Services.Configure<ApiKeySettings>(
    builder.Configuration.GetSection("ApiKey"));

// Configure Admin settings
builder.Services.Configure<AdminSettings>(
    builder.Configuration.GetSection("Admin"));

// Register services
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Payments API is healthy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

// Add API Key authentication middleware
app.UseApiKeyAuthentication();

app.UseAuthorization();

app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();

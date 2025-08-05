using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using AutoMapper;
using FCG_MS_Payments.Application.Mapping;
using FCG_MS_Payments.Application.Interfaces;
using FCG_MS_Payments.Application.Services;
using FCG_MS_Payments.Application.Validators;
using FCG_MS_Payments.Domain.Interfaces;
using FCG_MS_Payments.Infrastructure.Services;
using FCG_MS_Payments.Infrastructure.Repositories;
using FCG_MS_Payments.Api.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreatePaymentRequestValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(PaymentMappingProfile));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "your-super-secret-key-with-at-least-32-characters");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Stripe
var stripeSettings = builder.Configuration.GetSection("StripeSettings");
StripeConfiguration.ApiKey = stripeSettings["SecretKey"] ?? "sk_test_your_stripe_test_key";

// Register Application Services
builder.Services.AddScoped<IPaymentApplicationService, PaymentApplicationService>();

// Register Domain Services
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Register Infrastructure Services
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IPaymentRepository, InMemoryPaymentRepository>();

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

// Configure Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map health checks
app.MapHealthChecks("/health");

app.Run();

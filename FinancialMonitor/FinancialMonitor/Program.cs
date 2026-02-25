using FinancialMonitor.Data;
using FinancialMonitor.Interfaces;
using FinancialMonitor.Services;
using FinancialMonitor.Routes;
using FinancialMonitor.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using FluentValidation;
using FinancialMonitor.Validators;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Configuration
// ==========================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

var allowedOrigins =
    builder.Configuration.GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (allowedOrigins is null || allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("CORS origins not configured.");
}

// ==========================
// Services
// ==========================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters
        .Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<TransactionValidator>();
// ==========================
// Build
// ==========================

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// ==========================
// Middleware
// ==========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

// ==========================
// Endpoints
// ==========================

app.MapTransactionRoutes();
app.MapHub<TransactionHub>("/transactionHub");

app.Run();
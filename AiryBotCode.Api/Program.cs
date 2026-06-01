using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Infrastructure.Database.Persistence;
using AiryBotCode.Infrastructure.Database.Repository.BotSettings;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

var webAppPolicy = "WebApp";

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: webAppPolicy,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// Persistence for the control-panel settings endpoints. Registered lazily so the
// API still starts (auth, /ping) in environments without a database configured;
// the settings routes return 500 until Database:* is set. Unlike the bot's
// AIDbContext.registerDbContext, this does not seed or probe the connection at
// startup — the UI talks to the same BotSetting rows the bots read.
var dbHost = Configuration["Database:Host"];
if (!string.IsNullOrWhiteSpace(dbHost))
{
    var connectionString = new ConfigurationReader(Configuration).GetDatabaseConnectionString();
    builder.Services.AddDbContext<AIDbContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddScoped<IBotSettingRepository, BotSettingRepository>();
}
else
{
    Console.WriteLine("[API] Database:Host not configured — settings endpoints are disabled.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["Jwt:Issuer"],
        ValidAudience = Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]))
    };
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors(webAppPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/hello", () => "hello world");

app.MapGet("/ping", [Authorize] () => "pong")
.WithName("Ping");

app.Run();
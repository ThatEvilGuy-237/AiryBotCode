using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AiryBotCode.Infrastructure.Database.Persistence;
using AiryBotCode.Infrastructure.Database.Repository;
using AiryBotCode.Infrastructure.Database.Repository.BotSettings;
using AiryBotCode.Application.Interfaces.Repository;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

var svelteAppPolicy = "SvelteApp";

// Add services to the container.
// Allowed origins are configurable (Cors:AllowedOrigins). Behind the Caddy proxy
// the frontend and API share an origin so CORS is moot, but this keeps local
// dev (vite on :5173) and any future split-origin setup working.
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                  ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: svelteAppPolicy,
                      policy  =>
                      {
                          policy.WithOrigins(corsOrigins)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

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

// Database access for the control panel (shares the bot's CommandSettings table).
builder.Services.AddDbContext<AIDbContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICommandSettingsRepository, CommandSettingsRepository>();
builder.Services.AddScoped<IBotCommandRepository, BotCommandRepository>();
builder.Services.AddScoped<IChannelWebhookRepository, ChannelWebhookRepository>();
builder.Services.AddScoped<IBotSettingRepository, BotSettingRepository>();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// In Development the API owns schema creation. Production normally has the bot
// (which calls AIDbContext.registerDbContext → EnsureCreated) build the schema,
// but the airybotcode-dev stack runs panel+API only (no bot), so its airy_db_dev
// would stay empty. EnsureCreated builds the full model schema on the empty dev
// DB; it's a no-op once the tables exist. Guarded to Development so prod is
// unaffected (prod runs ASPNETCORE_ENVIRONMENT=Production).
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AiryBotCode.Infrastructure.Database.Persistence.AIDbContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            db.Database.EnsureCreated();
            // Also apply the idempotent column/table patches the bot normally runs
            // (the dev stack has no bot) so an existing dev DB gains new columns
            // like BotSettings.ThemeImage/ThemeData.
            AiryBotCode.Infrastructure.Database.Persistence.AIDbContext.EnsureNewerTables(db);
            Console.WriteLine("[DATABASE] Development: ensured schema (EnsureCreated + patches).");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DATABASE] Development schema ensure failed (non-fatal): {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors(svelteAppPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/hello", () => "hello world");

app.MapGet("/ping", [Authorize] () => "pong")
.WithName("Ping");

app.Run();
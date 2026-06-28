using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AiryBotCode.Infrastructure.Database.Persistence;
using AiryBotCode.Infrastructure.Database.Repository;
using AiryBotCode.Infrastructure.Database.Repository.BotSettings;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Services;

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
builder.Services.AddScoped<ISuggestionRepository, SuggestionRepository>();
// The API registers its own DI (it doesn't call RegisterInfrastructure), so the
// public suggestions share-code service must be wired here too.
builder.Services.AddScoped<ISuggestionShareCodeRepository, SuggestionShareCodeRepository>();
builder.Services.AddScoped<IShareCodeService, ShareCodeService>();

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Best-effort rate limit for the PUBLIC (code-gated) suggestion create endpoint —
// blunts spam/abuse; the 256-bit code is the real gate, this just caps frequency.
// Partition by the original client IP: behind Caddy the real client is the first
// X-Forwarded-For hop; fall back to the socket address in dev / no-proxy. (XFF is
// client-settable, so this is mitigation, not a hard boundary — acceptable here.)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("public-suggest-create", httpContext =>
    {
        var xff = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var clientKey = !string.IsNullOrWhiteSpace(xff)
            ? xff.Split(',')[0].Trim()
            : (httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        return RateLimitPartition.GetFixedWindowLimiter(clientKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
        });
    });
});
// Powers the panel's channel/role pickers (reads guild data from Discord with the
// stored bot token). Singleton so its short-lived per-bot cache is shared.
builder.Services.AddSingleton<AiryBotCode.Api.Services.DiscordGuildLookup>();
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
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AiryBotCode.Infrastructure.Database.Persistence.AIDbContext>();
    try
    {
        if (db.Database.CanConnect())
        {
            // Dev has no bot, so the API owns full schema creation there.
            if (app.Environment.IsDevelopment()) db.Database.EnsureCreated();
            // Idempotent CREATE TABLE/ALTER patches (IF NOT EXISTS). Safe to run in
            // BOTH environments — in prod it guarantees panel-only tables (e.g.
            // Suggestions) exist regardless of whether the bot has booted yet.
            AiryBotCode.Infrastructure.Database.Persistence.AIDbContext.EnsureNewerTables(db);
            Console.WriteLine("[DATABASE] Ensured schema patches (EnsureNewerTables).");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DATABASE] Schema ensure failed (non-fatal): {ex.Message}");
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
app.UseRateLimiter();

app.MapControllers();

app.MapGet("/hello", () => "hello world");

app.MapGet("/ping", [Authorize] () => "pong")
.WithName("Ping");

app.Run();
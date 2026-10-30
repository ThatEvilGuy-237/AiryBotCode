using System.Threading.RateLimiting;
using AiryBotCode.Api;
using AiryBotCode.Infrastructure.Database.Persistence;
using Evil.Sdk;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var airyDatabase = config.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not set: Airy's own database, with the airybotcode_app role");
var usersDatabase = config.GetConnectionString("Users")
    ?? throw new InvalidOperationException("ConnectionStrings:Users is not set: the users database connection of this app's own role");
var pathBase = config["PathBase"] ?? "/api";
var proxies = (config.GetSection("Proxies:Addresses").Get<string[]>() ?? []).Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
var clientHeader = config["Proxies:ClientHeader"];
var publicUrl = config["PublicUrl"];
var discord = new DiscordSignIn(config.GetValue<bool>("Discord:SignIn"), config["Discord:ClientId"], config["Discord:ClientSecret"]);
var commit = config["Build:Commit"];
var telemetryEndpoint = config["Telemetry:Endpoint"];
var telemetryProtocol = config["Telemetry:Protocol"];

var userRules = new UserRules(ListOnlyThisApp: true);
var sessionRules = new SessionRules(Browser: true, IdleTimeout: TimeSpan.FromHours(24), AbsoluteLifetime: TimeSpan.FromHours(24));

var evil = builder.AddEvil("airybotcode", "api", "Airy")
    .Commit(commit)
    .Telemetry(telemetryEndpoint, telemetryProtocol)
    .Proxies(proxies)
    .Users(usersDatabase, userRules)
    .Sessions(sessionRules)
    .Audit();
if (discord.On)
    evil.PublicUrl(publicUrl!).Discord(discord.App);

if (!string.IsNullOrWhiteSpace(clientHeader))
    builder.Services.PostConfigure<ForwardedHeadersOptions>(forwarded => forwarded.ForwardedForHeaderName = clientHeader);

builder.Services.AddSingleton(new AiryDatabase(airyDatabase));
builder.Services.AddDbContext<AIDbContext>(options => options.UseNpgsql(airyDatabase));
builder.Services.AddPanelServices();
builder.Services.AddHttpClient();
builder.Services.AddRateLimiter(options => options.AddPolicy(PublicSuggestLimit.Policy, PublicSuggestLimit.For));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AIDbContext>();
    db.Database.EnsureCreated();
    AIDbContext.EnsureNewerTables(db);
}

app.UsePathBase(pathBase);
app.UseRouting();
app.MapEvil();
app.Run();

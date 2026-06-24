using AiryBotCode.SlackBot.SlackEvents;
using AiryBotCode.SlackBot.Spine;
using Microsoft.Extensions.Configuration;
using SlackNet;
using SlackNet.SocketMode;

// ── config (env first, appsettings.json fallback) ───────────────────────────
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

string Require(string key)
{
    var v = config[key] ?? Environment.GetEnvironmentVariable(key.Replace(":", "__"));
    if (string.IsNullOrWhiteSpace(v)) throw new InvalidOperationException($"Missing required config: {key}");
    return v;
}

var botToken = Require("Slack:BotToken");          // xoxb-…
var appToken = Require("Slack:AppToken");          // xapp-…  (Socket Mode)
var spineUrl = config["Spine:Url"] ?? "http://spine:7000";
var agentId  = Require("Vinnie:AgentId");          // the Vinnie agent id
var jwtSecret = Require("Jwt:Secret");             // shared AiryBotCode JWT secret

var http = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
var spine = new SpineAgentClient(http, spineUrl, agentId, jwtSecret);

// ── Slack (Socket Mode) ─────────────────────────────────────────────────────
var slackServices = new SlackServiceBuilder()
    .UseApiToken(botToken)
    .UseAppLevelToken(appToken);

var api = slackServices.GetApiClient();
var auth = await api.Auth.Test();
var botUserId = auth.UserId;
Console.WriteLine($"[Vinnie] authenticated as {auth.User} ({botUserId}) in workspace {auth.Team}");

slackServices.RegisterEventHandler(_ => new VinnieHandler(api, spine, botUserId));

var socket = slackServices.GetSocketModeClient();
await socket.Connect();
Console.WriteLine("[Vinnie] Socket Mode connected — listening for messages.");

await Task.Delay(Timeout.Infinite);

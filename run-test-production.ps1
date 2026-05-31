# run-test-production.ps1
#
# Prep script for AiryBot. Brings up everything the bot DEPENDS on, then
# exits so you can run the bot yourself (debugger, VS, separate terminal,
# etc).
#
# Brings up:
#   1. The bot's own Postgres (docker compose service "db")
#   2. Docker + Mind + Wraith (Chronos is NOT touched — F5 it from VS)
#   3. The Chronos -> Wraith MCP toolbox + tool catalog rebuild so Airy sees
#      the agent_set_name / recall_facts / agent_add_trait / ... tools
#   4. Builds the .NET solution
#
# After this exits, run AiryBot by hand with one of:
#   dotnet run --project Bots\AiryBotCode.AiryBot\AiryBotCode.AiryBot.csproj
#   - or from Visual Studio, F5 on AiryBotCode.AiryBot
#
# Leaves everything running after the script finishes. Stop with:
#   ..\AgentTesting\scripts\services.ps1 -Action down
#   docker compose down            (in this dir, for the bot's db)
#
# ASCII-only on purpose. Windows PowerShell 5.1 mis-decodes em-dashes and box-
# drawing characters in UTF-8 files without a BOM, which breaks parsing of any
# code that comes after them.

[CmdletBinding()]
param(
    [string]$WraithUrl     = "http://localhost:5000",
    [string]$ChronosMcpUrl = "http://localhost:53301/mcp"
)

# Chronos is intentionally NOT managed by this script — F5 it from Visual
# Studio yourself. If the script also did `dotnet run` for Chronos, the two
# builds fought over the DLL locks in bin\Debug\net10.0\ and caused
# half-built / random-stop behavior.

$ErrorActionPreference = "Stop"

# Anchor to the script's own directory so we don't depend on the caller's CWD.
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir
$HiveScripts = Join-Path (Split-Path -Parent $ScriptDir) "AgentTesting\scripts"

function Section([string]$Title) {
    Write-Host ""
    Write-Host "=== $Title ===" -ForegroundColor Cyan
}

# 1. Bot's Postgres
Section "[1/4] Bot Postgres (docker compose: db)"
try {
    docker compose up -d db
    if ($LASTEXITCODE -ne 0) { throw "docker compose returned $LASTEXITCODE" }
} catch {
    Write-Host "!! docker compose failed. Is Docker Desktop running?" -ForegroundColor Yellow
    exit 1
}

# 2. Hive stack
Section "[2/4] Hive stack (docker + Mind + Wraith. Chronos is on you.)"
$svcScript = Join-Path $HiveScripts "services.ps1"
if (-not (Test-Path $svcScript)) {
    Write-Host "!! Could not find $svcScript" -ForegroundColor Yellow
    Write-Host "   Expected AgentTesting to be a sibling of AiryBotCode under The-Hive." -ForegroundColor Yellow
    exit 1
}
Write-Host "[run] docker + Mind + Wraith only. F5 Chronos from VS yourself." -ForegroundColor DarkGray
& $svcScript -Action up -SkipChronos
if ($LASTEXITCODE -ne 0) {
    Write-Host "!! Hive stack failed to start." -ForegroundColor Yellow
    exit 1
}

# 3. Refresh Wraith MCP catalog. Idempotent: repeated POSTs return 400 with
# "already exists" which we treat as success. Then rebuild Wraith's tool list
# so Airy sees the latest agent_* tools.
Section "[3/4] Refresh Wraith MCP tool catalog"
$body = @{
    id        = "chronos"
    label     = "chronos"
    transport = @{ kind = "http"; url = $ChronosMcpUrl }
} | ConvertTo-Json -Depth 5 -Compress

try {
    Invoke-RestMethod -Uri "$WraithUrl/toolboxes/mcp" -Method Post `
        -ContentType "application/json" -Body $body -TimeoutSec 10 | Out-Null
    Write-Host "[refresh] Chronos MCP toolbox registered." -ForegroundColor DarkGray
} catch {
    Write-Host "[refresh] toolbox add: $($_.Exception.Message) (probably already exists, OK)" -ForegroundColor DarkGray
}

try {
    $r = Invoke-RestMethod -Uri "$WraithUrl/tools/rebuild" -Method Post -TimeoutSec 30
    Write-Host "[refresh] Wraith tools rebuilt: $($r.tools) tools available." -ForegroundColor Green
} catch {
    Write-Host "!! tools/rebuild failed: $($_.Exception.Message)" -ForegroundColor Yellow
    exit 1
}

# 4. Build
Section "[4/4] dotnet build"
dotnet build "AiryBotCode.sln" --nologo
if ($LASTEXITCODE -ne 0) {
    Write-Host "!! Build failed." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Ready. Start the bot when you want:" -ForegroundColor Green
Write-Host "  dotnet run --project Bots\AiryBotCode.AiryBot\AiryBotCode.AiryBot.csproj --no-build" -ForegroundColor DarkGray
Write-Host "  or F5 on AiryBotCode.AiryBot from Visual Studio." -ForegroundColor DarkGray
Write-Host ""
Write-Host "Stop services when done:" -ForegroundColor DarkGray
Write-Host "  ..\AgentTesting\scripts\services.ps1 -Action down" -ForegroundColor DarkGray
Write-Host "  docker compose down" -ForegroundColor DarkGray
exit 0

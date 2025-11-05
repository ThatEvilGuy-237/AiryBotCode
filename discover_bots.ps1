# Get the script's directory to build absolute paths
$scriptDir = $PSScriptRoot

# Find all bot projects in the Bots directory
$botProjects = Get-ChildItem -Path (Join-Path $scriptDir "Bots") -Directory

# Start the docker-compose.override.yml content
$overrideContent = @"
services:
"@

# Add a service for each bot project
foreach ($project in $botProjects) {
    $projectName = $project.Name.ToLower()
    $projectPath = "Bots/$($project.Name)"

    $overrideContent += @"
  $projectName:
    extends:
      service: airy-bot-base
    command: dotnet run --project $projectPath
"@
}

# Write the content to the override file
Set-Content -Path (Join-Path $scriptDir "docker-compose.override.yml") -Value $overrideContent

# Run docker-compose
docker-compose up --build

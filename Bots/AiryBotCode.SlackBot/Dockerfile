# Vinnie — Slack bot for the Airy/Hive stack. Build context = this directory
# (the project is self-contained: no project references to the shared libs in v1).
FROM mcr.microsoft.com/dotnet/sdk:10.0.403 AS build
WORKDIR /src
COPY . .
RUN dotnet publish AiryBotCode.SlackBot.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0.14
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AiryBotCode.SlackBot.dll"]

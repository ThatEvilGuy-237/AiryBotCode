# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and all project files to leverage Docker cache
COPY AiryBotCode.sln .
COPY ["AiryBotCode.Application/AiryBotCode.Application.csproj", "AiryBotCode.Application/"]
COPY ["AiryBotCode.Domain/AiryBotCode.Domain.csproj", "AiryBotCode.Domain/"]
COPY ["AiryBotCode.Infrastructure/AiryBotCode.Infrastructure.csproj", "AiryBotCode.Infrastructure/"]
COPY ["AiryBotCode.Launcher/AiryBotCode.Launcher.csproj", "AiryBotCode.Launcher/"]
# Copy all bot project files
COPY ["Bots/AiryBotCode.AiryBot/AiryBotCode.AiryBot.csproj", "Bots/AiryBotCode.AiryBot/"]
COPY ["Bots/AiryBotCode.AiryDevBot/AiryBotCode.AiryDevBot.csproj", "Bots/AiryBotCode.AiryDevBot/"]
COPY ["Bots/AiryBotCode.AiryDevSecondBot/AiryBotCode.AiryDevSecondBot.csproj", "Bots/AiryBotCode.AiryDevSecondBot/"]
COPY ["Bots/AiryBotCode.AiryGuardian/AiryBotCode.AiryGuardian.csproj", "Bots/AiryBotCode.AiryGuardian/"]

# Restore dependencies for the entire solution
RUN dotnet restore "AiryBotCode.sln"

# Copy the rest of the source code
COPY . .

# Build and publish the specific bot projects needed for the final image
RUN dotnet publish "Bots/AiryBotCode.AiryBot/AiryBotCode.AiryBot.csproj" -c Release -o /app/publish/AiryBot
RUN dotnet publish "Bots/AiryBotCode.AiryGuardian/AiryBotCode.AiryGuardian.csproj" -c Release -o /app/publish/AiryGuardian

# Use the official .NET 8 runtime image for the final, smaller image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy ALL published output from the build stage
COPY --from=build /app/publish .

# The entrypoint will be specified by the 'command' in the docker-compose file
ENTRYPOINT ["dotnet"]
# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and restore as distinct layers
COPY AiryBotCode.sln ./
COPY AiryBotCode.Application/AiryBotCode.Application.csproj AiryBotCode.Application/
COPY AiryBotCode.Domain/AiryBotCode.Domain.csproj AiryBotCode.Domain/
COPY AiryBotCode.Infrastructure/AiryBotCode.Infrastructure.csproj AiryBotCode.Infrastructure/
COPY AiryBotCode.Bot/AiryBotCode.Bot.csproj AiryBotCode.Bot/

RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet publish AiryBotCode.Bot/AiryBotCode.Bot.csproj -c Release -o /app/publish

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose any ports if needed (uncomment if your bot uses a web server)
# EXPOSE 5000

# Set environment variables if needed
# ENV ASPNETCORE_ENVIRONMENT=Production

# Run the bot
ENTRYPOINT ["dotnet", "AiryBotCode.Bot.dll"]
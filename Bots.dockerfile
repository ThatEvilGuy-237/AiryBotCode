# Stage 1: Build the applications
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all .csproj files and the .sln file first to leverage Docker layer caching
COPY AiryBotCode.sln .
COPY AiryBotCode.Application/*.csproj ./AiryBotCode.Application/
COPY AiryBotCode.Domain/*.csproj ./AiryBotCode.Domain/
COPY AiryBotCode.Infrastructure/*.csproj ./AiryBotCode.Infrastructure/
COPY Bots/AiryBotCode.AiryBot/*.csproj ./Bots/AiryBotCode.AiryBot/
COPY Bots/AiryBotCode.AiryDevBot/*.csproj ./Bots/AiryBotCode.AiryDevBot/
COPY Bots/AiryBotCode.AiryDevSecondBot/*.csproj ./Bots/AiryBotCode.AiryDevSecondBot/
COPY Bots/AiryBotCode.AiryGuardian/*.csproj ./Bots/AiryBotCode.AiryGuardian/

# Restore dependencies for the entire solution
RUN dotnet restore "AiryBotCode.sln"

# Copy the rest of the source code
COPY . .

# Publish each bot application into a separate folder
RUN dotnet publish "Bots/AiryBotCode.AiryBot/AiryBotCode.AiryBot.csproj" -c Release -o /app/publish/AiryBot
RUN dotnet publish "Bots/AiryBotCode.AiryGuardian/AiryBotCode.AiryGuardian.csproj" -c Release -o /app/publish/AiryGuardian
RUN dotnet publish "Bots/AiryBotCode.AiryDevBot/AiryBotCode.AiryDevBot.csproj" -c Release -o /app/publish/AiryDevBot
RUN dotnet publish "Bots/AiryBotCode.AiryDevSecondBot/AiryBotCode.AiryDevSecondBot.csproj" -c Release -o /app/publish/AiryDevSecondBot

# Stage 2: Create the final, lean runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy all the published bot applications from the build stage
COPY --from=build /app/publish .

# The ENTRYPOINT is the base command. The specific DLL to run will be provided by docker-compose.
ENTRYPOINT ["dotnet"]

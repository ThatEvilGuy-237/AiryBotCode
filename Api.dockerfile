# Build and run the control-panel API (ASP.NET).
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY . .
RUN dotnet restore AiryBotCode.Api/AiryBotCode.Api.csproj
RUN dotnet publish AiryBotCode.Api/AiryBotCode.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
# Listen on a fixed internal port; Caddy reverse-proxies to it.
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "AiryBotCode.Api.dll"]

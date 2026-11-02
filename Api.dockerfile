FROM mcr.microsoft.com/dotnet/sdk:10.0.403 AS build
WORKDIR /source
COPY --from=evil-feed . /feed
COPY . .
RUN sed -i 's#value="/mnt/hdd/projects/evil-sdk-data/feed"#value="/feed"#' nuget.config && grep -q 'value="/feed"' nuget.config
RUN dotnet restore AiryBotCode.Api/AiryBotCode.Api.csproj
RUN dotnet publish AiryBotCode.Api/AiryBotCode.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0.14
WORKDIR /app
COPY --from=build /app .
ARG COMMIT=""
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    Build__Commit=$COMMIT
EXPOSE 8080
ENTRYPOINT ["dotnet", "AiryBotCode.Api.dll"]

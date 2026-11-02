FROM mcr.microsoft.com/dotnet/sdk:10.0.403 AS build
ARG PROJECT_PATH
ARG SLN_PATH
WORKDIR /source
COPY --from=evil-feed . /feed
COPY . .
RUN sed -i 's#value="/mnt/hdd/projects/evil-sdk-data/feed"#value="/feed"#' nuget.config && grep -q 'value="/feed"' nuget.config
RUN dotnet restore ${SLN_PATH}
RUN dotnet publish ${PROJECT_PATH} -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0.14
ARG DLL_NAME
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "AiryBotCode.AiryDevBot.dll"]

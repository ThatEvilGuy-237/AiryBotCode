# Use the .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJECT_PATH
ARG SLN_PATH
WORKDIR /source

# Copy the solution file
COPY ${SLN_PATH} .

# Copy all project directories and their contents
COPY . .

# Restore dependencies for the entire solution
RUN dotnet restore ${SLN_PATH}

# Build and publish the specified project within the context of the solution
RUN dotnet publish ${PROJECT_PATH} -c Release -o /app --no-restore

# Use the .NET 8 runtime image to run the application
FROM mcr.microsoft.com/dotnet/runtime:8.0
ARG DLL_NAME
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", DLL_NAME]
FROM mcr.microsoft.com/dotnet/runtime:8.0
ARG DLL_NAME
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", DLL_NAME]

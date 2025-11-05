# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the solution
RUN dotnet build -c Release

# Set the entrypoint
ENTRYPOINT ["dotnet"]

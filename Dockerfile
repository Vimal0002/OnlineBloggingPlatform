# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OnlineBloggingPlatform.csproj", "."]
RUN dotnet restore "./OnlineBloggingPlatform.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "OnlineBloggingPlatform.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OnlineBloggingPlatform.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Database will be created automatically by the application if it doesn't exist
# The seeding logic in Program.cs will handle initial data

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "OnlineBloggingPlatform.dll"]

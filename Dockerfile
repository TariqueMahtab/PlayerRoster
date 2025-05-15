# Use official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY *.sln .
COPY PlayerRoster.Server/*.csproj ./PlayerRoster.Server/
RUN dotnet restore

# Copy everything else and build
COPY PlayerRoster.Server/. ./PlayerRoster.Server/
WORKDIR /app/PlayerRoster.Server
RUN dotnet publish -c Release -o out

# Use official ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/PlayerRoster.Server/out ./
ENTRYPOINT ["dotnet", "PlayerRoster.Server.dll"]

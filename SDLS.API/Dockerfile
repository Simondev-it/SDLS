# Base image (Linux)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["SDLS.API/SDLS.API.csproj", "SDLS.API/"]
COPY ["SDLS.Services/SDLS.Services.csproj", "SDLS.Services/"]
COPY ["SDLS.Repositories/SDLS.Repositories.csproj", "SDLS.Repositories/"]
COPY ["SDLS.Model/SDLS.Model.csproj", "SDLS.Model/"]

RUN dotnet restore "./SDLS.API/SDLS.API.csproj"

COPY . .
WORKDIR "/src/SDLS.API"
RUN dotnet build "SDLS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SDLS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SDLS.API.dll"]
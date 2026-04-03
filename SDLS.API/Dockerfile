# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["SDLS.API/SDLS.API.csproj", "SDLS.API/"]
RUN dotnet restore "SDLS.API/SDLS.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/SDLS.API"
RUN dotnet publish "SDLS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SDLS.API.dll"]
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SDLS.API/SDLS.API.csproj", "SDLS.API/"]
COPY ["SDLS.Repositories/SDLS.Repositories.csproj", "SDLS.Repositories/"]
COPY ["SDLS.Model/SDLS.Model.csproj", "SDLS.Model/"]

RUN dotnet restore "SDLS.API/SDLS.API.csproj"

COPY . .
WORKDIR "/src/SDLS.API"

RUN dotnet publish "SDLS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SDLS.API.dll"]
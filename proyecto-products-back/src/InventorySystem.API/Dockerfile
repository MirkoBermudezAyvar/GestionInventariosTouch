FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/InventorySystem.API/InventorySystem.API.csproj", "InventorySystem.API/"]
COPY ["src/InventorySystem.Application/InventorySystem.Application.csproj", "InventorySystem.Application/"]
COPY ["src/InventorySystem.Domain/InventorySystem.Domain.csproj", "InventorySystem.Domain/"]
COPY ["src/InventorySystem.Infrastructure/InventorySystem.Infrastructure.csproj", "InventorySystem.Infrastructure/"]
RUN dotnet restore "InventorySystem.API/InventorySystem.API.csproj"

COPY src/ .
WORKDIR "/src/InventorySystem.API"
RUN dotnet build "InventorySystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventorySystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventorySystem.API.dll"]

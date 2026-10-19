FROM mcr.microsoft.com/dotnet/sdk:10.0.402 AS build
WORKDIR /src
COPY global.json ./
COPY src/Shared.Contracts/Shared.Contracts.csproj src/Shared.Contracts/
COPY src/Telemetry.Api/Telemetry.Api.csproj src/Telemetry.Api/
RUN dotnet restore src/Telemetry.Api/Telemetry.Api.csproj
COPY src/Shared.Contracts/ src/Shared.Contracts/
COPY src/Telemetry.Api/ src/Telemetry.Api/
RUN dotnet publish src/Telemetry.Api/Telemetry.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0.13
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Telemetry.Api.dll"]

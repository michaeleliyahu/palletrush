FROM mcr.microsoft.com/dotnet/sdk:10.0.402 AS build
WORKDIR /src
COPY global.json ./
COPY src/Shared.Contracts/Shared.Contracts.csproj src/Shared.Contracts/
COPY src/League.Api/League.Api.csproj src/League.Api/
RUN dotnet restore src/League.Api/League.Api.csproj
COPY src/Shared.Contracts/ src/Shared.Contracts/
COPY src/League.Api/ src/League.Api/
RUN dotnet publish src/League.Api/League.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0.13
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "League.Api.dll"]

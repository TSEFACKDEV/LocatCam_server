# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copier les fichiers de projet pour restaurer les dépendances
COPY LocatCam.slnx ./
COPY LocatCam.Api/LocatCam.Api.csproj LocatCam.Api/
COPY LocatCam.Core/LocatCam.Core.csproj LocatCam.Core/
COPY LocatCam.Infrastructure/LocatCam.Infrastructure.csproj LocatCam.Infrastructure/
COPY LocatCam.Tests/LocatCam.Tests.csproj LocatCam.Tests/

RUN dotnet restore LocatCam.Api/LocatCam.Api.csproj

# Copier le reste du code source
COPY . .

# Publier l'API
WORKDIR /src/LocatCam.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Créer le répertoire des logs
RUN mkdir -p /app/logs

COPY --from=build /app/publish .

# Port exposé par l'API ASP.NET Core
EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "LocatCam.Api.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
LABEL org.opencontainers.image.source="https://github.com/patheon-temple/kronos"
LABEL org.opencontainers.image.description="Kronos API"
LABEL org.opencontainers.image.licenses="MIT"
USER $APP_UID

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything else and build the application
COPY ./Kronos.WebAPI ./Kronos.WebAPI
COPY ./Athena.SDK ./Athena.SDK
COPY ./Hermes.SDK ./Hermes.SDK
WORKDIR "/src/Kronos.WebAPI"
RUN dotnet build --configuration Release -o /app/build

# Publish the app to a folder
FROM build AS publish
RUN dotnet publish --configuration Release --no-restore -o /app/publish

# Use the runtime image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kronos.WebAPI.dll"]
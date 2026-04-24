#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# Add 'contrib' component (needed for ttf-mscorefonts-installer) and install fonts
# Handles both DEB822 format (trixie+) and traditional sources.list (bookworm)
RUN if [ -f /etc/apt/sources.list.d/debian.sources ]; then \
        sed -i 's/^Components: main$/Components: main contrib/' /etc/apt/sources.list.d/debian.sources; \
    elif [ -f /etc/apt/sources.list ]; then \
        sed -i '/contrib/!s/main/main contrib/' /etc/apt/sources.list; \
    fi && \
    apt-get update && \
    apt-get install -y --no-install-recommends \
        ca-certificates \
        openssl \
        ttf-mscorefonts-installer \
        fonts-noto-cjk \
        fontconfig && \
    update-ca-certificates --fresh && \
    rm -rf /var/lib/apt/lists/*



# Generate a self-signed certificate
RUN mkdir -p /app/https && \
    openssl req -x509 -newkey rsa:4096 -sha256 -days 3650 -nodes \
    -keyout /app/https/private.key -out /app/https/certificate.crt \
    -subj "/C=US/ST=State/L=City/O=Organization/CN=localhost" && \
    openssl pkcs12 -export -out /app/https/aspnetapp.pfx \
    -inkey /app/https/private.key -in /app/https/certificate.crt \
    -password pass:CREDENTIAL_PLACEHOLDER


# Setup environment variables for the application to find the certificate
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="CREDENTIAL_PLACEHOLDER"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/app/https/aspnetapp.pfx"

WORKDIR /app
EXPOSE 80
EXPOSE 443



FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Migrators/Migrators.MSSQL/Migrators.MSSQL.csproj", "src/Migrators/Migrators.MSSQL/"]
COPY ["src/Migrators/Migrators.PostgreSQL/Migrators.PostgreSQL.csproj", "src/Migrators/Migrators.PostgreSQL/"]
COPY ["src/Migrators/Migrators.SqLite/Migrators.SqLite.csproj", "src/Migrators/Migrators.SqLite/"]

COPY ["src/Server.UI/Server.UI.csproj", "src/Server.UI/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Server.UI/Server.UI.csproj"
COPY . .

WORKDIR "/src/src/Server.UI"
RUN dotnet add package SkiaSharp.NativeAssets.Linux.NoDependencies
RUN dotnet add package HarfBuzzSharp.NativeAssets.Linux
RUN dotnet build "Server.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Server.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CleanArchitecture.Blazor.Server.UI.dll"]
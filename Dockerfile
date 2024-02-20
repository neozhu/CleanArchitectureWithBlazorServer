#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Migrators/Migrators.MSSQL/Migrators.MSSQL.csproj", "src/Migrators/Migrators.MSSQL/"]
COPY ["src/Migrators/Migrators.PostgreSQL/Migrators.PostgreSQL.csproj", "src/Migrators/Migrators.PostgreSQL/"]
COPY ["src/Migrators/Migrators.SqLite/Migrators.SqLite.csproj", "src/Migrators/Migrators.SqLite/"]

COPY ["src/Server.UI/Server.UI.csproj", "src/Server.UI/"]
COPY ["src/Server/Server.csproj", "src/Server/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Server.UI/Server.UI.csproj"
COPY . .
WORKDIR "/src/src/Server.UI"
RUN dotnet build "Server.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Server.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "CleanArchitecture.Blazor.Server.UI.dll"]

# Build context: repository root (docker build -f IdentityServer/Store.IdentityServer/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["IdentityServer/Store.IdentityServer/Store.IdentityServer.csproj", "IdentityServer/Store.IdentityServer/"]
RUN dotnet restore "IdentityServer/Store.IdentityServer/Store.IdentityServer.csproj"
COPY ["IdentityServer/Store.IdentityServer/", "IdentityServer/Store.IdentityServer/"]
RUN dotnet publish "IdentityServer/Store.IdentityServer/Store.IdentityServer.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.IdentityServer.dll"]

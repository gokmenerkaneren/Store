# Build context: repository root (docker build -f ApiGateways/Store.Gateway/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["ApiGateways/Store.Gateway/Store.Gateway.csproj", "ApiGateways/Store.Gateway/"]
RUN dotnet restore "ApiGateways/Store.Gateway/Store.Gateway.csproj"
COPY ["ApiGateways/Store.Gateway/", "ApiGateways/Store.Gateway/"]
RUN dotnet publish "ApiGateways/Store.Gateway/Store.Gateway.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Gateway.dll"]

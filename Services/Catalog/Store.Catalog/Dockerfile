# Build context: repository root (docker build -f Services/Catalog/Store.Catalog/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Catalog/Store.Catalog/Store.Catalog.csproj", "Services/Catalog/Store.Catalog/"]
RUN dotnet restore "Services/Catalog/Store.Catalog/Store.Catalog.csproj"
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Catalog/Store.Catalog/", "Services/Catalog/Store.Catalog/"]
RUN dotnet publish "Services/Catalog/Store.Catalog/Store.Catalog.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Catalog.dll"]

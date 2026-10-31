# Build context: repository root (docker build -f Services/Favorite/Presentation/Store.Favorite.WebApi/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Services/Favorite/Core/Store.Favorite.Domain/Store.Favorite.Domain.csproj", "Services/Favorite/Core/Store.Favorite.Domain/"]
COPY ["Services/Favorite/Core/Store.Favorite.Application/Store.Favorite.Application.csproj", "Services/Favorite/Core/Store.Favorite.Application/"]
COPY ["Services/Favorite/Infrastructure/Store.Favorite.Infrastructure/Store.Favorite.Infrastructure.csproj", "Services/Favorite/Infrastructure/Store.Favorite.Infrastructure/"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Favorite/Presentation/Store.Favorite.WebApi/Store.Favorite.WebApi.csproj", "Services/Favorite/Presentation/Store.Favorite.WebApi/"]
RUN dotnet restore "Services/Favorite/Presentation/Store.Favorite.WebApi/Store.Favorite.WebApi.csproj"
COPY ["Services/Favorite/Core/Store.Favorite.Domain/", "Services/Favorite/Core/Store.Favorite.Domain/"]
COPY ["Services/Favorite/Core/Store.Favorite.Application/", "Services/Favorite/Core/Store.Favorite.Application/"]
COPY ["Services/Favorite/Infrastructure/Store.Favorite.Infrastructure/", "Services/Favorite/Infrastructure/Store.Favorite.Infrastructure/"]
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Favorite/Presentation/Store.Favorite.WebApi/", "Services/Favorite/Presentation/Store.Favorite.WebApi/"]
RUN dotnet publish "Services/Favorite/Presentation/Store.Favorite.WebApi/Store.Favorite.WebApi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Favorite.WebApi.dll"]

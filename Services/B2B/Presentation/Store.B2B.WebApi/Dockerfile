# Build context: repository root (docker build -f Services/B2B/Presentation/Store.B2B.WebApi/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Services/B2B/Core/Store.B2B.Domain/Store.B2B.Domain.csproj", "Services/B2B/Core/Store.B2B.Domain/"]
COPY ["Services/B2B/Core/Store.B2B.Application/Store.B2B.Application.csproj", "Services/B2B/Core/Store.B2B.Application/"]
COPY ["Services/B2B/Infrastructure/Store.B2B.Persistence/Store.B2B.Persistence.csproj", "Services/B2B/Infrastructure/Store.B2B.Persistence/"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/B2B/Presentation/Store.B2B.WebApi/Store.B2B.WebApi.csproj", "Services/B2B/Presentation/Store.B2B.WebApi/"]
RUN dotnet restore "Services/B2B/Presentation/Store.B2B.WebApi/Store.B2B.WebApi.csproj"
COPY ["Services/B2B/Core/Store.B2B.Domain/", "Services/B2B/Core/Store.B2B.Domain/"]
COPY ["Services/B2B/Core/Store.B2B.Application/", "Services/B2B/Core/Store.B2B.Application/"]
COPY ["Services/B2B/Infrastructure/Store.B2B.Persistence/", "Services/B2B/Infrastructure/Store.B2B.Persistence/"]
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/B2B/Presentation/Store.B2B.WebApi/", "Services/B2B/Presentation/Store.B2B.WebApi/"]
RUN dotnet publish "Services/B2B/Presentation/Store.B2B.WebApi/Store.B2B.WebApi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.B2B.WebApi.dll"]

# Build context: repository root (docker build -f Services/Location/Presentation/Store.Location.WebApi/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Services/Location/Core/Store.Location.Domain/Store.Location.Domain.csproj", "Services/Location/Core/Store.Location.Domain/"]
COPY ["Services/Location/Core/Store.Location.Application/Store.Location.Application.csproj", "Services/Location/Core/Store.Location.Application/"]
COPY ["Services/Location/Infrastructure/Store.Location.Persistence/Store.Location.Persistence.csproj", "Services/Location/Infrastructure/Store.Location.Persistence/"]
COPY ["Store.DtoLayer/Store.DtoLayer.csproj", "Store.DtoLayer/"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Location/Presentation/Store.Location.WebApi/Store.Location.WebApi.csproj", "Services/Location/Presentation/Store.Location.WebApi/"]
RUN dotnet restore "Services/Location/Presentation/Store.Location.WebApi/Store.Location.WebApi.csproj"
COPY ["Services/Location/Core/Store.Location.Domain/", "Services/Location/Core/Store.Location.Domain/"]
COPY ["Services/Location/Core/Store.Location.Application/", "Services/Location/Core/Store.Location.Application/"]
COPY ["Services/Location/Infrastructure/Store.Location.Persistence/", "Services/Location/Infrastructure/Store.Location.Persistence/"]
COPY ["Store.DtoLayer/", "Store.DtoLayer/"]
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Location/Presentation/Store.Location.WebApi/", "Services/Location/Presentation/Store.Location.WebApi/"]
RUN dotnet publish "Services/Location/Presentation/Store.Location.WebApi/Store.Location.WebApi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Location.WebApi.dll"]

# Build context: repository root (docker build -f Services/Order/Presentation/Store.Order.WebApi/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Services/Order/Core/Store.Order.Domain/Store.Order.Domain.csproj", "Services/Order/Core/Store.Order.Domain/"]
COPY ["Services/Order/Core/Store.Order.Application/Store.Order.Application.csproj", "Services/Order/Core/Store.Order.Application/"]
COPY ["Services/Order/Core/Infrastructure/Store.Order.Persistence/Store.Order.Persistence.csproj", "Services/Order/Core/Infrastructure/Store.Order.Persistence/"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Order/Presentation/Store.Order.WebApi/Store.Order.WebApi.csproj", "Services/Order/Presentation/Store.Order.WebApi/"]
RUN dotnet restore "Services/Order/Presentation/Store.Order.WebApi/Store.Order.WebApi.csproj"
COPY ["Services/Order/Core/Store.Order.Domain/", "Services/Order/Core/Store.Order.Domain/"]
COPY ["Services/Order/Core/Store.Order.Application/", "Services/Order/Core/Store.Order.Application/"]
COPY ["Services/Order/Core/Infrastructure/Store.Order.Persistence/", "Services/Order/Core/Infrastructure/Store.Order.Persistence/"]
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Order/Presentation/Store.Order.WebApi/", "Services/Order/Presentation/Store.Order.WebApi/"]
RUN dotnet publish "Services/Order/Presentation/Store.Order.WebApi/Store.Order.WebApi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Order.WebApi.dll"]

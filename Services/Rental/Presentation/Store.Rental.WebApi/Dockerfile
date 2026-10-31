# Build context: repository root (docker build -f Services/Rental/Presentation/Store.Rental.WebApi/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Services/Rental/Core/Store.Rental.Domain/Store.Rental.Domain.csproj", "Services/Rental/Core/Store.Rental.Domain/"]
COPY ["Services/Rental/Core/Store.Rental.Application/Store.Rental.Application.csproj", "Services/Rental/Core/Store.Rental.Application/"]
COPY ["Services/Rental/Infrastructure/Store.Rental.Persistence/Store.Rental.Persistence.csproj", "Services/Rental/Infrastructure/Store.Rental.Persistence/"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Rental/Presentation/Store.Rental.WebApi/Store.Rental.WebApi.csproj", "Services/Rental/Presentation/Store.Rental.WebApi/"]
RUN dotnet restore "Services/Rental/Presentation/Store.Rental.WebApi/Store.Rental.WebApi.csproj"
COPY ["Services/Rental/Core/Store.Rental.Domain/", "Services/Rental/Core/Store.Rental.Domain/"]
COPY ["Services/Rental/Core/Store.Rental.Application/", "Services/Rental/Core/Store.Rental.Application/"]
COPY ["Services/Rental/Infrastructure/Store.Rental.Persistence/", "Services/Rental/Infrastructure/Store.Rental.Persistence/"]
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Rental/Presentation/Store.Rental.WebApi/", "Services/Rental/Presentation/Store.Rental.WebApi/"]
RUN dotnet publish "Services/Rental/Presentation/Store.Rental.WebApi/Store.Rental.WebApi.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Rental.WebApi.dll"]

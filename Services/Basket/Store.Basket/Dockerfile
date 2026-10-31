# Build context: repository root (docker build -f Services/Basket/Store.Basket/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Store.DtoLayer/Store.DtoLayer.csproj", "Store.DtoLayer/"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Basket/Store.Basket/Store.Basket.csproj", "Services/Basket/Store.Basket/"]
RUN dotnet restore "Services/Basket/Store.Basket/Store.Basket.csproj"
COPY ["Store.DtoLayer/", "Store.DtoLayer/"]
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Basket/Store.Basket/", "Services/Basket/Store.Basket/"]
RUN dotnet publish "Services/Basket/Store.Basket/Store.Basket.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Basket.dll"]

# Build context: repository root (docker build -f Services/Discount/Store.Discount/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Discount/Store.Discount/Store.Discount.csproj", "Services/Discount/Store.Discount/"]
RUN dotnet restore "Services/Discount/Store.Discount/Store.Discount.csproj"
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Discount/Store.Discount/", "Services/Discount/Store.Discount/"]
RUN dotnet publish "Services/Discount/Store.Discount/Store.Discount.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Discount.dll"]

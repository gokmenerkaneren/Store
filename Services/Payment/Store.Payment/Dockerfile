# Build context: repository root (docker build -f Services/Payment/Store.Payment/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["Services/Payment/Store.Payment/Store.Payment.csproj", "Services/Payment/Store.Payment/"]
RUN dotnet restore "Services/Payment/Store.Payment/Store.Payment.csproj"
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["Services/Payment/Store.Payment/", "Services/Payment/Store.Payment/"]
RUN dotnet publish "Services/Payment/Store.Payment/Store.Payment.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.Payment.dll"]

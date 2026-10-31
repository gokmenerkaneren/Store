# Build context: repository root (docker build -f FrontEnds/Store.WebUI/Dockerfile .)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["Store.DtoLayer/Store.DtoLayer.csproj", "Store.DtoLayer/"]
COPY ["Libraries/Store.Shared/Store.Shared.csproj", "Libraries/Store.Shared/"]
COPY ["FrontEnds/Store.WebUI/Store.WebUI.csproj", "FrontEnds/Store.WebUI/"]
RUN dotnet restore "FrontEnds/Store.WebUI/Store.WebUI.csproj"
COPY ["Store.DtoLayer/", "Store.DtoLayer/"]
COPY ["Libraries/Store.Shared/", "Libraries/Store.Shared/"]
COPY ["FrontEnds/Store.WebUI/", "FrontEnds/Store.WebUI/"]
RUN dotnet publish "FrontEnds/Store.WebUI/Store.WebUI.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.WebUI.dll"]

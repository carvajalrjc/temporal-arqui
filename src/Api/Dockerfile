# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
# restaurar sobre la solución (más fiable)
RUN dotnet restore MiniFacturacion.sln
RUN dotnet publish src/Api/MiniFacturacion.Api.csproj -c Release -o /out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MiniFacturacion.Api.dll"]


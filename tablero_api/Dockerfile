# Imagen base con el SDK de .NET 8 para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# Copiar el csproj y restaurar dependencias
COPY *.csproj .
RUN dotnet restore

# Copiar todo el código y compilar en Release
RUN dotnet publish -c Release -o /app

# Imagen más ligera con ASP.NET Runtime para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "tablero_api.dll"]

# 1. Etapa de compilación 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar las dependencias (NuGet)
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y compilar la API en modo Release
COPY . ./
RUN dotnet publish -c Release -o /app

# 2. Etapa de ejecución 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

# Exponer el puerto que usa Render por defecto
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Comando para arrancar tu API cuando el servidor encienda
ENTRYPOINT ["dotnet", "FutbolStatsWithFriends.dll"]
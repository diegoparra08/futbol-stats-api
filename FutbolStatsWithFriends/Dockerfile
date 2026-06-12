# Etapa de compilación (SDK de .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "FutbolStatsWithFriends.csproj"

# Compilar la API en modo Release
RUN dotnet publish "FutbolStatsWithFriends.csproj" -c Release -o /app

# Etapa de ejecución (Runtime de .NET 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FutbolStatsWithFriends.dll"]
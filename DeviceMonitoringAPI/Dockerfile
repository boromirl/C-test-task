# Официальный .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимоси
COPY ["DeviceMonitoringAPI.csproj", "."]
RUN dotnet restore "DeviceMonitoringAPI.csproj"

# Копируем все остальные файлы и собираем проект
COPY . .
RUN dotnet publish "DeviceMonitoringAPI.csproj" -c Release -o /app/publish

# Используем ASP.NET runtime image в качестве финальноего образа
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "DeviceMonitoringAPI.dll" ]
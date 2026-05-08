# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# نسخ كل المشروع
COPY . .

# Restore
RUN dotnet restore

# Publish (Release)
RUN dotnet publish -c Release -o /app/publish

# =========================
# Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# ⚠️ غيّر اسم الـ DLL حسب مشروعك
ENTRYPOINT ["dotnet", "Se7ety.Api.dll"]

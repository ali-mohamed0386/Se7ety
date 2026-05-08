# =========================
# Stage 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملفات المشروع
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish (Release)
RUN dotnet publish -c Release -o /app/publish

# =========================
# Stage 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# نسخ الناتج من مرحلة البناء
COPY --from=build /app/publish .

# تشغيل المشروع
# ⚠️ غيّر اسم الـ DLL حسب مشروعك
ENTRYPOINT ["dotnet", "Se7ety.Api.dll"]

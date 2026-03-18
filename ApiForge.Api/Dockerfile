# ─── Stage 1: build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["ApiForge.Api/ApiForge.Api.csproj",             "ApiForge.Api/"]
COPY ["ApiForge.Application/ApiForge.Application.csproj", "ApiForge.Application/"]
COPY ["ApiForge.Domain/ApiForge.Domain.csproj",       "ApiForge.Domain/"]
COPY ["ApiForge.Infrastructure/ApiForge.Infrastructure.csproj", "ApiForge.Infrastructure/"]

RUN dotnet restore "ApiForge.Api/ApiForge.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ApiForge.Api"
RUN dotnet build "ApiForge.Api.csproj" -c Release -o /app/build --no-restore

# ─── Stage 2: publish ─────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "ApiForge.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ─── Stage 3: runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# Run as non-root for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
WORKDIR /app

COPY --from=publish /app/publish .

# Ensure log directory exists and is writable
RUN mkdir -p logs && chown appuser:appgroup logs

USER appuser

EXPOSE 8080
EXPOSE 8443

ENV ASPNETCORE_URLS="http://+:8080"
ENV DOTNET_RUNNING_IN_CONTAINER=true

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget -qO- http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ApiForge.Api.dll"]

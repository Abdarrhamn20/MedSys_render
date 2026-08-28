FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Backend/Backend.csproj", "Backend/"]
RUN dotnet restore "Backend/Backend.csproj"

COPY Backend/ ./Backend/
RUN dotnet publish "Backend/Backend.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends postgresql-client \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
COPY scripts/import.sql ./import.sql
COPY scripts/entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh && sed -i 's/\r$//' ./entrypoint.sh
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["/app/entrypoint.sh"]
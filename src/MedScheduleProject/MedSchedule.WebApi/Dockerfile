# Debug phase (used by Visual Studio in fast mode)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install certificates and tools in build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MedSchedule.WebApi/MedSchedule.WebApi.csproj", "MedSchedule.WebApi/"]
RUN dotnet restore "./MedSchedule.WebApi/MedSchedule.WebApi.csproj"

# Generate and trust dev cert (for development)
RUN dotnet dev-certs https && \
    dotnet dev-certs https --trust && \
    mkdir -p /root/.aspnet/https && \
    dotnet dev-certs https -ep /root/.aspnet/https/aspnetapp.pfx -p devcertpassword

COPY . .
WORKDIR "/src/MedSchedule.WebApi"
RUN dotnet build "./MedSchedule.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MedSchedule.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configure Data Protection to use persistent storage
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=devcertpassword
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_DataProtection__StorageLocation=/app/dataprotection

RUN mkdir -p /app/dataprotection && \
    mkdir -p /https && \
    chown app:app /app/dataprotection && \
    chown app:app /https

COPY --from=build /root/.aspnet/https/aspnetapp.pfx /https/aspnetapp.pfx

ENTRYPOINT ["dotnet", "MedSchedule.WebApi.dll"]
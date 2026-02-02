FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pharma.Identity.API/Pharma.Identity.API.csproj", "Pharma.Identity.API/"]
COPY ["Pharma.Identity.Infrastructure/Pharma.Identity.Infrastructure.csproj", "Pharma.Identity.Infrastructure/"]
COPY ["Pharma.Identity.Application/Pharma.Identity.Application.csproj", "Pharma.Identity.Application/"]
COPY ["Pharma.Identity.Domain/Pharma.Identity.Domain.csproj", "Pharma.Identity.Domain/"]
RUN dotnet restore "Pharma.Identity.API/Pharma.Identity.API.csproj"
COPY . .
WORKDIR "/src/Pharma.Identity.API"
RUN dotnet build "./Pharma.Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pharma.Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pharma.Identity.API.dll"]

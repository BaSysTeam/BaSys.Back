#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BaSys.Host/BaSys.Host.csproj", "src/BaSys.Host/"]
COPY ["src/BaSys.Admin/BaSys.Admin.csproj", "src/BaSys.Admin/"]
COPY ["src/BaSys.Shared/BaSys.Shared.csproj", "src/BaSys.Shared/"]
COPY ["src/BaSys.Host.Infrastructure/BaSys.Host.Infrastructure.csproj", "src/BaSys.Host.Infrastructure/"]
COPY ["src/BaSys.SuperAdmin.DAL/BaSys.SuperAdmin.DAL.csproj", "src/BaSys.SuperAdmin.DAL/"]
COPY ["src/BaSys.Host.DAL/BaSys.Host.DAL.csproj", "src/BaSys.Host.DAL/"]
COPY ["src/BaSys.App/BaSys.App.csproj", "src/BaSys.App/"]
COPY ["src/BaSys.Constructor/BaSys.Constructor.csproj", "src/BaSys.Constructor/"]
COPY ["src/BaSys.SuperAdmin/BaSys.SuperAdmin.csproj", "src/BaSys.SuperAdmin/"]
COPY ["src/BaSys.Translation/BaSys.Translation.csproj", "src/BaSys.Translation/"]
RUN dotnet restore "./src/BaSys.Host/BaSys.Host.csproj"
COPY . .
WORKDIR "/src/src/BaSys.Host"
RUN dotnet build "./BaSys.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaSys.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaSys.Host.dll"]
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BaSys.Host/BaSys.Host.csproj", "src/BaSys.Host/"]
COPY ["src/BaSys.Admin/BaSys.Admin.csproj", "src/BaSys.Admin/"]
COPY ["src/BaSys.Core/BaSys.Core.csproj", "src/BaSys.Core/"]
COPY ["src/BaSys.Core.Services/BaSys.Core.Features.csproj", "src/BaSys.Core.Services/"]
COPY ["src/BaSys.DTO/BaSys.DTO.csproj", "src/BaSys.DTO/"]
COPY ["src/BaSys.DAL.Models/BaSys.DAL.Models.csproj", "src/BaSys.DAL.Models/"]
COPY ["src/BaSys.Metadata/BaSys.Metadata.csproj", "src/BaSys.Metadata/"]
COPY ["src/BaSys.Shared/BaSys.Shared.csproj", "src/BaSys.Shared/"]
COPY ["src/BaSys.Translation/BaSys.Translation.csproj", "src/BaSys.Translation/"]
COPY ["src/BaSys.Host.DAL/BaSys.Host.DAL.csproj", "src/BaSys.Host.DAL/"]
COPY ["src/BaSys.FluentQueries/BaSys.FluentQueries.csproj", "src/BaSys.FluentQueries/"]
COPY ["src/BaSys.Host.Identity/BaSys.Host.Identity.csproj", "src/BaSys.Host.Identity/"]
COPY ["src/BaSys.Host.Infrastructure/BaSys.Host.Infrastructure.csproj", "src/BaSys.Host.Infrastructure/"]
COPY ["src/BaSys.SuperAdmin.DAL/BaSys.SuperAdmin.DAL.csproj", "src/BaSys.SuperAdmin.DAL/"]
COPY ["src/BaSys.Logging.Infrastructure/BaSys.Logging.Infrastructure.csproj", "src/BaSys.Logging.Infrastructure/"]
COPY ["src/BaSys.Logging/BaSys.Logging.csproj", "src/BaSys.Logging/"]
COPY ["src/BaSys.FileStorage/BaSys.FileStorage.csproj", "src/BaSys.FileStorage/"]
COPY ["src/BaSys.App/BaSys.App.csproj", "src/BaSys.App/"]
COPY ["src/BaSys.Constructor/BaSys.Constructor.csproj", "src/BaSys.Constructor/"]
COPY ["src/BaSys.PublicAPI/BaSys.PublicAPI.csproj", "src/BaSys.PublicAPI/"]
COPY ["src/BaSys.SuperAdmin/BaSys.SuperAdmin.csproj", "src/BaSys.SuperAdmin/"]
RUN dotnet restore "./src/BaSys.Host/BaSys.Host.csproj" 
COPY . .
WORKDIR "/src/src/BaSys.Host"
RUN dotnet build "./BaSys.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BaSys.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaSys.Host.dll"]
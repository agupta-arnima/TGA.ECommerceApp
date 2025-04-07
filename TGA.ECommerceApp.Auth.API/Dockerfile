# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TGA.ECommerceApp.Auth.API/TGA.ECommerceApp.Auth.API.csproj", "TGA.ECommerceApp.Auth.API/"]
COPY ["TGA.ECommerceApp.Auth.Application/TGA.ECommerceApp.Auth.Application.csproj", "TGA.ECommerceApp.Auth.Application/"]
COPY ["TGA.ECommerceApp.Auth.data/TGA.ECommerceApp.Auth.Data.csproj", "TGA.ECommerceApp.Auth.data/"]
COPY ["TGA.ECommerceApp.Auth.Domain/TGA.ECommerceApp.Auth.Domain.csproj", "TGA.ECommerceApp.Auth.Domain/"]
RUN dotnet restore "./TGA.ECommerceApp.Auth.API/TGA.ECommerceApp.Auth.API.csproj"
COPY . .
WORKDIR "/src/TGA.ECommerceApp.Auth.API"
RUN dotnet build "./TGA.ECommerceApp.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TGA.ECommerceApp.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGA.ECommerceApp.Auth.API.dll"]
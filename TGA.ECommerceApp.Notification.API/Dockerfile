# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TGA.ECommerceApp.Notification.API/TGA.ECommerceApp.Notification.API.csproj", "TGA.ECommerceApp.Notification.API/"]
COPY ["TGA.ECommerceApp.Infra.Bus/TGA.ECommerceApp.Infra.Bus.csproj", "TGA.ECommerceApp.Infra.Bus/"]
COPY ["TGA.ECommerceApp.Domain.Core/TGA.ECommerceApp.Domain.Core.csproj", "TGA.ECommerceApp.Domain.Core/"]
COPY ["TGA.ECommerceApp.Notification.Application/TGA.ECommerceApp.Notification.Application.csproj", "TGA.ECommerceApp.Notification.Application/"]
COPY ["TGA.ECommerceApp.Notification.Data/TGA.ECommerceApp.Notification.Data.csproj", "TGA.ECommerceApp.Notification.Data/"]
COPY ["TGA.ECommerceApp.Notification.Domain/TGA.ECommerceApp.Notification.Domain.csproj", "TGA.ECommerceApp.Notification.Domain/"]
RUN dotnet restore "./TGA.ECommerceApp.Notification.API/TGA.ECommerceApp.Notification.API.csproj"
COPY . .
WORKDIR "/src/TGA.ECommerceApp.Notification.API"
RUN dotnet build "./TGA.ECommerceApp.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TGA.ECommerceApp.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGA.ECommerceApp.Notification.API.dll"]
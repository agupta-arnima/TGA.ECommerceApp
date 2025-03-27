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
COPY ["TGA.ECommerceApp.Order.API/TGA.ECommerceApp.Order.API.csproj", "TGA.ECommerceApp.Order.API/"]
COPY ["TGA.ECommerceApp.Order.Application/TGA.ECommerceApp.Order.Application.csproj", "TGA.ECommerceApp.Order.Application/"]
COPY ["TGA.ECommerceApp.Order.Data/TGA.ECommerceApp.Order.Data.csproj", "TGA.ECommerceApp.Order.Data/"]
COPY ["TGA.ECommerceApp.Order.Domain/TGA.ECommerceApp.Order.Domain.csproj", "TGA.ECommerceApp.Order.Domain/"]
RUN dotnet restore "./TGA.ECommerceApp.Order.API/TGA.ECommerceApp.Order.API.csproj"
COPY . .
WORKDIR "/src/TGA.ECommerceApp.Order.API"
RUN dotnet build "./TGA.ECommerceApp.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TGA.ECommerceApp.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGA.ECommerceApp.Order.API.dll"]
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Capstone.ECommerceApp.Order.API/Capstone.ECommerceApp.Order.API.csproj", "Capstone.ECommerceApp.Order.API/"]
COPY ["Capstone.ECommerceApp.Infra.Bus/Capstone.ECommerceApp.Infra.Bus.csproj", "Capstone.ECommerceApp.Infra.Bus/"]
COPY ["Capstone.ECommerceApp.Domain.Core/Capstone.ECommerceApp.Domain.Core.csproj", "Capstone.ECommerceApp.Domain.Core/"]
COPY ["Capstone.ECommerceApp.Order.Application/Capstone.ECommerceApp.Order.Application.csproj", "Capstone.ECommerceApp.Order.Application/"]
COPY ["Capstone.ECommerceApp.Order.Data/Capstone.ECommerceApp.Order.Data.csproj", "Capstone.ECommerceApp.Order.Data/"]
COPY ["Capstone.ECommerceApp.Order.Domain/Capstone.ECommerceApp.Order.Domain.csproj", "Capstone.ECommerceApp.Order.Domain/"]
RUN dotnet restore "./Capstone.ECommerceApp.Order.API/Capstone.ECommerceApp.Order.API.csproj"
COPY . .
WORKDIR "/src/Capstone.ECommerceApp.Order.API"
RUN dotnet build "./Capstone.ECommerceApp.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Capstone.ECommerceApp.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Capstone.ECommerceApp.Order.API.dll"]
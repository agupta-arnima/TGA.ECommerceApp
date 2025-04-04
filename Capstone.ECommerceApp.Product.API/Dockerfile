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
COPY ["Capstone.ECommerceApp.Product.API/Capstone.ECommerceApp.Product.API.csproj", "Capstone.ECommerceApp.Product.API/"]
COPY ["Capstone.ECommerceApp.Product.Application/Capstone.ECommerceApp.Product.Application.csproj", "Capstone.ECommerceApp.Product.Application/"]
COPY ["Capstone.ECommerceApp.Product.Data/Capstone.ECommerceApp.Product.Data.csproj", "Capstone.ECommerceApp.Product.Data/"]
COPY ["Capstone.ECommerceApp.Product.Domain/Capstone.ECommerceApp.Product.Domain.csproj", "Capstone.ECommerceApp.Product.Domain/"]
RUN dotnet restore "./Capstone.ECommerceApp.Product.API/Capstone.ECommerceApp.Product.API.csproj"
COPY . .
WORKDIR "/src/Capstone.ECommerceApp.Product.API"
RUN dotnet build "./Capstone.ECommerceApp.Product.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Capstone.ECommerceApp.Product.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Capstone.ECommerceApp.Product.API.dll"]
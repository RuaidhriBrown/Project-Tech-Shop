#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Correct the COPY paths to reflect the actual structure within the build context
COPY ["Services/Project.Tech.Shop.Web.Services.Migrations.Startup/Project.Tech.Shop.Services.Migrations.Startup.csproj", "./Services/Project.Tech.Shop.Web.Services.Migrations.Startup/"]
COPY ["Services/Project.Tech.Shop.Services.Products/Project.Tech.Shop.Services.Products.csproj", "./Services/Project.Tech.Shop.Services.Products/"]
COPY ["Services/Project.Tech.Shop.Services.Common/Project.Tech.Shop.Services.Common.csproj", "./Services/Project.Tech.Shop.Services.Common/"]
COPY ["Services/Project.Tech.Shop.Services.UsersAccounts/Project.Tech.Shop.Services.UsersAccounts.csproj", "./Services/Project.Tech.Shop.Services.UsersAccounts/"]

RUN dotnet restore "./Services/Project.Tech.Shop.Web.Services.Migrations.Startup/Project.Tech.Shop.Services.Migrations.Startup.csproj"
COPY . .
WORKDIR "/src/Services/Project.Tech.Shop.Web.Services.Migrations.Startup"
RUN dotnet build "./Project.Tech.Shop.Services.Migrations.Startup.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Project.Tech.Shop.Services.Migrations.Startup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.Tech.Shop.Services.Migrations.Startup.dll"]
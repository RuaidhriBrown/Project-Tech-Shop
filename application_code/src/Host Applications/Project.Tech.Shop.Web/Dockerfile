# Use official .NET runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Ensure these paths are relative to the context specified in docker-compose.yml or the docker build command
COPY ["Host Applications/Project.Tech.Shop.Web/Project.Tech.Shop.Web.csproj", "Host Applications/Project.Tech.Shop.Web/"]
COPY ["Services/Project.Tech.Shop.Services.Common/Project.Tech.Shop.Services.Common.csproj", "Services/Project.Tech.Shop.Services.Common/"]
COPY ["Services/Project.Tech.Shop.Services.Products/Project.Tech.Shop.Services.Products.csproj", "Services/Project.Tech.Shop.Services.Products/"]
COPY ["Services/Project.Tech.Shop.Services.UsersAccounts/Project.Tech.Shop.Services.UsersAccounts.csproj", "Services/Project.Tech.Shop.Services.UsersAccounts/"]

RUN dotnet restore "Host Applications/Project.Tech.Shop.Web/Project.Tech.Shop.Web.csproj"
COPY . .
WORKDIR "/src/Host Applications/Project.Tech.Shop.Web"
RUN dotnet build "Project.Tech.Shop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application to /app/publish
FROM build AS publish
RUN dotnet publish "Project.Tech.Shop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.Tech.Shop.Web.dll"]

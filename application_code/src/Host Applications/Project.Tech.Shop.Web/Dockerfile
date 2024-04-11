#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Host Applications/Project.Tech.Shop.Web/Project.Tech.Shop.Web.csproj", "src/Host Applications/Project.Tech.Shop.Web/"]
RUN dotnet restore "./src/Host Applications/Project.Tech.Shop.Web/Project.Tech.Shop.Web.csproj"
COPY . .
WORKDIR "/src/src/Host Applications/Project.Tech.Shop.Web"
RUN dotnet build "./Project.Tech.Shop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Project.Tech.Shop.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.Tech.Shop.Web.dll"]
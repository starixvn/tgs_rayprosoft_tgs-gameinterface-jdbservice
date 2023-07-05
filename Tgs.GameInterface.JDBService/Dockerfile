#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameInterface.JDBService/Tgs.GameInterface.JDBService.csproj", "Tgs.GameInterface.JDBService/"]
COPY ["Tgs.GameInterface.JDBService.Core/Tgs.GameInterface.JDBService.Core.csproj", "Tgs.GameInterface.JDBService.Core/"]
RUN dotnet restore "Tgs.GameInterface.JDBService/Tgs.GameInterface.JDBService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameInterface.JDBService"
RUN dotnet build "Tgs.GameInterface.JDBService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameInterface.JDBService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameInterface.JDBService.dll"]
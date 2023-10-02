#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["PVService/PVService.csproj", "PVService/"]
RUN dotnet restore "PVService/PVService.csproj"
COPY . .
WORKDIR "/src/PVService"
RUN dotnet build "PVService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "PVService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM build AS final
# FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
RUN dotnet dev-certs https
ENTRYPOINT ["./PVService"]

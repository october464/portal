#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Finportal.csproj", ""]
RUN dotnet restore "./Finportal.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Finportal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Finportal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Finportal.dll
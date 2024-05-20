#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN addgroup --gid 2000 --group "group1"
RUN adduser --gid 2000 --uid 1000 --disabled-password --gecos "" "user1"
WORKDIR /app
RUN chown user1:group1 /app
USER user1:group1
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Factories.WebApi.PL/Factories.WebApi.PL.csproj", "src/Factories.WebApi.PL/"]
COPY ["src/Factories.WebApi.BLL/Factories.WebApi.BLL.csproj", "src/Factories.WebApi.BLL/"]
COPY ["src/Factories.WebApi.DAL/Factories.WebApi.DAL.csproj", "src/Factories.WebApi.DAL/"]
RUN dotnet restore "./src/Factories.WebApi.PL/Factories.WebApi.PL.csproj"
COPY . .
WORKDIR "/src/src/Factories.WebApi.PL"
RUN dotnet build "./Factories.WebApi.PL.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Factories.WebApi.PL.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Factories.WebApi.PL.dll"]
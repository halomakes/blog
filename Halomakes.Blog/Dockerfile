FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Halomakes.Blog/Halomakes.Blog.csproj", "Halomakes.Blog/"]
RUN dotnet restore "Halomakes.Blog/Halomakes.Blog.csproj"
COPY . .
WORKDIR "/src/Halomakes.Blog"
RUN dotnet build "./Halomakes.Blog.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Halomakes.Blog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Halomakes.Blog.dll"]

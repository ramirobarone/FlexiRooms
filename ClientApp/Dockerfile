# Etapa build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Node para compilar Angular durante dotnet publish (Angular 15 compatible)
RUN apt-get update && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*

# Copiar solución y proyectos
COPY Hotelis/FlexiRooms.sln ./Hotelis/FlexiRooms.sln
COPY ClientApp/ClientApp.csproj ./ClientApp/ClientApp.csproj
COPY Application/Application.csproj ./Application/Application.csproj
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/Infrastructure.csproj

RUN dotnet restore ./ClientApp/ClientApp.csproj

# Copiar todo
COPY . .

# Publicar (esto también ejecuta npm build por el target PublishRunWebpack)
RUN dotnet publish ./ClientApp/ClientApp.csproj -c Release -o /app/publish

# Etapa runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "ClientApp.dll"]
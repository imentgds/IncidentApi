FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /api

# Copier le fichier sln et les fichiers csproj d'abord (cache Docker)
COPY *.slnx ./
COPY WebApplication2/*.csproj WebApplication2/
COPY IncidentApp/AppTests.csproj IncidentApp/

# Récupérer les dépendances - restore each project
RUN dotnet restore WebApplication2/WebApplication2.csproj
RUN dotnet restore IncidentApp/AppTests.csproj

# Copier tout le reste
COPY . .

# Publier uniquement l'API (important)
RUN dotnet publish WebApplication2/WebApplication2.csproj -c Release -o /app/publish

# Préparer l'env. d'exécution (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Forcer l'API dans le conteneur d'être accessible depuis la machine hôte
# et d'écouter sur le port 80
ENV ASPNETCORE_URLS=http://0.0.0.0:80
EXPOSE 80

# Copier les fichiers publiés de l'application depuis l'étape de build
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplication2.dll"]
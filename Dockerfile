# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie le fichier projet et restaure les dépendances
COPY ["NotificationService.csproj", "./"]
RUN dotnet restore

# Copie tout le reste et publie l'application en mode Release
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Étape d'exécution (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Commande de démarrage du microservice
ENTRYPOINT ["dotnet", "NotificationService.dll"]
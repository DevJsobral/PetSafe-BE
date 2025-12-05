# Etapa 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto primeiro para aproveitar cache do Docker
COPY PetSafe.csproj .
RUN dotnet restore

# Copia o resto do código
COPY . .
RUN dotnet publish -c Release -o /app/out

# Etapa 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia apenas os arquivos publicados
COPY --from=build /app/out .

# Define a URL que a aplicação vai escutar
ENV ASPNETCORE_URLS=http://0.0.0.0:5179
ENV ASPNETCORE_ENVIRONMENT=Production

# Expõe a porta
EXPOSE 5179

# Define o ponto de entrada
ENTRYPOINT ["dotnet", "PetSafe.dll"]

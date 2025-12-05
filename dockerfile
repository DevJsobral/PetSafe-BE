# Etapa 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj
COPY PetSafe.csproj .
RUN dotnet restore

# Copia o restante do projeto
COPY . .
RUN dotnet publish -c Release -o /app/out


# Etapa 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia a build
COPY --from=build /app/out .

# Render define a porta no env $PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

# Apenas informativo — Render ignora EXPOSE
EXPOSE 10000

ENTRYPOINT ["dotnet", "PetSafe.dll"]

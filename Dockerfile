# Estágio 1: Compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivo de projeto e restaurar dependências
COPY ["APICatalogo.csproj", "./"]
RUN dotnet restore "APICatalogo.csproj"

# Copiar todo o restante dos arquivos e compilar
COPY . .
RUN dotnet build "APICatalogo.csproj" -c Release -o /app/build

# Estágio 2: Publicação da aplicação
FROM build AS publish
RUN dotnet publish "APICatalogo.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 3: Execução (Runtime final)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render e Railway expõem as portas automaticamente. O .NET 8 escuta na porta 8080 por padrão.
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "APICatalogo.dll"]

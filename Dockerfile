# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj e restaurar dependências
COPY ["BankMore.ContaCorrente.csproj", "./"]
RUN dotnet restore "BankMore.ContaCorrente.csproj"

# Copiar todo o código e fazer build
COPY . .
RUN dotnet build "BankMore.ContaCorrente.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BankMore.ContaCorrente.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8081

# Copiar arquivos publicados
COPY --from=publish /app/publish .

# Definir variáveis de ambiente padrão
ENV ASPNETCORE_URLS=http://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BankMore.ContaCorrente.dll"]

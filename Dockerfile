FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GetEmpStatus.csproj", "."]
RUN dotnet restore "GetEmpStatus.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "GetEmpStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GetEmpStatus.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GetEmpStatus.dll"]
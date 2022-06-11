FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TheBaron.csproj", "./"]
RUN dotnet restore "TheBaron.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "TheBaron.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TheBaron.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheBaron.dll"]

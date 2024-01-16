FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EventOrganizer.Identity/EventOrganizer.Identity.csproj", "EventOrganizer.Identity/"]
RUN dotnet restore "EventOrganizer.Identity/EventOrganizer.Identity.csproj"
COPY . .
WORKDIR "/src/EventOrganizer.Identity"
RUN dotnet build "EventOrganizer.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventOrganizer.Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventOrganizer.Identity.dll"]

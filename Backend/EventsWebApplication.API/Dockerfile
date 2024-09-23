FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EventsWebApplication.API/EventsWebApplication.API.csproj", "EventsWebApplication.API/"]
COPY ["EventsWebApplication.Application/EventsWebApplication.Application.csproj", "EventsWebApplication.Application/"]
COPY ["EventsWebApplication.Domain/EventsWebApplication.Domain.csproj", "EventsWebApplication.Domain/"]
COPY ["EventsWebApplication.Infrastructure/EventsWebApplication.Infrastructure.csproj", "EventsWebApplication.Infrastructure/"]
RUN dotnet restore "EventsWebApplication.API/EventsWebApplication.API.csproj"
COPY . .
WORKDIR "/src/EventsWebApplication.API"
RUN dotnet build "EventsWebApplication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EventsWebApplication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этап для выполнения миграций
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migration
WORKDIR /src
COPY --from=build /src /src
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Выполнение миграций
WORKDIR /src/EventsWebApplication.Infrastructure
RUN dotnet ef database update --project ../EventsWebApplication.Infrastructure --startup-project .


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventsWebApplication.API.dll"]

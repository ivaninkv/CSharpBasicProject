FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY ["FlightsMetaSubscriber.App/FlightsMetaSubscriber.App.csproj", "FlightsMetaSubscriber.App/"]
RUN dotnet restore "FlightsMetaSubscriber.App/FlightsMetaSubscriber.App.csproj"
COPY . .
WORKDIR "/src/FlightsMetaSubscriber.App"
RUN dotnet build "FlightsMetaSubscriber.App.csproj" -c Release -o /app/build
RUN dotnet publish "FlightsMetaSubscriber.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlightsMetaSubscriber.App.dll"]

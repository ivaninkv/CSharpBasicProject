FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ARG TARGETARCH
ARG TARGETOS
ARG BUILDPLATFORM

RUN arch=$TARGETARCH \
    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
    && echo $TARGETOS-$arch > /tmp/rid

WORKDIR /src
COPY ["FlightsMetaSubscriber.App/FlightsMetaSubscriber.App.csproj", "FlightsMetaSubscriber.App/"]
RUN dotnet restore "FlightsMetaSubscriber.App/FlightsMetaSubscriber.App.csproj" -r $(cat /tmp/rid)
COPY . .
WORKDIR "/src/FlightsMetaSubscriber.App"
RUN dotnet build "FlightsMetaSubscriber.App.csproj" -c Release -o /app/build -r $(cat /tmp/rid)
RUN dotnet publish "FlightsMetaSubscriber.App.csproj" -c Release -o /app/publish -r $(cat /tmp/rid)

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlightsMetaSubscriber.App.dll"]

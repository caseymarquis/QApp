FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY AppCore/AppCore.csproj AppCore/
COPY App/App.csproj App/
RUN dotnet restore AppCore/AppCore.csproj
COPY . .
WORKDIR /src/AppCore
RUN dotnet build AppCore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AppCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AppCore.dll"]

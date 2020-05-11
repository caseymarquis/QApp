FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
COPY App/App.csproj App/
RUN dotnet restore App/App.csproj
COPY ./App/ ./App/
RUN dotnet publish ./App/App.csproj -c release -o /output --no-restore

FROM node AS www
WORKDIR /wwwdev
COPY App/wwwdev/*.json ./
RUN npm install
COPY App/wwwdev/ ./
#COPY App/wwwroot/*.ico /wwwroot/
RUN node ./node_modules/webpack/bin/webpack.js --progress --colors --display-error-details --content-base ../wwwroot

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /app
COPY --from=build /output ./
Copy --from=www /wwwroot ./wwwroot/

#Normal entrypoint:
#ENTRYPOINT ["dotnet", "QApp.dll"]

#Heroku entrypoint:
CMD DATABASE_URL=$DATABASE_URL ASPNETCORE_URLS=http://*:$PORT dotnet QApp.dll

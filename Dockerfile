FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app
COPY AppCore/AppCore.csproj AppCore/
COPY App/App.csproj App/
RUN dotnet restore AppCore/AppCore.csproj
COPY ./AppCore/ ./AppCore/
COPY ./App/ ./App/
RUN dotnet publish ./AppCore/AppCore.csproj -c Release -o /output

FROM node AS www
WORKDIR /wwwdev
COPY App/wwwdev/*.json ./
RUN npm install
COPY App/wwwdev/ ./
#COPY App/wwwroot/*.ico /wwwroot/
RUN node ./node_modules/webpack/bin/webpack.js --progress --colors --display-error-details --content-base ../wwwroot

FROM microsoft/dotnet:2.1-runtime AS final
WORKDIR /app
COPY --from=build /output ./
Copy --from=www /wwwroot ./wwwroot/
ENTRYPOINT ["dotnet", "AppCore.dll"]

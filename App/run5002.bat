REM Remember to compile the website first by running './wwwdev/compile.bat'. If you're testing multiple servers, the dev server will only point at the first one.
SET NOHSTS=1
SET ASPNETCORE_URLS=http://0.0.0.0:5002/
dotnet run

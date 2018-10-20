REM If you want to add something to this which won't work for everyone,
REM then you should make a :startit call in here to a gitignored file.

call start _start-dev-server.bat

%SystemRoot%\explorer.exe "vscode://file/%~dp0App/wwwdev/"

call :startcmd ./QApp.sln
call :startcmd "C:\Program Files\AutoHotkey\AutoHotkey.exe"
call :startcmd "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "http://localhost:8080/"
call :startcmd "github desktop.lnk"

:startcmd
if exist "%~1" (
  start "%~1" "%~1" %~2
)
EXIT /B 0
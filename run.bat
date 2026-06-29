@echo off
REM Double-click to start Inkvoice. Close the server window to stop it.
cd /d "%~dp0"
start "Inkvoice server" dotnet run -c Release --urls "http://localhost:5099"
timeout /t 6 >nul
start http://localhost:5099

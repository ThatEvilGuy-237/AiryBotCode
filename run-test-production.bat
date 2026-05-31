@echo off
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-test-production.ps1" %*
exit /b %ERRORLEVEL%

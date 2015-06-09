@echo off
SETLOCAL EnableDelayedExpansion
echo.
echo Deploying MyVote.Client.Web to Windows Azure Web Sites...
echo #########################################################
echo.

set CHUTZ_EXE=%~dp0\src\packages\Chutzpah.3.1.1\tools\chutzpah.console.exe

if not exist "%CHUTZ_EXE%" (
  goto need_build
)
echo Executing unit tests...
"%CHUTZ_EXE%" "%~dp0\src\MyVote.Client.Web.Tests\Tests"
if !ERRORLEVEL! NEQ 0 goto test_fail

echo Pushing to azure...
git push azure master

goto:EOF

:test_fail
echo Unit test failure.
echo.
goto:EOF

:need_build
echo Chutzpah unit test runner not found. Please build the solution to restore NuGet packages.
echo.
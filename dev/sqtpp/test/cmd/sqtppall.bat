@echo off
REM -------------------------------------------------------------------
REM TODO: Add some description.
REM -------------------------------------------------------------------
setlocal
set failct=0
set sqtpp=..\..\Debug\sqtpp.exe

if not exist failed\nul mkdir failed
if not exist sqtppout\nul mkdir sqtppout

for %%a in (input\*.*) do call :exec %%~nxa

rem for %a in ( input\* ) call :compare %a%

:end
exit /b


REM -------------------------------------------------------------------
REM Subroutine calling the preprocessor and performing the file compare
REM of the result against the file in the reference directory
REM -------------------------------------------------------------------
:exec
rem Ignore source safe files.
set ext=%~x1
if "%ext%" == ".scc" goto :eof

%sqtpp% input\%1 > sqtppout\%1 2>&1 
fc /b sqtppout\%1 reference\%1 >nul
if errorlevel 1 (
	echo %1 failed
	copy sqtppout\%1 failed\%1 >nul
	set /a failCount=%failCount% + 1
	
)

goto :eof
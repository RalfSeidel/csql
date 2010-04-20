@echo off
REM -------------------------------------------------------------------
REM 1. Run the preprocessor for every file found in the input folder
REM    and emit the output to sqtppout. Compare the result with the
REM    same file in the output folder. If the compare operation finds a 
REM    difference the test failed. 
REM 
REM 2. Some tests use specialize sqtpp arguments. These tests are 
REM    stored in the input\special folder. Each of these test is 
REM    executed with it own call.
REM -------------------------------------------------------------------
setlocal
set failCount=0
set computername=sqtpp_test_host
set username=sqtpp_test_user
set sqtpp=..\..\..\Debug\sqtpp.exe


if not exist failed\nul mkdir failed
if not exist sqtppout\nul mkdir sqtppout

for %%a in (input\*.*) do call :exec %%~nxa

REM Remove empty line and emit line numbers
call :exec special\emitline1.h /E /e+
call :exec special\emitline2.h /E /e+

REM Check include directories are used as expected
call :exec special\findfile.h /E /e+ /Iinput\special\include

:end
exit /b


REM -------------------------------------------------------------------
REM Subroutine calling the preprocessor and performing the file compare
REM of the result against the file in the reference directory.
REM 
REM The first parameter is the path of the test.
REM Any further parameter is passed to sqtpp.
REM -------------------------------------------------------------------
:exec
rem Ignore source safe files.
set ext=%~x1
if "%ext%" == ".scc" goto :eof

set test_i=%1
set test_o=%test_i:\=_%
set test_o=%test_o:/=_%
set test_args=
if not "%2" == "" set test_args=%test_args% %2
if not "%3" == "" set test_args=%test_args% %3
if not "%4" == "" set test_args=%test_args% %4
if not "%5" == "" set test_args=%test_args% %5
if not "%6" == "" set test_args=%test_args% %6
if not "%7" == "" set test_args=%test_args% %7
if not "%8" == "" set test_args=%test_args% %8
if not "%9" == "" set test_args=%test_args% %9
rem echo %sqtpp% -iinput\%test_i% %test_args% ^> sqtppout\%test_o% 
%sqtpp% -iinput\%test_i% %test_args% -Osqtppout\%test_o%
fc /b sqtppout\%test_o% reference\%test_o% >nul
if errorlevel 1 (
	echo %test_i% failed
	copy sqtppout\%test_o% failed\%test_o% >nul
	set /a failCount=%failCount% + 1
	
)

goto :eof
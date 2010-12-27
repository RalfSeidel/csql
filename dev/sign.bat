@echo off
REM Use this batch file to apply the digital signing certificate to the files
REM produced by the "deploy" build configuration.
REM 
REM Because the files are packed into a setup .msi file the signing batch has to be called twice.
REM A) Build the files.
REM    Signed all binaries.
REM    Set the readonly flag on a files exception the setup files. 
REM B) Build the setup.
REM    Call this batch again to sign the setup files.

setlocal
call "%VS100COMNTOOLS%\vsvars32.bat"

if .%1 == .  (
	echo Password for certificate is required.
	exit /b
)
set password=%1
echo Password=%password%
call :SignFile bin\deploy\csql.addin.dll bin\deploy\Sqt.DbcProvider.dll bin\deploy\csql.lib.dll bin\deploy\csql.exe bin\deploy\sqtpp.exe bin\deploy\CSql.Setup.CustomActions.dll
call :SignFile bin\deploy\csqlsetup.msi bin\deploy\setup.exe

exit /b




:SignFile
@signtool sign /f sqlservice.pfx /p %password% /t http://timestamp.verisign.com/scripts/timstamp.dll /v %1 %2 %3 %4 %5 %6 %7 %8 %9
rem @signtool sign /f sqlservice.pfx /p %password% /v %1 %2 %3 %4 %5 %6 %7 %8 %9

goto :eof
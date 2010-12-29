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

REM Note: The microsoft setup project takes the files from the projects refereced by "Primary output from" 
REM from the obj directory of the C# projects. For the c++ project and detected references the files
REM are taken from the output directory (bin/deploy).

call :SignFile bin\deploy\Sqt.DbcProvider.dll bin\deploy\csql.lib.dll bin\deploy\sqtpp.exe obj\deploy\Setup.CustomActions\CSql.Setup.CustomActions.dll obj\deploy\csql.exe\csql.exe obj\deploy\csql.addin\csql.addin.dll 
rem call :SignFile bin\deploy\csqlsetup.msi bin\deploy\setup.exe

exit /b




:SignFile
rem See https://knowledge.verisign.com/support/code-signing-support/index?page=content&id=SO5820
@signtool sign /n "SQL Service" /s My /ac d:\src\sqtlib\components\cert\MSCV-VSClass3.cer /t http://timestamp.verisign.com/scripts/timstamp.dll %1 %2 %3 %4 %5 %6 %7 %8 %9
rem @signtool sign /f sqlservice.pfx /p %password% /v %1 %2 %3 %4 %5 %6 %7 %8 %9

goto :eof
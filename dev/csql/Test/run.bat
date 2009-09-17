@echo off
REM Batch file to run all %csql% tests.
setlocal 
set failCount=0
set computername=sqtpp_test_host
set username=sqtpp_test_user
set csql=..\..\Debug\csql.exe

if not exist csqlout\nul mkdir csqlout

echo on
@echo.
@echo *** Running MS SQL Server tests ***
@echo.

%csql% -cf mssql.cfg.txt -i input\PrintFromProcedure.sql  > csqlout\PrintFromProcedure.mssql.log
%csql% -cf mssql.cfg.txt -i input\DependencyWarning.sql > csqlout\DependencyWarning.mssql.log

%csql% -cf mssql.cfg.txt -i input\SelectTest.sql  > csqlout\SelectTest.mssql.log

%csql% -cf mssql.cfg.txt -i input\ErrorLineTest_1.sql > csqlout\ErrorLineTest_1.mssql.log
%csql% -cf mssql.cfg.txt -i input\ErrorLineTest_2.sql > csqlout\ErrorLineTest_2.mssql.log
%csql% -cf mssql.cfg.txt -i input\ErrorLineTest_3.sql > csqlout\ErrorLineTest_3.mssql.log
%csql% -cf mssql.cfg.txt -i input\ErrorLineTest_4.sql > csqlout\ErrorLineTest_4.mssql.log

@echo.
@echo *** Running Sybase ASE SQL Server tests ***
@echo.

%csql% -cf sybase.cfg.txt -i input\PrintFromProcedure.sql  > csqlout\PrintFromProcedure.sybase.log
REM %csql% -cf sybase.cfg.txt -i input\DependencyWarning.sql > csqlout\DependencyWarning.sybase.log

%csql% -cf sybase.cfg.txt -i input\SelectTest.sql  > csqlout\SelectTest.sybase.log

%csql% -cf sybase.cfg.txt -i input\ErrorLineTest_1.sql > csqlout\ErrorLineTest_1.sybase.log
%csql% -cf sybase.cfg.txt -i input\ErrorLineTest_2.sql > csqlout\ErrorLineTest_2.sybase.log
%csql% -cf sybase.cfg.txt -i input\ErrorLineTest_3.sql > csqlout\ErrorLineTest_3.sybase.log
%csql% -cf sybase.cfg.txt -i input\ErrorLineTest_4.sql > csqlout\ErrorLineTest_4.sybase.log


@echo off
REM Compare the results with the files in the reference directory.
for %%a in (csqlout\*.*) do call :comp %%~nxa


:end
exit /b


REM -------------------------------------------------------------------
REM Subroutine to compare the results of a test run with the reference
REM results.
REM -------------------------------------------------------------------
:comp

fc /b csqlout\%1 reference\%1 >nul
if errorlevel 1 (
	echo %1 failed
	set /a failCount=%failCount% + 1
)



goto :eof
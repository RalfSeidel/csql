@echo off
REM Batch file to run all csql tests.
setlocal 
set failCount=0
set computername=sqtpp_test_host
set username=sqtpp_test_user
set csql=..\..\Debug\csql.exe
set path=%path%;..\..\Debug

if not exist csqlout\nul mkdir csqlout

echo on
csql -cf mssql.cfg.txt -i input\PrintFromProcedure.sql  > csqlout\PrintFromProcedure.mssql.log
csql -cf mssql.cfg.txt -i input\DependencyWarning.sql > csqlout\DependencyWarning.mssql.log

csql -cf mssql.cfg.txt -i input\SelectTest.sql  > csqlout\SelectTest.mssql.log

csql -cf mssql.cfg.txt -i input\ErrorLineTest_1.sql > csqlout\ErrorLineTest_1.mssql.log
csql -cf mssql.cfg.txt -i input\ErrorLineTest_2.sql > csqlout\ErrorLineTest_2.mssql.log
csql -cf mssql.cfg.txt -i input\ErrorLineTest_3.sql > csqlout\ErrorLineTest_3.mssql.log
csql -cf mssql.cfg.txt -i input\ErrorLineTest_4.sql > csqlout\ErrorLineTest_4.mssql.log

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
	rem copy csqlout\%test_o% failed\%test_o% >nul
	set /a failCount=%failCount% + 1
)




goto :eof
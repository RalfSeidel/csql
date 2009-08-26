REM Batch file to run all csql tests.

if not exist csqlout\nul mkdir csqlout

csql -cf mssql.cfg.txt -i input\PrintFromProcedure.sql  > csqlout\PrintFromProcedure.mssql.log

csql -cf mssql.cfg.txt -i input\SelectTest.sql  > csqlout\SelectTest.mssql.log

csql -cf mssql.cfg.txt -i input\ErrorLineTest_1.sql > csqlout\ErrorLineTest_1.mssql.log
csql -cf mssql.cfg.txt -i input\ErrorLineTest_2.sql > csqlout\ErrorLineTest_2.mssql.log
csql -cf mssql.cfg.txt -i input\ErrorLineTest_3.sql > csqlout\ErrorLineTest_3.mssql.log
csql -cf mssql.cfg.txt -i input\ErrorLineTest_4.sql > csqlout\ErrorLineTest_4.mssql.log
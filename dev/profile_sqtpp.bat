setlocal
set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 9.0\Team Tools\Performance Tools
cd release
rem vsinstr.exe sqtpp.exe
vsperfcmd /start:trace /output:sqtpp.vsp
rem sqtpp -oe:\temp\bsqlout.tmp D:\src\SqtLib\Sqt.Web\Sql\install.csql
sqtpp -onul /e+ /Co1200 /Id:\src\wzd.net\sql\Include -Id:\src\wzd.net\sql\scripts -Id:\src\wzd.net\sql\dais -DSCC_DF -DSCC_CURRENT=SCC_VF_DEUTSCHLAND D:\src\Wzd.NET\Sql\Scripts\Common_Install_368.sql 
vsperfcmd /shutdown
vsperfreport sqtpp.vsp /output:e:\temp /summary:all










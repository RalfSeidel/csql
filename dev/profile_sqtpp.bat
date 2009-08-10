setlocal
set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 9.0\Team Tools\Performance Tools
cd release
vsinstr.exe sqtpp.exe
vsperfcmd /start:trace /output:sqtpp.vsp
sqtpp -oe:\temp\bsqlout.tmp D:\src\SqtLib\Sqt.Web\Sql\install.csql
vsperfcmd /shutdown
vsperfreport sqtpp.vsp /output:e:\temp /summary:all










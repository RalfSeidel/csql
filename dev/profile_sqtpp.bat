REM Batch to profile sqtpp
REM Before you run this script mache shure that the /Profile option
REM is set in the advanced linker options of the sqtpp release
REM configuration
setlocal
set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools\x64
set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools
cd bin\release
vsinstr.exe sqtpp.exe
vsperfcmd /start:trace /output:sqtpp.vsp

rem For checking this job file use 
rem sqtpp -onul /Co1200 ..\..\IntegrationTest\Files\sqtpp\input\ansiabc.txt

sqtpp -onul /e+ /Co1200 /DSQAF_DATABASE=Nautilus  /ID:\Src\Nautilus\Product\dev\Database\FrameWork\Scripts /ID:\Src\Nautilus\Product\dev\Database\FrameWork\Header /ID:\Src\Nautilus\Product\dev\Database\Topas\Scripts /ID:\Src\Nautilus\Product\dev\Database\Topas\Header /ID:\Src\Nautilus\Product\dev\Database\Topas\Custom /DSCC_CURRENT=SCC_STROEER D:\Src\Nautilus\Product\dev\Database\Framework\Setup\FrameworkSetup.csql
vsperfcmd /shutdown
vsperfreport sqtpp.vsp /output:c:\temp /summary:all










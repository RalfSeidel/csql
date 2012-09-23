setlocal

rem *** x64 ***
set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 11.0\Team Tools\Performance Tools\x64
set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\amd64

rem *** x86 *** 
rem set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 11.0\Team Tools\Performance Tools
rem set path=%path%;C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin

set sqtpp=bin\release\sqtpp.exe
rem set sqtpp="c:\Program Files (x86)\SqlService\csql\sqtpp.exe"
rem set sqtpp=bin\sqtpp.exe

timer on
%sqtpp% -onul /e+ /Co1200 /DSQAF_DATABASE=Nautilus  /ID:\Src\Nautilus\Product\dev\Database\FrameWork\Scripts /ID:\Src\Nautilus\Product\dev\Database\FrameWork\Header /ID:\Src\Nautilus\Product\dev\Database\Topas\Scripts /ID:\Src\Nautilus\Product\dev\Database\Topas\Header /ID:\Src\Nautilus\Product\dev\Database\Topas\Custom /DSCC_CURRENT=SCC_STROEER D:\Src\Nautilus\Product\dev\Database\Topas\Setup\setup.csql
timer off










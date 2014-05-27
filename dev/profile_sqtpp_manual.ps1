$sqtpp="bin\release\sqtpp.exe";
# $sqtpp="c:\Program Files\Sql Service\csql\sqtpp.exe"

$t0 = [System.DateTime]::Now;
&$sqtpp  -onul /e+ /Co1200 /DSQAF_DATABASE=Nautilus /ID:\Src\Nautilus\Product\dev\Database\FrameWork\Scripts /ID:\Src\Nautilus\Product\dev\Database\FrameWork\Header /ID:\Src\Nautilus\Product\dev\Database\Topas\Scripts /ID:\Src\Nautilus\Product\dev\Database\Topas\Header /ID:\Src\Nautilus\Product\dev\Database\Topas\Custom /DSCC_CURRENT=SCC_STROEER D:\Src\Nautilus\Product\dev\Database\Topas\Setup\setup.csql
$t1 = [System.DateTime]::Now;
$d = $t1 - $t0;
$d.ToString();











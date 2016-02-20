# ---------------------------------------------------------------------
# Batch to profile sqtpp
# Before you run this script mache sure that the /Profile option
# is set in the advanced linker options of the sqtpp release
# configuration
# ---------------------------------------------------------------------
function Add-Path( [string] $directory )
{
    [string] $p = $ENV:Path;

    if ( !$p.EndsWith( ";" ) )
    {
        $p+= ";"
    }

    if ( !$directory.EndsWith( ";" ) ) {
        $directory+= ";"
    }

    if ( (";" + $p).Contains( ";" + $directory ) ) {
       return;
    }

    $p = $p + $directory;

    ENV:Path = $p
}



Add-Path "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Performance Tools\x64"
Add-Path "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Team Tools\Performance Tools"
cd bin\release

Remove-Item sqtpp.vsp

vsinstr.exe sqtpp.exe
vsperfcmd /U /start:trace /output:sqtpp.vsp


# For checking this job file use 
# sqtpp -onul /Co1200 ..\..\IntegrationTest\Files\sqtpp\input\ansiabc.txt

$input="..\..\IntegrationTest\Files\sqtpp\ProfileInput.sql"
$output="nul"


./sqtpp -o $output /e+ /Co1200 /DSQAF_DATABASE=Nautilus  /ID:\Src\Nautilus\Product\dev_iqd\Database\FrameWork\Scripts /ID:\Src\Nautilus\Product\dev_iqd\Database\FrameWork\Header /ID:\Src\Nautilus\Product\dev_iqd\Database\Topas\Scripts /ID:\Src\Nautilus\Product\dev_iqd\Database\Topas\Header /ID:\Src\Nautilus\Product\dev_iqd\Database\Topas\Custom /DSCC_CURRENT=SCC_STROEER $input
vsperfcmd /shutdown
vsperfreport sqtpp.vsp /output:c:\temp /summary:all
cd ..\..










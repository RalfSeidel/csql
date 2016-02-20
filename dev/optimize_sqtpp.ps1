# ---------------------------------------------------------------------
# Batch to optimize sqtpp using profile guided optimization
# The batch simplize instruments the program, preprocesses a
# typical file and optimizes the program.
# 
# For informations about profile guide optimization see e.g.
# https://blogs.msdn.microsoft.com/vcblog/2008/11/12/pogo/ (POGO Pogo aka PGO aka Profile Guided Optimization ).
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

    $ENV:Path = $p
}

# Adjust Path for compiler and linker tools.
$VsDirectory = "C:\Program Files (x86)\Microsoft Visual Studio 12.0\"
$ENV:Path=$VsDirectory + "VC\BIN\amd64"
Add-Path $VsDirectory + "VC\BIN\amd64"
Add-Path "C:\Program Files (x86)\Windows Kits\8.1\bin\x64"

# Set Lib Environment for the linker.
$l = $VsDirectory + "VC\LIB\amd64;";
$l = $l + "C:\Program Files (x86)\Windows Kits\8.1\lib\winv6.3\um\x64;"
$ENV:LIB = $l

# Gather required windows libraries.
[string[]] $winlib = @("kernel32.lib",  "user32.lib", "gdi32.lib",  "winspool.lib", "comdlg32.lib",  "advapi32.lib" )
$winlib = $winlib + @("shell32.lib", "ole32.lib", "oleaut32.lib", "uuid.lib", "odbc32.lib", "odbccp32.lib")
$winlib

$sqtpp="bin\release\sqtpp.exe";


# 1. Instrument by linking with /LTCG:PGInstrument
# Note that source code has to be compiled with the /GL flag (Whole Program Optimization)
Write-Host "Instrumenting sqtpp.exe"
link.exe /LTCG:PGInstrument /ERRORREPORT:PROMPT /OUT:"$sqtpp" $winlib /INCREMENTAL:NO /NOLOGO /MANIFEST /MANIFESTUAC:"level='asInvoker' uiAccess='false'" /manifest:embed /DEBUG /PDB:"bin\Release\sqtpp.pdb" /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /PGD:"bin\Release\sqtpp.pgd" /TLBID:1 /DYNAMICBASE /MACHINE:X64 /NXCOMPAT /IMPLIB:"bin\Release\sqtpp.lib" obj\Release\sqtpp.exe\sqtpp.res obj\Release\sqtpp.exe\*.obj  obj\Release\sqtpp.lib


# 2. Run the instrumented file 
Write-Host "Running the instrumented file "
&$sqtpp -onul /e+ /Co1200 IntegrationTest\Files\sqtpp\ProfileInput.csql


# 3. Relink with optimization with /LTCG:PGOPTIMIZE  
Write-Host "Relinken sqtpp.exe with optimization"
link.exe /LTCG:PGOPTIMIZE /ERRORREPORT:PROMPT /OUT:"$sqtpp" $winlib /INCREMENTAL:NO /NOLOGO /MANIFEST /MANIFESTUAC:"level='asInvoker' uiAccess='false'" /manifest:embed /DEBUG /PDB:"bin\Release\sqtpp.pdb" /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /PGD:"bin\Release\sqtpp.pgd" /TLBID:1 /DYNAMICBASE /MACHINE:X64 /NXCOMPAT /IMPLIB:"bin\Release\sqtpp.lib" obj\Release\sqtpp.exe\sqtpp.res obj\Release\sqtpp.exe\*.obj  obj\Release\sqtpp.lib

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle( "CSQL" )]
[assembly: AssemblyDescription( "Addin for Visual Studio to integrate the functionality of the csql script executor." )]
[assembly: AssemblyCompany( "SQL Service GmbH" )]
[assembly: AssemblyProduct( "CSQL" )]
[assembly: AssemblyCopyright( "© 2009 SQL Service GmbH" )]

[assembly: AssemblyVersion( "0.9.9.0" )]
[assembly: AssemblyFileVersion( "0.9.9.0" )]
[assembly: NeutralResourcesLanguageAttribute( "en-US" )]

// Note: For some unknown reason an assembly that hosts a visual studio addin
// needs the com visible attribute set to true. If the assembly is not COM
// visible you will get the error messages "No such interface supported (Error 80004002)"
// when the addin is loaded by the IDE.
[assembly: ComVisible( true )]
[assembly: CLSCompliant( false )]

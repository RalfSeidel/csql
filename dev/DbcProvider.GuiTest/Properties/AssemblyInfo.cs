using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "DbProvider.GuiTest" )]
[assembly: AssemblyDescription( "Simple tests for the GUI components of the Db Provider" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "SQL Service" )]
[assembly: AssemblyProduct( "DbConnection.GuiTest" )]
[assembly: AssemblyCopyright( "Copyright © SQL Service 2010" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
)]


[assembly: AssemblyVersion( "1.0.0.0" )]
// Because of the strong signing the InternalsVisibleTo attributes must specify a public key.
// To get the public key from the strong name run the following commands:
//    sn -p csql.snk csql.pub
//    sn -tp csql.pub
[assembly: InternalsVisibleTo( "Sqt.DbcProvider.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c7249ae2083e5bcd1a3fff2fe1812a24fa1eaf36e36895a592792bcbaed5345d71b59267590885ad9c5f578ed7b6740c31eaafaedaecb45d548afacdb9b97867ae76301c23daed8fdb57bf4e6a6e1f9874c5ad42e976e11afeb0aa89901ae90e7390ce259d8cc4536ab821546478baeb0f9b1962ab36a31bb1dc704ed71e249b" )]

﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;

[assembly: AssemblyProduct( "csql" )]
[assembly: AssemblyCompany( "SQL Service GmbH" )]
[assembly: AssemblyCopyright( "Copyright © 2009 SQL Service GmbH" )]
[assembly: NeutralResourcesLanguage( "en-US" )]
[assembly: AssemblyVersion( "1.0.*" )]
#if DEBUG
[assembly: AssemblyConfiguration( "Debug" )]
#else
[assembly: AssemblyConfiguration( "Release" )]
#endif


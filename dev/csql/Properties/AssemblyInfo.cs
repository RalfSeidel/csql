using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle( "csql library" )]
[assembly: AssemblyDescription( "Sql command processor with preprocessor support." )]
[assembly: CLSCompliant( true )]
[assembly: ComVisible( false )]

// Because of the strong signing the InternalsVisibleTo attributes must specify a public key.
// To get the public key from the strong name run the following commands:
//    sn -p csql.snk csql.pub
//    sn -tp csql.pub
[assembly: InternalsVisibleTo( "CSql.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c7249ae2083e5bcd1a3fff2fe1812a24fa1eaf36e36895a592792bcbaed5345d71b59267590885ad9c5f578ed7b6740c31eaafaedaecb45d548afacdb9b97867ae76301c23daed8fdb57bf4e6a6e1f9874c5ad42e976e11afeb0aa89901ae90e7390ce259d8cc4536ab821546478baeb0f9b1962ab36a31bb1dc704ed71e249b" )]



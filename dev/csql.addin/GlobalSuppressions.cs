// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 


using System.Diagnostics.CodeAnalysis;
[module: SuppressMessage( "Microsoft.Design", "CA1017:MarkAssembliesWithComVisible", Justification = "If the assembly is not COM visible you will get the error messages \"No such interface supported (Error 80004002)\" when the addin is loaded by the IDE." )]
[module: SuppressMessage( "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sqt.VisualStudio", Justification = "The namespace contains classes imported from another project." )]
[module: SuppressMessage( "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sqt.VisualStudio.Util", Justification = "The namespace contains classes imported from another project." )]
[module: SuppressMessage( "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "csql.addin.Commands", Justification = "Namespace is used to group all commands." )]
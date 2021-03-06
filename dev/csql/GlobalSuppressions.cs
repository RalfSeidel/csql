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
[module: SuppressMessage( "Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "lib" )]


[module: SuppressMessage( "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sqt.DbcProvider.Provider.IbmDb2", Justification = "Namespace is used to separted provider type depenend code." )]
[module: SuppressMessage( "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sqt.DbcProvider.Provider.MsSql", Justification = "Namespace is used to separted provider type depenend code." )]
[module: SuppressMessage( "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sqt.DbcProvider.Provider.Oracle", Justification = "Namespace is used to separted provider type depenend code." )]
[module: SuppressMessage( "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sqt.DbcProvider.Provider.Sybase", Justification = "Namespace is used to separted provider type depenend code." )]


//---------------------------------------------------------------------
/**
** @file
** <copyright file="DbMacros.h" company="SQL Service GmbH">
** Copyright (c) SQL Service GmbH.  All rights reserved.
** </copyright>
** <summary>
** Common helper macros for MS Transact SQL Scripts / Sample macro
** package for sqtpp and csql.
** </summary>
**
** <remarks>
** This file can be included several times. Depending on the
** definitions in the including script the helpers are defined in 
** different ways.
** </remarks>
**
** @defgroup Control Definitions controlling what else will be defined in this file.
** @defgroup Action Macro definitions that are expanded into SQL script .
*/
//---------------------------------------------------------------------

/// @def CSQL_CREATE_ALL
/// <summary>
/// If this definition is set when this file is include any other
/// CSQL_CREATE_xxx macro will be implictly defined.
/// </summary>
/// <remarks>
/// If no other flow control is defined this definition will be 
/// set by default.
/// </remarks>
/// @ingroup Control

/// @def CSQL_DROP_CREATE
/// <summary>
/// If this definition is set any CSQL_DROP_xxx macro will be 
/// defined automaticly for any corresponding CSQL_CREATE_xxx
/// defined by the includer.
/// </summary>
/// <remarks>
/// If no other flow control is defined this definition will be 
/// set by default.
/// </remarks>
/// @ingroup Control
/// @example DropCreate.csql

/// @def CSQL_DROP_DEFAULTS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping default constraints.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_DEFAULTS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating default constraints.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_RULES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping check constraints.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_RULES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating check constraints.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_TYPES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping user defined datatypes.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_TYPES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating user defined datatypes.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_TABLES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping tables.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_TABLES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating tables.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_PRIMARY_KEYS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping primary keys.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_PRIMARY_KEYS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating primary keys.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_FOREIGN_KEYS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping foreign keys.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_FOREIGN_KEYS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating foreign keys.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_INDEXES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping indexes.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_INDEXES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating indexes.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_VIEWS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping views.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_VIEWS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating views.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_PROCEDURES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping procedures.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_PROCEDURES
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating procedures.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_FUNCTIONS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping functions.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_FUNCTIONS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating functions.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_SYNONYMS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping SYNONYMS.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_SYNONYMS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating SYNONYMS.
/// </summary>
/// @ingroup Control


/// @def CSQL_DROP_TRIGGERS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are dropping triggers.
/// </summary>
/// @ingroup Control

/// @def CSQL_CREATE_TRIGGERS
/// <summary>
/// Use this defintion to include/excluded sections in your script
/// that are creating triggers.
/// </summary>
/// @ingroup Control



/// @def CSQL_EXECUTE_TESTS
/// <summary>
/// Include all sections within the <c>@#ifdef CSQL_EXECUTE_TESTS / @#endif</c> sections.
/// </summary>
/// <remarks>
/// You may place some code within the <c>CSQL_EXECUTE_TESTS</c> to perform some tests
/// of your SQL Server logic. 
/// </remarks>
/// @ingroup Control


/// @def CSQL_DROP_PKEYS
/// <summary>
/// Obsolete synonym for <see cref="CSQL_DROP_PRIMARY_KEYS"/>.
/// </summary>
/// @ingroup Control
/// @deprecated Use <see cref="CSQL_DROP_PRIMARY_KEYS"/> instead.

/// @def CSQL_CREATE_PKEYS
/// <summary>
/// Obsolete synonym for <see cref="CSQL_CREATE_PRIMARY_KEYS"/>.
/// </summary>
/// @ingroup Control
/// @deprecated Use <see cref="CSQL_CREATE_PRIMARY_KEYS"/> instead.

/// @def CSQL_UNDEF_ALL
/// <summary>
//// Define the Macro CSQL_UNDEF_ALL to clear all previous macro definitions.
/// </summary>
/// 
/// <remarks>
/// This flow control macro is obsolete. It was used in the following fashion:
/// 
/// <code><pre>
/// // Drop all database objects defined in the file "Script.csql".
/// @#define CSQL_DROP_ALL
/// @#include "Script.csql"
/// 
/// // Reset all CSQL_* definitions.
/// @#define CSQL_UNDEF_ALL
/// @#include "dbMacros.h"
/// 
/// // Include "Script.csql" again but now create all objects defined in it.
/// @#define CSQL_CREATE_ALL
/// @#include "Script.csql"
/// </pre></code>
///
/// Since some version of sqtpp you can use the build pre processor 
/// directive <c>@#undef_all</c> instead. The following example shows
/// the difference.
/// <code><pre>
/// // Drop all database objects defined in the file "Script.csql".
/// @#define CSQL_DROP_ALL
/// @#include "Script.csql"
/// 
/// // Reset all CSQL_* definitions.
/// @#undef_all CSQL_
/// 
/// // Include "Script.csql" again but now create all objects defined in it.
/// @#define CSQL_CREATE_ALL
/// @#include "Script.csql"
/// </pre></code>
/// </remarks>
/// @deprecated Use <c>@#undef_all CSQL_</c> instead of including this file
/// with <c>CSQL_UNDEF_ALL</c> defined.
/// @ingroup Control


/// @def CSQL_DROP_FKEYS
/// <summary>
/// Obsolete synonym for <see cref="CSQL_DROP_FOREIGN_KEYS"/>.
/// </summary>
/// @ingroup Control
/// @deprecated Use <see cref="CSQL_DROP_FOREIGN_KEYS"/> instead.

/// @def CSQL_CREATE_FKEYS
/// <summary>
/// Obsolete synonym for <see cref="CSQL_CREATE_FOREIGN_KEYS"/>.
/// </summary>
/// @ingroup Control
/// @deprecated Use <see cref="CSQL_CREATE_FOREIGN_KEYS"/> instead.


#ifdef CSQL_UNDEF_ALL

#undef CSQL_DROP_ALL
#undef CSQL_DROP_DEFAULTS
#undef CSQL_DROP_RULES
#undef CSQL_DROP_TYPES
#undef CSQL_DROP_TABLES
#undef CSQL_DROP_PRIMARY_KEYS
#undef CSQL_DROP_FOREIGN_KEYS
#undef CSQL_DROP_PKEYS
#undef CSQL_DROP_FKEYS
#undef CSQL_DROP_INDEXES
#undef CSQL_DROP_TRIGGERS
#undef CSQL_DROP_VIEWS
#undef CSQL_DROP_PROCEDURES
#undef CSQL_DROP_FUNCTIONS
#undef CSQL_DROP_SYNONYMS

#undef CSQL_CREATE_ALL
#undef CSQL_CREATE_DEFAULTS
#undef CSQL_CREATE_RULES
#undef CSQL_CREATE_TYPES
#undef CSQL_CREATE_TABLES
#undef CSQL_CREATE_ALL_BUT_TABLES
#undef CSQL_CREATE_PKEYS
#undef CSQL_CREATE_PRIMARY_KEYS
#undef CSQL_CREATE_FOREIGN_KEYS
#undef CSQL_CREATE_FKEYS
#undef CSQL_CREATE_INDEXES
#undef CSQL_CREATE_TRIGGERS
#undef CSQL_CREATE_VIEWS
#undef CSQL_CREATE_PROCEDURES
#undef CSQL_CREATE_FUNCTIONS
#undef CSQL_CREATE_SYNONYMS

#undef CSQL_GRANT_ALL
#undef CSQL_GRANT_TABLES
#undef CSQL_GRANT_VIEWS
#undef CSQL_GRANT_PROCEDURES
#undef CSQL_GRANT_FUNCTIONS

#undef CSQL_INSERT_DEF_DATA
#undef CSQL_EXECUTE_TESTS

#undef CSQL_UNDEF_ALL

#else

/*
** If no flow control macro is defined the default behaviour is
** to drop and create everything.
*/

#if !( defined(CSQL_CREATE_ALL)          || defined(CSQL_CREATE_ALL_BUT_TABLES) \
    || defined(CSQL_CREATE_DEFAULTS )    || defined(CSQL_CREATE_RULES ) \
    || defined(CSQL_CREATE_TYPES)        || defined(CSQL_CREATE_TABLES) \
    || defined(CSQL_CREATE_PRIMARY_KEYS) || defined(CSQL_CREATE_FOREIGN_KEYS)  \
    || defined(CSQL_CREATE_PKEYS)        || defined(CSQL_CREATE_FKEYS)  \
    || defined(CSQL_CREATE_INDEXES)      || defined(CSQL_CREATE_TRIGGERS) \
    || defined(CSQL_CREATE_VIEWS)        || defined(CSQL_CREATE_PROCEDURES) \
    || defined(CSQL_CREATE_FUNCTIONS)    || defined(CSQL_CREATE_SYNONYMS) \
    || defined(CSQL_GRANT_ALL) \
    || defined(CSQL_GRANT_TABLES)        || defined(CSQL_GRANT_VIEWS) \
    || defined(CSQL_GRANT_PROCEDURES)    || defined(CSQL_GRANT_FUNCTIONS) \
    || defined(CSQL_INSERT_DEF_DATA) \
    || defined(CSQL_EXECUTE_TESTS)   \
    || defined(CSQL_DROP_ALL) \
    || defined(CSQL_DROP_DEFAULTS)       || defined(CSQL_DROP_RULES) \
    || defined(CSQL_DROP_TYPES)          || defined(CSQL_DROP_TABLES) \
    || defined(CSQL_DROP_PRIMARY_KEYS)   || defined(CSQL_DROP_FOREIGN_KEYS)  \
    || defined(CSQL_DROP_PKEYS)          || defined(CSQL_DROP_FKEYS)  \
    || defined(CSQL_DROP_INDEXES)        || defined(CSQL_DROP_TRIGGERS) \
    || defined(CSQL_DROP_VIEWS)          || defined(CSQL_DROP_PROCEDURES) \
    || defined(CSQL_DROP_FUNCTIONS)      || defined(CSQL_DROP_SYNONYMS))
#define CSQL_CREATE_ALL
#define CSQL_DROP_CREATE
#define CSQL_GRANT_ALL
#define CSQL_INSERT_DEF_DATA
#define CSQL_EXECUTE_TESTS
#endif


/*
** Alles einspielen? Dann die ensprechenden defines setzten.
*/

#if defined( CSQL_CREATE_ALL ) || defined( CSQL_CREATE_ALL_BUT_TABLES )
#undef CSQL_CREATE_DEFAULTS
#undef CSQL_CREATE_RULES
#undef CSQL_CREATE_TYPES
#undef CSQL_CREATE_TABLES
#undef CSQL_CREATE_PRIMARY_KEYS
#undef CSQL_CREATE_FOREIGN_KEYS
#undef CSQL_CREATE_PKEYS
#undef CSQL_CREATE_FKEYS
#undef CSQL_CREATE_INDEXES
#undef CSQL_CREATE_TRIGGERS
#undef CSQL_CREATE_VIEWS
#undef CSQL_CREATE_PROCEDURES
#undef CSQL_CREATE_FUNCTIONS
#undef CSQL_CREATE_SYNONYMS

#ifdef CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_DEFAULTS     CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_RULES        CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_TYPES        CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_PRIMARY_KEYS CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_FOREIGN_KEYS CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_INDEXES      CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_TRIGGERS     CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_VIEWS        CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_PROCEDURES   CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_FUNCTIONS    CSQL_CREATE_ALL_BUT_TABLES
#define CSQL_CREATE_SYNONYMS     CSQL_CREATE_ALL_BUT_TABLES
#else
#define CSQL_CREATE_DEFAULTS     CSQL_CREATE_ALL
#define CSQL_CREATE_RULES        CSQL_CREATE_ALL
#define CSQL_CREATE_TYPES        CSQL_CREATE_ALL
#define CSQL_CREATE_TABLES       CSQL_CREATE_ALL
#define CSQL_CREATE_PRIMARY_KEYS CSQL_CREATE_ALL
#define CSQL_CREATE_FOREIGN_KEYS CSQL_CREATE_ALL
#define CSQL_CREATE_INDEXES      CSQL_CREATE_ALL
#define CSQL_CREATE_TRIGGERS     CSQL_CREATE_ALL
#define CSQL_CREATE_VIEWS        CSQL_CREATE_ALL
#define CSQL_CREATE_PROCEDURES   CSQL_CREATE_ALL
#define CSQL_CREATE_FUNCTIONS    CSQL_CREATE_ALL
#define CSQL_CREATE_SYNONYMS     CSQL_CREATE_ALL
#endif
#endif // CSQL_CREATE_ALL

#if defined( CSQL_DROP_ALL ) || defined( CSQL_DROP_CREATE )
#undef CSQL_DROP_DEFAULTS
#undef CSQL_DROP_RULES
#undef CSQL_DROP_TYPES
#undef CSQL_DROP_TABLES
#undef CSQL_DROP_PRIMARY_KEYS
#undef CSQL_DROP_FOREIGN_KEYS
#undef CSQL_DROP_PKEYS
#undef CSQL_DROP_FKEYS
#undef CSQL_DROP_INDEXES
#undef CSQL_DROP_TRIGGERS
#undef CSQL_DROP_VIEWS
#undef CSQL_DROP_PROCEDURES
#undef CSQL_DROP_FUNCTIONS
#undef CSQL_DROP_SYNONYMS
#endif // defined( CSQL_DROP_ALL ) || defined( CSQL_DROP_CREATE )


#if defined( CSQL_DROP_CREATE )

#ifdef CSQL_CREATE_DEFAULTS
#define CSQL_DROP_DEFAULTS     CSQL_CREATE_DEFAULTS
#endif
#ifdef CSQL_CREATE_RULES
#define CSQL_DROP_RULES        CSQL_CREATE_RULES
#endif
#ifdef CSQL_CREATE_TYPES
#define CSQL_DROP_TYPES        CSQL_CREATE_TYPES
#endif
#ifdef CSQL_CREATE_TABLES
#define CSQL_DROP_TABLES       CSQL_CREATE_TABLES
#endif
#ifdef CSQL_CREATE_PRIMARY_KEYS
#define CSQL_DROP_PRIMARY_KEYS CSQL_CREATE_PRIMARY_KEYS
#endif
#ifdef CSQL_CREATE_FOREIGN_KEYS
#define CSQL_DROP_FOREIGN_KEYS CSQL_CREATE_FOREIGN_KEYS
#endif
#ifdef CSQL_CREATE_INDEXES
#define CSQL_DROP_INDEXES      CSQL_CREATE_INDEXES
#endif
#ifdef CSQL_CREATE_TRIGGERS
#define CSQL_DROP_TRIGGERS     CSQL_CREATE_TRIGGERS
#endif
#ifdef CSQL_CREATE_VIEWS
#define CSQL_DROP_VIEWS        CSQL_CREATE_VIEWS
#endif
#ifdef CSQL_CREATE_PROCEDURES
#define CSQL_DROP_PROCEDURES   CSQL_CREATE_PROCEDURES
#endif
#ifdef CSQL_CREATE_FUNCTIONS
#define CSQL_DROP_FUNCTIONS    CSQL_CREATE_FUNCTIONS
#endif
#ifdef CSQL_CREATE_SYNONYMS
#define CSQL_DROP_SYNONYMS     CSQL_CREATE_SYNONYMS
#endif


#elif defined( CSQL_DROP_ALL ) || defined( CSQL_DROP_ALL_BUT_TABLES )

#define CSQL_DROP_DEFAULTS     CSQL_DROP_ALL
#define CSQL_DROP_RULES        CSQL_DROP_ALL
#define CSQL_DROP_TYPES        CSQL_DROP_ALL
#ifndef CSQL_DROP_ALL_BUT_TABLES
#define CSQL_DROP_TABLES       CSQL_DROP_ALL
#endif
#define CSQL_DROP_PKEYS        CSQL_DROP_ALL
#define CSQL_DROP_FKEYS        CSQL_DROP_ALL
#define CSQL_DROP_INDEXES      CSQL_DROP_ALL
#define CSQL_DROP_TRIGGERS     CSQL_DROP_ALL
#define CSQL_DROP_VIEWS        CSQL_DROP_ALL
#define CSQL_DROP_PROCEDURES   CSQL_DROP_ALL
#define CSQL_DROP_FUNCTIONS    CSQL_DROP_ALL
#define CSQL_DROP_SYNONYMS     CSQL_DROP_ALL

#endif // CSQL_DROP_ALL


#if defined( CSQL_GRANT_ALL )
#define CSQL_GRANT_TABLES      CSQL_GRANT_ALL
#define CSQL_GRANT_VIEWS       CSQL_GRANT_ALL
#define CSQL_GRANT_PROCEDURES  CSQL_GRANT_ALL
#define CSQL_GRANT_FUNCTIONS   CSQL_GRANT_ALL
#endif // CSQL_GRANT_ALL


/*
** For backward compatibility.
*/
#if !defined( CSQL_DROP_PRIMARY_KEYS ) && defined( CSQL_DROP_PKEYS )
#define CSQL_DROP_PRIMARY_KEYS CSQL_DROP_PKEYS
#endif
#if !defined( CSQL_CREATE_PRIMARY_KEYS ) && defined( CSQL_CREATE_PKEYS )
#define CSQL_CREATE_PRIMARY_KEYS CSQL_CREATE_PKEYS
#endif

#if !defined( CSQL_DROP_FOREIGN_KEYS ) && defined( CSQL_DROP_FKEYS )
#define CSQL_DROP_FOREIGN_KEYS CSQL_DROP_FKEYS
#endif
#if !defined( CSQL_CREATE_FOREIGN_KEYS ) && defined( CSQL_CREATE_FKEYS )
#define CSQL_CREATE_FOREIGN_KEYS CSQL_CREATE_FKEYS
#endif

#if defined( CSQL_DROP_PRIMARY_KEYS ) && !defined( CSQL_DROP_PKEYS )
#define CSQL_DROP_PKEYS CSQL_DROP_PRIMARY_KEYS
#endif
#if defined( CSQL_CREATE_PRIMARY_KEYS ) && !defined( CSQL_CREATE_PKEYS )
#define CSQL_CREATE_PKEYS CSQL_CREATE_PRIMARY_KEYS
#endif

#if defined( CSQL_DROP_FOREIGN_KEYS ) && !defined( CSQL_DROP_FKEYS )
#define CSQL_DROP_FKEYS CSQL_DROP_FOREIGN_KEYS
#endif
#if defined( CSQL_CREATE_FOREIGN_KEYS ) && !defined( CSQL_CREATE_FKEYS )
#define CSQL_CREATE_FKEYS CSQL_CREATE_FOREIGN_KEYS
#endif

/*
** Redefine all helper macros.
*/
#undef CSQL_SET_CATALOG
#undef CSQL_DROP_DEFAULT
#undef CSQL_DROP_TYPE
#undef CSQL_DROP_RULE
#undef CSQL_DROP_SCHEMA
#undef CSQL_DROP_TABLE
#undef CSQL_DROP_TEMP_TABLE
#undef CSQL_DROP_VIEW
#undef CSQL_DROP_SYNONYM
#undef CSQL_DROP_PROCEDURE
#undef CSQL_DROP_TRIGGER
#undef CSQL_DROP_TYPE
#undef CSQL_DROP_USER
#undef CSQL_DROP_LOGIN
#undef CSQL_DROP_PRIMARY_KEY
#undef CSQL_DROP_FOREIGN_KEY
#undef CSQL_DROP_PK_CONSTRAINT
#undef CSQL_DROP_FK_CONSTRAINT
#undef CSQL_DROP_UNIQUE_CONSTRAINT
#undef CSQL_DROP_UQ_CONSTRAINT
#undef CSQL_DROP_INDEX
#undef CSQL_DROP_FUNCTION
#undef CSQL_CREATE_DEFAULT
#undef CSQL_CREATE_TYPE
#undef CSQL_CREATE_RULE
#undef CSQL_CREATE_SCHEMA
#undef CSQL_CREATE_VIEW
#undef CSQL_CREATE_PROCEDURE
#undef CSQL_CREATE_FUNCTION
#undef CSQL_CREATE_SCALAR_FUNCTION
#undef CSQL_CREATE_INLINE_FUNCTION
#undef CSQL_CREATE_TABLE_FUNCTION
#undef CSQL_CREATE_TRIGGER
#undef CSQL_CREATE_INSERT_TRIGGER
#undef CSQL_CREATE_UPDATE_TRIGGER
#undef CSQL_CREATE_DELETE_TRIGGER

/// <summary>
/// Macro to change the current database/catalog.
/// </summary>
/// @ingroup Action
#define CSQL_SET_CATALOG( C ) if db_name() != __QUOTE( C ) use C;

/// <summary>
/// Infoausgabe am Anfang eines SQL Scripts.
///</summary>
/// @ingroup Action
#define CSQL_PRINT_ENTER_SCRIPT \
set quoted_identifier, ansi_padding off \
set concat_null_yields_null, ansi_warnings, ansi_nulls on \
print '*** Enter script: ' ##__FILE__ ##' ***' \
print '*** Context: ' + @@servername + '.' + db_name() + ' ***'

/// <summary>
/// Infoausgabe am Ende eines SQL Scripts.
/// </summary>
/// @ingroup Action
#define CSQL_PRINT_EXIT_SCRIPT print '*** Exit script: ' ##__FILE__ ##' ***'

/// <summary>
/// Macro to add source location and version informations at the
/// beginning of a SQL programm object (stored procedure, view, etc).
/// </summary>
/// @ingroup Action
#define CSQL_OBJECT_INFO File: __FILE__ / SCRIPT_REVISION / __TIMESTAMP__


/// @ingroup Action
/// <summary>
/// Macro to create a schema.
/// </summary>
/// <remarks>
/// The default is only created if it doesn't alread exist.
/// </remarks>
/// <param name="X">
/// The name of the schema 
/// </param>
/// @example CreateDefault.csql
#define CSQL_CREATE_SCHEMA( X ) \
if not exists ( select 1 from sys.schemas where name = #@X ) \
begin \
	declare @SqlStmt nvarchar( max ) \
	set @SqlStmt = 'create schema ' + #@X \
	print @SqlStmt \
	exec( @SqlStmt ) \
end




/// @ingroup Action
/// <summary>
/// Macro to create a default that can be used for binding by sp_bindefault.
/// </summary>
/// <remarks>
/// The default is only created if it doesn't alread exist.
/// <strong>Note:</strong> The feature to create and bind defaults is deprecated
/// and is said to be removed in some future version of MS SQL Server.
/// </remarks>
/// <param name="X">
/// The (schema qualified) name of the default. 
/// </param>
/// <param name="V">
/// The value of the default.
/// </param>
/// @example CreateDefault.csql
#ifdef CSQL_CREATE_DEFAULTS
#define CSQL_CREATE_DEFAULT( X, V ) \
if not exists ( select 1 from sys.objects where type = 'D' and object_id = object_id( #@X ) ) \
begin \
	declare @SqlStmt nvarchar( max ) \
	print 'create default ' + #@X \
	set @SqlStmt = 'create default ' + #@X + ' as ' + __QUOTE( V ) \
	exec( @SqlStmt ) \
end
#else  // !CSQL_CREATE_DEFAULTS
#define CSQL_CREATE_DEFAULTS(X, V)
#endif // CSQL_CREATE_DEFAULTS


/// <summary>
/// Macro to drop a default previously created by <code>create default</code>
/// </summary>
/// <remarks>
/// If the default is used i.e. bound to a table attribute or user defined 
/// type the server will raise an error.
/// </remarks>
/// <param name="X">
/// The (schema qualified) name of the default. 
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_DEFAULTS
#define CSQL_DROP_DEFAULT( X ) \
if exists ( select 1 from sys.objects where type = 'D' and object_id = object_id( #@X ) ) \
begin \
	declare @SqlStmt nvarchar( max ) \
	set @SqlStmt = 'drop default ' + #@X \
	print @SqlStmt \
	exec( @SqlStmt ) \
end
#else  // !CSQL_DROP_DEFAULTS
#define CSQL_DROP_DEFAULTS(X)
#endif // CSQL_DROP_DEFAULTS


/// @ingroup Action
/// <summary>
/// Macro to create a rule (check) that can be used for binding by sp_bindrule
/// </summary>
/// <remarks>
/// The rule is only created if it doesn't alread exist.
/// <strong>Note:</strong> The feature to create and bind rules is deprecated and
/// is said to be removed in some future version of MS SQL Server.
/// </remarks>
/// <param name="X">
/// The (schema qualified) name of the rule 
/// </param>
/// <param name="E">
/// The check expression for the rule.
/// </param>
/// @example CreateRule.csql
#ifdef CSQL_CREATE_RULES
#define CSQL_CREATE_RULE( X, E ) \
if not exists ( select 1 from sys.objects where type = 'R' and object_id = object_id( #@X ) ) \
begin \
	declare @SqlStmt nvarchar( max ) \
	print 'create rule ' + #@X \
	set @SqlStmt = 'create rule ' + #@X + ' as ' + __QUOTE( E ) \
	exec( @SqlStmt ) \
end
#else  // !CSQL_CREATE_RULES
#define CSQL_CREATE_RULES(X, E)
#endif // CSQL_CREATE_RULES


/// <summary>
/// Macro to drop a rule previously created by <code>create rule</code>
/// </summary>
/// <remarks>
/// If the rule is used i.e. bound to a table attribute or user defined 
/// type the server will raise an error.
/// </remarks>
/// <param name="X">
/// The (schema qualified) name of the rule. 
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_RULES
#define CSQL_DROP_RULE( X ) \
if exists ( select 1 from sys.objects where type = 'R' and object_id = object_id( #@X ) ) \
begin \
	declare @SqlStmt nvarchar( max ) \
	set @SqlStmt = 'drop rule ' + #@X \
	print @SqlStmt \
	exec( @SqlStmt ) \
end
#else  // !CSQL_DROP_RULES
#define CSQL_DROP_RULES(X)
#endif // CSQL_DROP_RULES


/// @ingroup Action
/// <summary>
/// Macro to create a type (alias).
/// </summary>
/// <remarks>
/// The type is only created if it doesn't already exist.
/// </remarks>
/// <param name="X">
/// The (schema qualified) name of the type 
/// </param>
/// <param name="E">
/// The type expression.
/// </param>
/// @example CreateType.csql
#ifdef CSQL_CREATE_TYPES
#define CSQL_CREATE_TYPE( X, E ) \
if not exists ( select 1 from sys.types where name = parsename( #@X, 1 ) and is_user_defined = 1 ) \
begin \
	declare @SqlStmt nvarchar( max ) \
	print 'create type ' + #@X \
	set @SqlStmt = 'create type ' + #@X + ' from ' + __QUOTE( E ) \
	exec( @SqlStmt ) \
end
#else  // !CSQL_CREATE_TYPES
#define CSQL_CREATE_TYPES(X, E)
#endif // CSQL_CREATE_TYPES


/// <summary>
/// Macro to drop a type previously created by <code>create type</code>
/// </summary>
/// <remarks>
/// If the TYPE is used i.e. bound to a table attribute or user defined 
/// type the server will raise an error.
/// </remarks>
/// <param name="X">
/// The (schema qualified) name of the type. 
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_TYPES
#define CSQL_DROP_TYPE( X ) \
if exists ( select 1 from sys.types where name = parsename( #@X, 1 ) and is_user_defined = 1 ) \
begin \
	declare @SqlStmt nvarchar( max ) \
	set @SqlStmt = 'drop type ' + #@X \
	print @SqlStmt \
	exec( @SqlStmt ) \
end
#else  // !CSQL_DROP_TYPES
#define CSQL_DROP_TYPES(X)
#endif // CSQL_DROP_TYPES


/// <summary>
/// Macro to drop a table if it exists.
/// </summary>
/// <param name="X">
/// The qualified name of the table. This may include the schema
/// in which the table is created.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_TABLES
#define CSQL_DROP_TABLE(X) \
if exists (select 1 from sys.objects where type ='U' and object_id = object_id( #@X ) ) \
begin \
if exists (select 1 from sys.objects where type ='P' and object_id = object_id( 'dbo.pUtilDropRefKeys' ) ) \
	exec dbo.pUtilDropRefKeys #@X, 1 \
print 'drop table ' + #@X \
drop table X \
end
#else  // !CSQL_DROP_TABLES
#define CSQL_DROP_TABLE(X)
#endif // CSQL_DROP_TABLES


/// <summary>
/// Macro to drop a table in the temporary database (tempdb) if it exists.
/// </summary>
/// <param name="X">
/// The qualified name of the table. This may include the schema
/// in which the table is created.
/// </param>
/// @ingroup Action
#define CSQL_DROP_TEMP_TABLE(X) \
if exists (select 1 from tempdb.dbo.sys.objects where type ='U' and name = parsename( #@X, 1) ) \
begin \
print 'drop temp table ' + #@X \
drop table tempdb..X \
end


/// <summary>
/// Macro to copy a whole table in the current database into a
/// new table in the tempdb database.
/// </summary>
/// <param name="X">
/// The qualified name of the table. This may include the schema
/// in which the table is created.
/// </param>
/// <remarks>
/// <strong>Note:</strong> The macro name may be misleading. The tempdb is not
/// the place to create real backups because it is deleted with every server
/// reboot. This macro is intended for upgrade scripts where you have to
/// perform some sophisticated changes to an existing table. Use this macro
/// to copy the current data into the tempdb. Recreate your persistent table
/// and reinsert the data from the &quot;backup&quot;.
/// </remarks>
/// @ingroup Action
#define CSQL_BACKUP_TABLE(X) \
if exists (select 1 from sys.objects where type ='U' and object_id = object_id(#@X) ) \
begin \
print 'backup table ' + #@X \
select * into tempdb..X from X \
end \
else \
print 'Can''t backup table data from ' + #@X + '. Table doesn''t exist.' \
end


/// <summary>
/// Macro to drop a view if it exists.
/// </summary>
/// <param name="X">
/// The qualified name of the view. This may include the schema
/// in which the view is created.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_VIEWS
#define CSQL_DROP_VIEW(X) \
if exists (select 1 from sys.objects where type ='V' and object_id = object_id(#@X) ) \
begin \
print 'drop view ' + #@X \
drop view X \
end
#else // !CSQL_DROP_VIEWS
#define CSQL_DROP_VIEW(X)
#endif // CSQL_DROP_VIEWS


/// <summary>
/// Create or alter a view.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the view.
/// </param>
/// @ingroup Action
#define CSQL_CREATE_VIEW(X) \
if not exists (select 1 from sys.objects where type ='V' and object_id = object_id(#@X)) \
begin \
print 'create view ' + #@X \
exec( 'create view ' + #@X + ' as select ''Forward implementation only'' as Forward') \
end \
else \
begin \
print 'alter view ' + #@X \
end \
go \
alter view X


/// <summary>
/// Macro to drop a procedure if it exists.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the procedure.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_PROCEDURES
#define CSQL_DROP_PROCEDURE(X) \
if exists (select 1 from sys.objects where type ='P' and object_id = object_id(#@X) ) \
begin \
print 'drop procedure ' + #@X \
drop procedure X \
end
#else  // !CSQL_DROP_PROCEDURES
#define CSQL_DROP_PROCEDURE(X)
#endif // CSQL_DROP_PROCEDURES


/// <summary>
/// Create or alter a procedure.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the procedure.
/// </param>
/// @ingroup Action
#define CSQL_CREATE_PROCEDURE(X) \
if not exists (select 1 from sys.objects where type ='P' and object_id = object_id(#@X)) \
begin \
	print 'create procedure ' + #@X \
	exec( 'create procedure ' + #@X + ' as raiserror 50000 ''Forward implementation only''') \
	end \
	else \
	begin \
	print 'alter procedure ' + #@X \
end \
go \
alter procedure X


/// <summary>
/// Macro to drop a function if it exists.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the function.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_FUNCTIONS
#define CSQL_DROP_FUNCTION(X) \
if exists (select 1 from sys.objects where type in ('FN', 'IF', 'TF') and object_id = object_id(#@X) ) \
begin \
print 'drop function ' + #@X \
drop function X \
end
#else  // !CSQL_DROP_FUNCTIONS
#define CSQL_DROP_FUNCTION(X)
#endif // CSQL_DROP_FUNCTIONS


/// <summary>
/// Macro to create a function returning a scalar value.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the function.
/// </param>
/// @ingroup Action
#define CSQL_CREATE_FUNCTION(X) \
if not exists (select 1 from sys.objects where type ='FN' and object_id = object_id(#@X)) \
begin \
	print 'create function ' + #@X \
	exec( 'create function ' + #@X + '() returns binary as begin return 0x; end') \
end \
else \
begin \
	print 'alter function ' + #@X \
end \
go \
alter function X


/// <summary>
/// Macro to create a function returning a scalar value.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the function.
/// </param>
/// @ingroup Action
#define CSQL_CREATE_SCALAR_FUNCTION(X) \
if not exists (select 1 from sys.objects where type ='FN' and object_id = object_id(#@X)) \
begin \
	print 'create function ' + #@X \
	exec( 'create function ' + #@X + '() returns binary as begin return 0x; end') \
end \
else \
begin \
	print 'alter function ' + #@X \
end \
go \
alter function X


/// <summary>
/// Macro to create a function returning a temporary table (inline).
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the function.
/// </param>
/// @ingroup Action
/// @example CreateInlineFunction.csql
#define CSQL_CREATE_INLINE_FUNCTION(X) \
if not exists ( select 1 from sys.objects where type ='IF' and object_id = object_id(#@X) ) \
begin \
	print 'create function ' + #@X \
	exec( 'create function ' + #@X + '() returns table as return ( select ''X'' as Forward )') \
end \
else \
begin \
	print 'alter function ' + #@X \
end \
go \
alter function X


/// <summary>
/// Macro to create a function returning a temporary table.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the function.
/// </param>
/// @ingroup Action
#define CSQL_CREATE_TABLE_FUNCTION(X) \
if not exists ( select 1 from sys.objects where type ='TF' and object_id = object_id(#@X) ) \
begin \
	print 'create function ' + #@X \
	exec( 'create function ' + #@X + '() returns @t table( x binary ) as begin return; end') \
end \
else \
begin \
	print 'alter function ' + #@X \
end \
go \
alter function X


/// <summary>
/// Macro to drop a synonym if it exists.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the synonym.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_SYNONYMS
#define CSQL_DROP_SYNONYM(X) \
if exists (select 1 from sys.objects where type = 'SN' and object_id = object_id(#@X) ) \
begin \
print 'drop synonym ' + #@X \
drop synonym X \
end
#else  // !CSQL_DROP_SYNONYMS
#define CSQL_DROP_SYNONYM(X)
#endif // CSQL_DROP_SYNONYMS


/// <summary>
/// Macro to drop a trigger with the specified name.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the trigger.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_TRIGGERS
#define CSQL_DROP_TRIGGER( X ) \
if exists (select 1 from sys.triggers where object_id = object_id(#@X) ) \
begin \
print 'drop trigger ' + #@X \
drop trigger X \
end
#else  // !CSQL_DROP_TRIGGERS
#define CSQL_DROP_TRIGGER(X)
#endif // CSQL_DROP_TRIGGERS


/// <summary>
/// Disable a trigger.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the trigger.
/// </param>
/// <remarks>
/// If the trigger doesn't exist or is not enabled the macro does nothing.
/// </remarks>
/// @ingroup Action
#define CSQL_DISABLE_TRIGGER( X ) \
if exists ( select 1 from sys.triggers where object_id = object_id( #X ) and is_disabled = 0 ) \
begin \
	declare @Schema sysname, @Table sysname, @Trigger sysname, @SqlStmt nvarchar( max ), @MsgText nvarchar(max); \
	select @Schema  = schema_name( schema_id ) \
	     , @Table   = object_name( parent_object_id ) \
	     , @Trigger = name \
	  from sys.objects \
	 where object_id = object_id( #X ); \
	set @SqlStmt = 'alter table ' + @Schema + '.' + @Table + ' disable trigger ' + @Trigger; \
	set @MsgText = 'Disable trigger ' + @Trigger; \
	print @MsgText; \
	exec( @SqlStmt ); \
end


/// <summary>
/// Enable a trigger.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the trigger.
/// </param>
/// <remarks>
/// If the trigger doesn't exist or is not disabled the macro does nothing.
/// </remarks>
/// @ingroup Action
#define CSQL_ENABLE_TRIGGER( X ) \
if exists ( select 1 from sys.triggers where object_id = object_id( #X ) and is_disabled = 1 ) \
begin \
	declare @Schema sysname, @Table sysname, @Trigger sysname, @SqlStmt nvarchar( max ), @MsgText nvarchar(max); \
	select @Schema  = schema_name( schema_id ) \
	     , @Table   = object_name( parent_object_id ) \
	     , @Trigger = name \
	  from sys.objects \
	 where object_id = object_id( #X ); \
	set @SqlStmt = 'alter table ' + @Schema + '.' + @Table + ' enable trigger ' + @Trigger; \
	set @MsgText = 'Enable trigger ' + @Trigger; \
	print @MsgText; \
	exec( @SqlStmt ); \
end


/// <summary>
/// Revoke database access for the specified user (drop the user).
/// </summary>
/// <param name="X">
/// The name of the user.
/// </param>
/// @ingroup Action
#define CSQL_DROP_USER( X ) \
if exists ( select name from sysusers where uid != gid and name = #@X and name not like '%[_]role' ) \
begin \
print 'drop user ' + #@X \
exec sp_dropuser #@X \
end


/// <summary>
/// Revoke server access for the specified user (drop the login).
/// </summary>
/// <param name="X">
/// The name of the user.
/// </param>
/// @ingroup Action
#define CSQL_DROP_LOGIN( X ) \
if exists ( select name from master..syslogins where name = #@X ) \
begin \
print 'drop login ' + #@X \
exec sp_droplogin #@X \
end


/// <summary>
/// Macro to drop a primary key constraint specified by its (qualified) name.
/// </summary>
/// <param name="X">
/// The (schema qualified) of the contraint.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_PRIMARY_KEYS
#define CSQL_DROP_PK_CONSTRAINT( X ) \
if exists (select 1 from sys.objects where object_id = object_id( #@X ) and type = 'PK')\
begin\
	declare @MsgText varchar(255), @SqlStmt varchar(255), @TblName sysname, @KeyName sysname  \
	select @TblName = schema_name( schema_id ) + '.' + object_name( parent_object_id ) \
	     , @KeyName = name \
	  from sys.objects \
	 where object_id = object_id( #@X ) and type = 'PK' \
	set @MsgText = 'drop primary key ' + @TblName + '.' + #@X \
	print @MsgText; \
	set @SqlStmt = 'alter table ' + @TblName + ' drop constraint ' + @KeyName; \
	exec( @SqlStmt ) \
end
#else // !CSQL_DROP_PRIMARY_KEYS
#define CSQL_DROP_PK_CONSTRAINT( X )
#endif // CSQL_DROP_PRIMARY_KEYS


/// <summary>
/// Macro to drop a synonym primary key constraint specified by the
/// (qualified) table name it is defined for.
/// </summary>
/// <param name="X">
/// The (schema qualified) of the table.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_PRIMARY_KEYS
#define CSQL_DROP_PRIMARY_KEY( X ) \
if exists ( select 1 from sys.objects where object_id = object_id( #@X ) and type = 'U' )\
begin \
	declare @MsgText varchar(255), @SqlStmt varchar(255), @PkName sysname \
	select @PkName = name \
	  from sys.objects \
	 where parent_object_id = object_id( #@X ) \
	   and type = 'PK' \
	if @@rowcount > 0 \
	begin \
		set   @MsgText = 'drop primary key ' + #@X + '.' + @PkName \
		print @MsgText \
		set @SqlStmt = 'alter table ' + #@X + ' drop constraint ' + @PkName \
		exec( @SqlStmt ) \
	end \
end
#else // !CSQL_DROP_PRIMARY_KEYS
#define CSQL_DROP_PRIMARY_KEY( X )
#endif // CSQL_DROP_PRIMARY_KEYS


/// <summary>
/// Macro to drop a foreign key constraint specified by its (qualified) name.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the constraint.
/// </param>
/// @ingroup Action
/// @example DropForeignKey.csql
#ifdef CSQL_DROP_FOREIGN_KEYS
#define CSQL_DROP_FK_CONSTRAINT( X ) \
if exists (select 1 from sys.objects where object_id = object_id( #@X )and type = 'F')\
begin\
	declare @MsgText varchar(255), @SqlStmt varchar(255), @TblName sysname, @ConstraintName sysname \
	select @TblName        = schema_name( schema_id ) + '.' + object_name(parent_object_id)\
	     , @ConstraintName = name \
	  from sys.objects \
	 where object_id = object_id( #@X ) and type = 'F' \
	set @MsgText = 'drop foreign key ' + @TblName + '.' + @ConstraintName \
	print @MsgText \
	set @SqlStmt = 'alter table ' + @TblName + ' drop constraint ' + @ConstraintName \
	exec( @SqlStmt ) \
end
#else // !CSQL_DROP_FOREIGN_KEYS
#define CSQL_DROP_FK_CONSTRAINT( X )
#endif // CSQL_DROP_FOREIGN_KEYS


/// <summary>
/// Macro to drop a foreign key constraint specified the table
/// that defines the key and the referenced table.
/// </summary>
/// <param name="Detail">
/// The qualified name of the table for which the constraint is defined.
/// </param>
/// <param name="Master">
/// The (schema qualified) name of the referenced table.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_FOREIGN_KEYS
#define CSQL_DROP_FOREIGN_KEY( Detail, Master ) \
if exists ( select 1 from sysforeignkeys where fkeyid = object_id( #Detail ) and rkeyid  = object_id( #Master ) ) \
begin\
	declare @MsgText varchar(255), @SqlStmt varchar(255), @Constraint sysname \
	select @Constraint = object_name(constid) \
	  from sysforeignkeys \
	 where fkeyid = object_id( #Detail ) and rkeyid = object_id( #Master ) \
	set @MsgText = 'drop foreign key ' + #Detail + '-->' + #Master + ' (' + @Constraint + ')' \
	print @MsgText \
	set @SqlStmt = 'alter table ' + #Detail + ' drop constraint ' + @Constraint \
	exec( @SqlStmt ) \
end
#else // !CSQL_DROP_FOREIGN_KEYS
#define CSQL_DROP_FOREIGN_KEY( Detail, Master )
#endif // CSQL_DROP_FOREIGN_KEYS

/// <summary>
/// Drop all foreign key constraints defined for the specified table.
/// </summary>
/// <param name="Detail">
/// The qualified name of the table for which the constraints are defined.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_FOREIGN_KEYS
#define CSQL_DROP_ALL_FOREIGN_KEYS( Detail ) \
declare c cursor local forward_only for \
	select ' drop constraint ' + #@Detail + '.' + k.name \
	     , 'alter table ' + #@Detail \
	     + ' drop constraint ' + k.name \
	  from sys.foreign_keys k \
	 where k.parent_object_id = object_id( #@Detail ) \
declare @Message nvarchar(max), @SqlStmt nvarchar(max) \
open c \
fetch c into @Message, @SqlStmt \
while @@fetch_status = 0 \
begin \
	print @Message \
	exec( @SqlStmt ) \
	fetch c into @Message, @SqlStmt \
end \
close c \
deallocate c 
#else // !CSQL_DROP_FOREIGN_KEYS
#define CSQL_DROP_ALL_FOREIGN_KEYS( Detail )
#endif // CSQL_DROP_FOREIGN_KEYS


/// <summary>
/// Macro to drop an index spefied by the table and index name.
/// </summary>
/// <param name="TblName">
/// The (schema qualified) name of the table for which the index is defined.
/// </param>
/// <param name="IdxName">
/// The name of the index.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_INDEXES
#define CSQL_DROP_INDEX( TblName, IdxName ) \
if exists (select 1 from sys.indexes where name = #IdxName and object_id = object_id( #TblName ) ) \
begin \
	declare @MsgText varchar(255) \
	set @MsgText = 'drop index ' + #TblName + '.' + #IdxName \
	print @MsgText \
	drop index TblName.IdxName \
end
#else // !CSQL_DROP_INDEXES
#define CSQL_DROP_INDEX( TblName, IdxName  )
#endif // CSQL_DROP_INDEXES


/// <summary>
/// Drop all indexes defined for the specified table and which 
/// are not enforcing a unique or primary key contraint.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the table for which to drop the indexes.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_INDEXES
#define CSQL_DROP_ALL_INDEXES( X ) \
declare c cursor local forward_only for \
	select 'drop index ' + #X + '.' + i.name as SqlStmt \
	 from sys.indexes as i \
	 where i.object_id = object_id( #X ) \
	   and i.is_unique_constraint = 0 \
	   and i.is_primary_key = 0 \
declare @Message nvarchar(max), @SqlStmt nvarchar(max) \
open c \
fetch c into @SqlStmt \
while @@fetch_status = 0 \
begin \
	print @SqlStmt \
	exec( @SqlStmt ) \
	fetch c into @SqlStmt \
end \
close c \
deallocate c 
#else // !CSQL_DROP_INDEXES
#define CSQL_DROP_ALL_INDEXES( X )
#endif // CSQL_DROP_INDEXES




/// <summary>
/// Macro to drop a unique constraint specified by its (qualified) name.
/// </summary>
/// <param name="X">
/// The (schema qualified) name of the contraint.
/// </param>
/// @ingroup Action
#ifdef CSQL_DROP_INDEXES
#define CSQL_DROP_UNIQUE_CONSTRAINT( X ) \
if exists (select 1 from sys.objects where object_id = object_id( #@X ) and type = 'UQ')\
begin\
	declare @MsgText varchar(255), @SqlStmt nvarchar(512), @TblName sysname, @Constraint sysname \
	select @TblName    = schema_name( schema_id ) + '.' + object_name( parent_object_id ) \
	     , @Constraint = name \
	  from sys.objects \
	 where object_id = object_id( #@X ) and type = 'UQ' \
	set @MsgText = 'drop unique constraint ' + @TblName + '.' + @Constraint \
	print @MsgText \
	set @SqlStmt = 'alter table ' + @TblName + ' drop constraint ' + @Constraint \
	exec( @SqlStmt ) \
end
#else
#define CSQL_DROP_UNIQUE_CONSTRAINT( X )
#endif

/// <summary>
/// Obsolete synonym for <see cref="CSQL_DROP_UNIQUE_CONSTRAINT" /> 
/// </summary>
/// @deprecated Use <see cref="CSQL_DROP_UNIQUE_CONSTRAINT" /> instead.
/// @ingroup Action
#define CSQL_DROP_UQ_CONSTRAINT( X ) CSQL_DROP_UNIQUE_CONSTRAINT( X )


#endif // CSQL_UNDEF_ALL

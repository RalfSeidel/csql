/**
** @file
** @author Ralf Seidel
** @brief Declaration of the pre processor directives.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_DIRECTIVE_H
#define SQTPP_DIRECTIVE_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Token.h"

namespace sqtpp {

/**
** @brief Enumeratiom of all preprocessor directives.
*/
enum Directive {
	/// Undefined directive / initial value.
	DIR_UNDEFINED,
	/// define
	DIR_DEFINE,
	/// undef
	DIR_UNDEF,
	/// undefall
	DIR_UNDEFALL,

	/// if
	DIR_IF,
	/// ifdef
	DIR_IFDEF,
	/// ifndef
	DIR_IFNDEF,
	/// elif
	DIR_ELIF,
	/// else
	DIR_ELSE,
	/// endif
	DIR_ENDIF,

	/// include
	DIR_INCLUDE,
 
	/// line
	DIR_LINE,
	/// error
	DIR_ERROR,

	/// pragma
	DIR_PRAGMA,
	/// import
	DIR_IMPORT,
	/// using
	DIR_USING,

	/// message
	DIR_MESSAGE,

	/// exec
	DIR_EXEC
};

/**
** @brief A pre processor directive.
*/
class DirectiveInfo
{
public:
	// Undefined / Unknown diretive.
	static const DirectiveInfo Undefined;

	// define
	static const DirectiveInfo Define;
	// undef
	static const DirectiveInfo Undef;
	// undefall
	static const DirectiveInfo Undefall;

	// if
	static const DirectiveInfo If;
	// ifdef
	static const DirectiveInfo Ifdef;
	// ifndef
	static const DirectiveInfo Ifndef;
	// elif
	static const DirectiveInfo Elif;
	// else
	static const DirectiveInfo Else;
	// endif
	static const DirectiveInfo Endif;

	// include
	static const DirectiveInfo Include;
 
	// line
	static const DirectiveInfo Line;
	// error
	static const DirectiveInfo Error;

	// pragma
	static const DirectiveInfo Pragma;
	// import
	static const DirectiveInfo Import;
	// using
	static const DirectiveInfo Using;

	// message
	static const DirectiveInfo Message;

	// exec
	static const DirectiveInfo Exec;

private:
	// All directives.
	static const DirectiveInfo* m_directives[];

	// The id of the directive.
	Directive     m_directive;

	Token         m_token;

	// The directive identifier.
	const wstring m_identifier;


private:
	// Copy c'tor (not implemented)
	DirectiveInfo( const DirectiveInfo& that );
	// Assignment operator (not implemented)
	DirectiveInfo& operator=( const DirectiveInfo& that );
public:
	// Initializing constructor.
	DirectiveInfo( Directive directive, Token token, const wchar_t* text );
	// Destructor.
	~DirectiveInfo() throw();

	// Get directive of given identifier.
	static const DirectiveInfo* findDirectiveInfo( const wstring& identifier );

	// Get directive of given identifier.
	static const Directive getDirective( const wstring& identifier );

	// Get the directive id.
	Directive getDirective() const throw() { return m_directive; }

	// Get corresponding scanner token of the directive.
	Token getToken() const throw()         { return m_token; }

	// Get the directive identifier.
	const wstring& getIdentifier() const throw() { return m_identifier; }

	// Identifier comparison.
	bool operator== ( const wchar_t* text   ) const;
	// Identifier comparison.
	bool operator== ( const wstring& text   ) const;
	// Id comparison.
	bool operator== ( const DirectiveInfo& that ) const;
};


} // namespace

#endif // SQTPP_DIRECTIVE_H

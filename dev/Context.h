/**
** @file
** @author Ralf Seidel
** @brief Declaration of the scanner contexts.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_CONTEXT_H
#define SQTPP_CONTEXT_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

/**
** @brief Current context of the scanner.
*/
enum Context 
{
	/// Undefined context.
	CTX_UNDEFINED,
	/// Default context (no special token has been found so far). 
	CTX_DEFAULT,
	/// A block comment was found. Read everything
	/// insided the comment until the block comment
	/// end has been found.
	CTX_BLOCK_COMMENT,
	/// A line comment was found. Read everything
	/// until the end of line has been found. 
	CTX_LINE_COMMENT,
	/// We are reading a string. If multiy line string support is
	/// enabled you may see this context when a new line is encountered.
	CTX_SQUOTE_STRING,
	CTX_DQUOTE_STRING,
	/// A directive has been found and processed. Now 
	/// scan everything until the end of the line.
	CTX_FINISH_DIRECTIVE,
	/// This context will be set if the \#include directive has been found.
	/// The only difference to the default context is that the scanner
	/// will recognise < and > as a string delimiter.
	CTX_INCLUDE_DIRECTIVE,
	/// A conditional has been evaluated as false.
	/// Scan conditionals only until the end of the
	/// current conditional has been found.
	CTX_CONDITIONAL_FALSE,
	/// A conditional has been evaluated as true.
	/// Ignore any subsequent \#elif or #\else.
	CTX_CONDITIONAL_DONE
};

/**
** @brief Debugging informations about the defined Contexts.
*/
class ContextInfo 
{
public:
	// The Context id.
	const Context  context;
	// The enumeration symbol.
	const wchar_t* pwszSymbol;
	// A descriptiv text / name.
	const wchar_t* pwszDescription;

	// Initialising constructor.
	ContextInfo( Context context, const wchar_t* const pwszSymbol, const wchar_t* const pwszDescription );

	// Get information about the given context.
	static const ContextInfo& getContextInfo( Context context );

private:
	// Assignment operator (not implemented).
	ContextInfo& operator= ( const ContextInfo& that );
};


} // namespace

#endif // SQTPP_CONTEXT_H

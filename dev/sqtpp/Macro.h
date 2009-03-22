/**
** @file
** @author Ralf Seidel
** @brief Declaration of the #sqtpp::Macro class.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_MACRO_H
#define SQTPP_MACRO_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Token.h"

namespace sqtpp {

class Options;
class Processor;
class Location;
class Macro;
class MacroExpander;
class MacroSet;
class MacroArgument;
class MacroArguments;
class MacroArgumentValues;


/**
** @brief A macro agrgument.
**
** A macro argument is not much more than a identifier that is just 
** a simple string.
*/
class MacroArgument
{
private:
	// The argument name.
	std::wstring m_sIdentifier;
public:
	// The constructor.
	MacroArgument( const std::wstring& identifier );

	// Get the argument name / identifier.
	const std::wstring& getIdentifier() const throw() { return m_sIdentifier; }
};


/**
** @brief Collection of macro arguments.
*/
class MacroArguments : public std::vector<MacroArgument>
{
public:
	// Get the index of the arguement.
	int getArgumentIndex( const wstring& identifier ) const throw();

	const_iterator find( const wstring& identifier ) const throw();
};

/**
** @brief Class responsible for macro expansion.
**
** 
*/
class MacroExpander
{
private:
	static MacroExpander m_instance;
protected:
	MacroExpander() {}
	virtual ~MacroExpander() {}

protected:
	// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result );
	// Expand macros in the macro argument list.
	virtual void expandArguments( const Processor& processor, const MacroArgumentValues& argumentValues, MacroArgumentValues& result );

	virtual void expandIndentifier( const Processor& processor, const TokenExpressions& tokenExpressions, TokenExpressions::const_iterator& itToken, TokenExpressions& result );
public:
	static MacroExpander& getInstance() throw() { return m_instance; }

friend class Macro;
};



/**
** @brief A macro.
*/
class Macro
{
private:
	/// The macro identifier.
	std::wstring     m_sIdentifier;

	/// The location where the macro has been defined.
	std::wstring     m_sDefFile;

	/// The file where the macro was defined.
	size_t           m_nDefLine;

	/// The whole macro expression as it was found in the source.
	/// This variable is not used by the preprocessor. It is a debugging aid only.
	std::wstring     m_sDefText;

	/// The macro arguments.
	MacroArguments   m_arguments;

	/// Is this macro defined with an argument list?
	bool             m_hasArgs;

	/// Does this macro have a variable argument list?
	bool             m_hasVarArgs;

	/// Is the macro a build in / predefined macro which cannot be overriden by preprocessor \#define or \#undef?
	bool             m_isBuildin;

	/// Does with macro expanded in more than one line.
	bool             m_isMultiLine;

	/// Check if this macro is currently expanding (Macros where this flag is set will not expand).
	bool             m_isExpanding;

	/// The tokenized expression.
	TokenExpressions m_tokens;

	/// The object responsible for expanding the macro.
	MacroExpander*   m_pMacroExpander;

public:
	// The default constructor.
	Macro();

	// The constructor.
	Macro( const wstring& identifier, const wstring& file, const size_t line );

protected:
	// The constructor for buildin macros.
	Macro( const wchar_t* identifier, MacroExpander* pMacroExpander );

public:

	// The copy constructor.
	//Macro( const Macro& that );

	// The copy constructor.
	//Macro& operator=( const Macro& that );

	// The destructor.
	~Macro() throw() {}

	/// Get the macro name / identifier.
	const std::wstring& getIdentifier() const throw() { return m_sIdentifier; }

	/// Get the file where the macro has been defined.
	const std::wstring& getDefineFile() const throw() { return m_sDefFile; }

	/// Get the line number in which the macro has been defined.
	const size_t        getDefineLine() const throw() { return m_nDefLine; }

	/// Get the text of the macro definition.
	const std::wstring& getDefineText() const throw() { return m_sDefText; }

	/// Get all arguments.
	const MacroArguments& getArguments() const throw() { return m_arguments; }

	/// Get all macro tokens.
	const TokenExpressions& getTokens() const throw() { return m_tokens; }

	/// Check if this macro has a variable number of arguments.
	bool hasArguments() const throw() { return m_hasArgs; }

	/// Check if this macro has a variable number of arguments.
	bool hasVarArgs() const throw() { return m_hasVarArgs; }

	/// Set flag indicating if the macro has a variable arguement list or no.
	void hasVarArgs( bool bHasVarArgs ) throw() { m_hasVarArgs = bHasVarArgs; }

	/// Is the macro a build in / predefined macro which cannot be overriden by preprocessor \#define or \#undef?
	bool isBuildin() const throw()    { return m_isBuildin; }

	/// Does this macro expand to more than one line?
	bool isMultiLine() const throw()  { return m_isMultiLine; }

	/// Is the macro a build in / predefined macro which cannot be overriden by preprocessor \#define or \#undef?
	bool isExpanding() const throw()  { return m_isExpanding; }

	/// Set flag indication that the processor is currently expanding this macro.
	void setExpanding( bool bSet ) throw() { m_isExpanding = bSet; }

	/// Set the argument list.
	void setArguments( const MacroArguments& arguments );

	/// Define the macro expression.
	void setExpression( const TokenExpressions& tokens, const wstring& sDefintionText );

	/// Expand the macro.
	void expand( const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result ) const;

	/// Comparison of the macro identifier.
	bool operator==( const Macro& that ) const { return m_sIdentifier == that.m_sIdentifier; }

	/// Comparison of the macro identifier.
	bool operator!=( const Macro& that ) const { return m_sIdentifier != that.m_sIdentifier; }

	/// Comparison of the macro identifier.
	bool operator<( const Macro& that ) const { return m_sIdentifier < that.m_sIdentifier; }

	/// Comparison of the macro identifier.
	bool operator>( const Macro& that ) const { return m_sIdentifier > that.m_sIdentifier; }

	/// Comparison of the macro identifier.
	bool operator<=( const Macro& that ) const { return m_sIdentifier <= that.m_sIdentifier; }

	/// Comparison of the macro identifier.
	bool operator>=( const Macro& that ) const { return m_sIdentifier >= that.m_sIdentifier; }
};



/**
** @brief Set of macros (mapping of identifier to a macro).
**
** 
*/
class MacroSet : public std::map<std::wstring, Macro> 
{
};

/**
** @brief Collection of macro arguments values.
*/
class MacroArgumentValues : public std::vector<TokenExpressions> 
{
};


} // namespace


#endif // SQTPP_MACRO_H

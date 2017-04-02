#include "stdafx.h"
#include <sstream>
#include "Options.h"
#include "Util.h"
#include "Processor.h"
#include "Error.h"
#include "Macro.h"

namespace sqtpp {

// --------------------------------------------------------------------
// MacroSet
// --------------------------------------------------------------------

MacroSet::MacroSet()
{
}


// --------------------------------------------------------------------
// MacroArgument
// --------------------------------------------------------------------

/**
** @brief The constructor.
*/
MacroArgument::MacroArgument( const std::wstring& identifier )
: m_sIdentifier( identifier )
{
}

// --------------------------------------------------------------------
// MacroArgumentValues
// --------------------------------------------------------------------
MacroArgumentValues::MacroArgumentValues()
{
}


// --------------------------------------------------------------------
// MacroArguments
// --------------------------------------------------------------------

/**
** Get the index of the arguement.
** @returns -1 if no argument with given name exisits.
**          Index of argument otherwise
*/
int MacroArguments::getArgumentIndex( const wstring& identifier ) const throw()
{
	for ( size_t i = 0; i < this->size(); ++i ) {
		const MacroArgument& argument = (*this)[i];
		if ( argument.getIdentifier() == identifier )
			return (int)i;
	}
	return -1;
}

MacroArguments::const_iterator MacroArguments::find( const wstring& identifier ) const throw()
{
	const_iterator itResult = begin();
	for ( itResult = begin(); itResult != end(); ++itResult ) {
		const MacroArgument& argument = *itResult;
		if ( argument.getIdentifier() == identifier )
			break;
	}
	return itResult;
}


// --------------------------------------------------------------------
// MacroExpander
// --------------------------------------------------------------------

/**
** @brief Default macro expander singleton.
*/
MacroExpander MacroExpander::m_instance;

namespace {
/**
** @brief Helper for macro expansion.
**
** @todo Better documentation. Unify with Processor::MacroExpansion
*/
class ArgumentTokenStream : public ITokenStream
{
private:
	typedef ITokenStream base;

	TokenExpressions::const_iterator&      it;
	const TokenExpressions::const_iterator itEnd;

	ArgumentTokenStream( const ArgumentTokenStream& );
	ArgumentTokenStream& operator=( const ArgumentTokenStream& that );

public:
	ArgumentTokenStream( const TokenExpressions& tokenExpressions, TokenExpressions::const_iterator& it )
	: it( it )
	, itEnd( tokenExpressions.end() )
	{
		// Dummy comparison asserts iterator compatibility in in VS 2005
		it == itEnd;
	}

	bool  expandMacros() const throw()
	{
		return true;
	}

	/// Just iterate through the tokens an return the next one.
	Token getNextToken( wistream&, TokenExpression& tokenExpression )
	{
		if ( this->it != this->itEnd ) {
			tokenExpression = *this->it;
			this->it++;
			return tokenExpression.token;
		} else {
			return TOK_END_OF_FILE;
		}
	}
};
}

void MacroExpander::expandIndentifier( const Processor& processor, const TokenExpressions& tokenExpressions, TokenExpressions::const_iterator& itToken, TokenExpressions& result )
{
	const TokenExpression& tokenExpression = *itToken;
	const wstring&         identifier      = tokenExpression.getIdentifier();
	const MacroSet&        allMacros       = processor.getMacros();
	MacroSet::const_iterator itMacro = allMacros.find( identifier );
	bool isMacro = itMacro != allMacros.end();
	if ( isMacro ) {
		const Macro& macro = itMacro->second;
		MacroArgumentValues argumentValues;
		TokenExpressions    macroExpansion;
		// Do not expand macros in the argument list which are currently processed
		// to avoid recursion like in the following example:
		//	#define A B
		//	#define B A
		//	A
		// The result will be A: A --> B  --> A (is expanding)
		if ( macro.isExpanding() ) {
			isMacro = false;
		} else if ( macro.hasArguments() ) {
			TokenExpressions::const_iterator itBackup = itToken;
			++itToken;
			ArgumentTokenStream ats( tokenExpressions, itToken );
			isMacro = processor.collectMacroArgumentValues( macro, ats, argumentValues );
			if ( !isMacro ) {
				itToken = itBackup;
			}
		} else {
			++itToken;
		}
		if ( isMacro ) {
			macro.expand( processor, argumentValues, macroExpansion );
			result.insert( result.end(), macroExpansion.begin(), macroExpansion.end() );
		}
	}
	if ( !isMacro ) {
		result.push_back( tokenExpression );
		++itToken;
	}
}

/**
** @brief Expand macros in the macro argument list.
**
** Helper for Expand().
*/
void MacroExpander::expandArguments( const Processor& processor, const MacroArgumentValues& argumentValues, MacroArgumentValues& result )
{
	result.clear();
	for ( MacroArgumentValues::const_iterator itArgument = argumentValues.begin(); itArgument != argumentValues.end(); ++itArgument ) {
		const TokenExpressions& argumentExpressions = *itArgument;
		TokenExpressions expandedTokens;
		for ( TokenExpressions::const_iterator itToken = argumentExpressions.begin(); itToken != argumentExpressions.end(); ) {
			const TokenExpression& tokenExpression = *itToken;
			const Token token = tokenExpression.token;
			if ( token == TOK_IDENTIFIER ) {
				expandIndentifier( processor, argumentExpressions, itToken, expandedTokens );
			} else {
				expandedTokens.push_back( tokenExpression );
				++itToken;
			}
		}
		result.push_back( expandedTokens );
	}
}

/**
** @brief Expand a macro.
**
** The operators ## (concatenate) and # stringyfi will be handeled here.
** The handling of the concate operator is quiet easy. To concate two tokens
** any white space, new line or comment before and after the operator is removed.
*/
void MacroExpander::expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
{
	const MacroSet&            allMacros      = processor.getMacros();
	const MacroArguments&      macroArguments = macro.getArguments();
	const Options&             options        = processor.getOptions();
	const TokenExpressions&    tokens         = macro.getTokens();
	Token                      prevToken      = TOK_UNDEFINED;
	TokenExpressions           interimResult;

	// Expand macros in the arguments.
	MacroArgumentValues expandedArguments;
	expandArguments( processor, argumentValues, expandedArguments );

	const_cast<Macro&>(macro).setExpanding( true );


	try {
		// Replace parameter tokens with argument expressions.
		for ( TokenExpressions::const_iterator itToken = tokens.begin(); itToken != tokens.end(); ++itToken ) {
			const TokenExpression& tokenExpression = *itToken;
			const Token            token           = tokenExpression.token;
			const wstring&         identifier      = tokenExpression.identifier;
			bool                   emitExpression = true;

			switch ( token ) {
			case TOK_EOL_BACKSLASH:
				emitExpression = false;
				break;
			case TOK_SHARP_SHARP:
				while ( interimResult.begin() != interimResult.end() ) {
					const TokenExpression& rtokenExpression = *interimResult.rbegin();
					const Token            rtoken           = rtokenExpression.token;
					if ( rtoken != TOK_SPACE         && rtoken != TOK_NEW_LINE
					  && rtoken != TOK_BLOCK_COMMENT && rtoken != TOK_LINE_COMMENT )
					{
						break;
					}
					interimResult.pop_back();
				}
				if ( interimResult.begin() == interimResult.end() ) {
					// Missing first argument for the \#\# operator.
					throw error::C2160();
				}
				// skip all white space after the '##' operator.
				while ( ++itToken != tokens.end() ) {
					const TokenExpression& expression2 = *itToken;
					const Token            token2           = expression2.token;
					if ( token2 != TOK_SPACE         && token2 != TOK_NEW_LINE
					  && token2 != TOK_BLOCK_COMMENT && token2 != TOK_LINE_COMMENT )
					{
						break;
					}
				}
				if ( itToken == tokens.end() ) {
					// Missing second argument for the \#\# operator.
					throw error::C2161();
				}
				--itToken;

				emitExpression = false;
				break;

			case TOK_SHARP:
			case TOK_SHARP_AT: {
				// Determine string delimiter and delimiter escaping.
				// If the operator is the charizeoperator (\#@) the delimiter is always '. Otherwises it depends
				// on the preprocessor options.
				const wchar_t delimiter     = token == TOK_SHARP_AT ? L'\'' : wchar_t(options.getStringDelimiter());
				const wchar_t escape        = options.getStringQuoting() == Options::QUOT_DOUBLE ? delimiter : L'\\';
				bool          bOperandFound = false;

				// skip all white space after the '#' or '#@' operator.
				while ( ++itToken != tokens.end() ) {
					const TokenExpression& tokenExpression = *itToken;
					const Token            token           = tokenExpression.token;
					if ( token != TOK_SPACE         && token != TOK_NEW_LINE
					  && token != TOK_BLOCK_COMMENT && token != TOK_LINE_COMMENT )
					{
						break;
					}
				}
				if ( itToken != tokens.end() ) {
					const TokenExpression& tokenExpression = *itToken;
					const Token            token           = tokenExpression.getToken();

					if ( token == TOK_IDENTIFIER ) {
						const wstring& identifier     = tokenExpression.getText();
						int            nArgumentIndex = macroArguments.getArgumentIndex( identifier );
						if ( nArgumentIndex >= 0 ) {
							const TokenExpressions& argumentExpressions = argumentValues[nArgumentIndex];
							const wstring stringizedTokens = argumentExpressions.stringize( delimiter, escape );
							TokenExpression replacement( TOK_STRING, tokenExpression.getContext(), stringizedTokens );
							interimResult.push_back( replacement );
							bOperandFound  = true;
						} else {
							MacroSet::const_iterator itNestedMacro = allMacros.find( identifier );

							if ( itNestedMacro != allMacros.end() ) {
								// TODO: allow stringizing of non arguments / macros?
							}
						}
					} else {
						// TODO: allow strinizing of non arguments?
					}

				}

				if ( !bOperandFound ) {
					// Missing second argument for the \#(@) operator.
					if ( token == TOK_SHARP ) {
						// The token following a stringizing operator (#) has to be a macro argument.
						throw error::C2162A();
					} else {
						// The token following a charizing operator (#@) has to be a macro argument.
						throw error::C2162B();
					}
				}
				emitExpression = false;
				break;
			}
			case TOK_IDENTIFIER:
				if ( prevToken != TOK_SHARP && prevToken != TOK_SHARP_AT ) {
					int nArgumentIndex = macroArguments.getArgumentIndex( identifier );
					if ( nArgumentIndex >= 0 ) {
						const TokenExpressions& argumentTokens = expandedArguments[nArgumentIndex];
						interimResult.insert( interimResult.end(), argumentTokens.begin(), argumentTokens.end() );
						emitExpression = false;
					} else {
						// expandIndentifier( processor, tokens, itToken, result );
						//interimResult.push_back( tokenExpression );
						//emitExpression = false;
						//--itToken;
					}
				}
				break;
			case TOK_SPACE:
				if ( !options.multiLineMacroExpansion() ) {
					if ( prevToken != TOK_SPACE ) {
						TokenExpression replacement( TOK_SPACE, tokenExpression.getContext(), L" " );
						interimResult.push_back( replacement );
						prevToken = TOK_SPACE;
					}
					emitExpression = false;
				}
				break;
			case TOK_NEW_LINE:
				if ( !options.multiLineMacroExpansion() ) {
					if ( prevToken != TOK_SPACE ) {
						TokenExpression replacement( TOK_SPACE, tokenExpression.getContext(), L" " );
						interimResult.push_back( replacement );
						prevToken = TOK_SPACE;
					}
					emitExpression = false;
				}
				break;
			case TOK_SQL_LINE_COMMENT:
				emitExpression = options.keepSqlComments();
				break;
			case TOK_LINE_COMMENT:
				emitExpression = options.keepLineComments();
				break;
			} // switch ( token )

			if ( emitExpression ) {
				interimResult.push_back( tokenExpression );
				prevToken = tokenExpression.getToken();
			}
		}
		// Expand nested macros
		for ( TokenExpressions::const_iterator itToken = interimResult.begin(); itToken != interimResult.end(); ++itToken ) {
			const TokenExpression& tokenExpression = *itToken;
			const Token            token           = tokenExpression.token;

			if ( token == TOK_IDENTIFIER ) {
				expandIndentifier( processor, interimResult, itToken, result );
				--itToken;
			} else {
				result.push_back( tokenExpression );
			}
		}
	}
	catch( ... ) {
		const_cast<Macro&>(macro).setExpanding( false );
		throw;
	}

	const_cast<Macro&>(macro).setExpanding( false );
}




// --------------------------------------------------------------------
// Macro
// --------------------------------------------------------------------

Macro::Macro()
: m_nDefLine( 0 )
, m_hasArgs( false )
, m_hasVarArgs( false )
, m_isBuildin( false )
, m_isMultiLine( false )
, m_isExpanding( false )
, m_pMacroExpander( NULL )
{
}


Macro::Macro( const wchar_t* identifier, MacroExpander* pMacroExpander )
: m_sIdentifier( identifier )
, m_nDefLine( 0 )
, m_hasArgs( false )
, m_hasVarArgs( false )
, m_isBuildin( true )
, m_isMultiLine( false )
, m_isExpanding( false )
, m_pMacroExpander( pMacroExpander )
{
}

Macro::Macro( const wstring& identifier, const wstring& file, const size_t line )
: m_sIdentifier( identifier )
, m_sDefFile( file )
, m_nDefLine( line )
, m_hasArgs( false )
, m_hasVarArgs( false )
, m_isBuildin( false )
, m_isMultiLine( false )
, m_isExpanding( false )
, m_pMacroExpander( &MacroExpander::m_instance )
{
}

/**
** @brief Set macro arguements.
*/
void Macro::setArguments( const MacroArguments& arguments )
{
	m_arguments = arguments;
	m_hasArgs  = true;
}

/**
** @brief Define the macro expression.
**
*/
void Macro::setExpression( const TokenExpressions& tokens, const wstring& expressionText )
{
	m_tokens      = tokens;
	m_isMultiLine = false;
	m_sDefText    = Util::trim( expressionText );



	for ( TokenExpressions::const_iterator it = tokens.begin(); it != tokens.end(); ++it ) {
		const TokenExpression& tokex = *it;
		if ( tokex.getToken() == TOK_NEW_LINE ) {
			m_isMultiLine = true;
			break;
		}
	}
}

/**
** @brief Expand the macro.
**
** The methode delegates the macro expansion to its macro expander.
**
** @param processor The preprocessor.
** @param argumentValues The tokens scanned for the arguments.
** @param result The resulting token expressions.
*/
void Macro::expand( const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result) const
{
	m_pMacroExpander->expand( *this, processor, argumentValues, result );

#	if 0 && defined _DEBUG

	wclog << this->getIdentifier().c_str();
	if ( this->hasArguments() ) {
		const wchar_t* pszSeperator = L" ";
		wclog << L"(";
		for ( MacroArgumentValues::const_iterator itArgument = argumentValues.begin(); itArgument != argumentValues.end(); ++itArgument ) {
			wclog << pszSeperator;
			pszSeperator = L", ";
			const TokenExpressions& argumentExpressions = *itArgument;
			for ( TokenExpressions::const_iterator itToken = argumentExpressions.begin(); itToken != argumentExpressions.end(); ++itToken ) {
				const TokenExpression& tokenExpression = *itToken;
				wclog << tokenExpression.getText().c_str();
			}
		}
		if ( argumentValues.size() > 0 ) {
			wclog << L" )";
		} else {
			wclog << L")";
		}
	}
	wclog << L" --> ";

	for ( TokenExpressions::const_iterator itToken = result.begin(); itToken != result.end(); ++itToken ) {
		const TokenExpression& tokenExpression = *itToken;
		wclog << tokenExpression.getText().c_str();
	}
	wclog << endl;

#	endif
}


} // namespace

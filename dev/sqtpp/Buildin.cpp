#include "stdafx.h"
#include <algorithm>
#include <cassert>
#include "Exceptions.h"
#include "Expression.h"
#include "File.h"
#include "Util.h"
#include "Macro.h"
#include "Context.h"
#include "Options.h"
#include "Processor.h"
#include "Buildin.h"


namespace sqtpp {

// --------------------------------------------------------------------
// BuildinMacro
// --------------------------------------------------------------------

BuildinMacro::BuildinMacro( const wchar_t* identifier, MacroExpander& macroExpander )
: base( identifier, &macroExpander )
{
}

void BuildinMacro::addBuildinMacros( const Options& options, MacroSet& macros )
{
	TokenExpression  token;
	TokenExpressions tokens;
	BuildinCounter cntr;
	BuildinDate date;
	BuildinEval eval;
	BuildinFile file;
	BuildinHost host;
	BuildinLine line;
	BuildinQuote quot;
	BuildinTimestamp stmp;
	BuildinTime time;
	BuildinUser user;

	macros[cntr.getIdentifier()] = cntr;
	macros[date.getIdentifier()] = date;
	macros[eval.getIdentifier()] = eval;
	macros[file.getIdentifier()] = file;
	macros[host.getIdentifier()] = host;
	macros[line.getIdentifier()] = line;
	macros[quot.getIdentifier()] = quot;
	macros[stmp.getIdentifier()] = stmp;
	macros[time.getIdentifier()] = time;
	macros[user.getIdentifier()] = user;

	BuildinMacro  language( L"__SQTPP_LANGUAGE", MacroExpander::getInstance() );
	const Options::LanguageInfo& languageInfo = options.getLanguageInfo();
	tokens.clear();
	token = TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, languageInfo.pwszSymbol );
	tokens.push_back( token );
	language.setExpression( tokens, L"" );
	macros[language.getIdentifier()] = language;

	BuildinMacro  language2( languageInfo.pwszMacro, MacroExpander::getInstance() );
	macros[language2.getIdentifier()] = language2;

}


// --------------------------------------------------------------------
// BuildinFile
// --------------------------------------------------------------------

class BuildinFile::BuildinFileExpander : public sqtpp::MacroExpander
{
	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__FILE__" );
		assert( argumentValues.size() == 0 );

		const File&    file      = processor.getFile();
		const Options& options   = processor.getOptions();
		const wchar_t  delimiter = (wchar_t)options.getStringDelimiter();
		wstring        filePath  = file.getPath();
		wstringstream  buffer;


		buffer << delimiter;

		for ( wstring::const_iterator it = filePath.begin(); it != filePath.end(); ++it ) {
			wchar_t ch = *it;
			if ( ch == delimiter ) {
				if ( options.doubleQuoteEscaping() ) {
					buffer << ch << ch;
				} else {
					buffer << L'\\' << ch;
				}
			} else if ( ch == L'\\' ) {
				if ( options.doubleQuoteEscaping() ) {
					// SQL Mode: just insert backslash.
					buffer << ch;
				} else {
					// C Mode: insert backslash escaped.
					buffer << ch << ch;
				}
			} else {
				buffer << ch;
			}
		}

		buffer << delimiter;

		filePath = buffer.str();

		TokenExpression token( TOK_STRING, processor.getContext(), filePath );

		result.push_back( token );
	}
};

BuildinFile::BuildinFileExpander BuildinFile::m_expander;

BuildinFile::BuildinFile()
: base( L"__FILE__", BuildinFile::m_expander )
{
}

// --------------------------------------------------------------------
// BuildinLine
// --------------------------------------------------------------------

class BuildinLine::BuildinLineExpander : public sqtpp::MacroExpander
{
	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__LINE__" );
		assert( argumentValues.size() == 0 );

		const File&  file  = processor.getFile();
		size_t       nLine = file.getLine();
		wstring      sLine = lexical_cast<wstring>( (int)nLine );

		TokenExpression token( TOK_STRING, processor.getContext(), sLine );

		result.push_back( token );
	}
};

BuildinLine::BuildinLineExpander BuildinLine::m_expander;

BuildinLine::BuildinLine()
: base( L"__LINE__", BuildinLine::m_expander )
{
}


// --------------------------------------------------------------------
// BuildinDate
// --------------------------------------------------------------------

class BuildinDate::BuildinDateExpander : public sqtpp::MacroExpander
{
	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__DATE__" );
		assert( argumentValues.size() == 0 );

		const Options& options    = processor.getOptions();
		const wstring  sFormat    = options.getDateFormat();
		wchar_t        wcBuffer[128];
		tm             localTime;

		processor.getTimestamp( localTime );


		// Convert date to string.
		size_t length = wcsftime( wcBuffer, sizeof(wcBuffer) / sizeof( wchar_t ), sFormat.c_str(), &localTime );
		if ( length == 0 ) {
			throw RuntimeError( "The time format is to long." );
		}

		wstring sDate( wcBuffer, length );
		TokenExpression token( TOK_STRING, processor.getContext(), sDate );

		result.push_back( token );
	}
};

BuildinDate::BuildinDateExpander BuildinDate::m_expander;

BuildinDate::BuildinDate()
: base( L"__DATE__", BuildinDate::m_expander )
{
}


// --------------------------------------------------------------------
// BuildinTime
// --------------------------------------------------------------------

class BuildinTime::BuildinTimeExpander : public sqtpp::MacroExpander
{
	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__TIME__" );
		assert( argumentValues.size() == 0 );

		const Options& options    = processor.getOptions();
		const wstring  sFormat    = options.getTimeFormat();
		wchar_t        wcBuffer[128];
		tm             localTime;

		processor.getTimestamp( localTime );

		// Convert date to string.
		size_t length = wcsftime( wcBuffer, sizeof(wcBuffer) / sizeof( wchar_t ), sFormat.c_str(), &localTime );
		if ( length == 0 ) {
			throw RuntimeError( "The time format is to long." );
		}

		wstring sTime( wcBuffer, length );
		TokenExpression token( TOK_STRING, processor.getContext(), sTime );

		result.push_back( token );
	}
};

BuildinTime::BuildinTimeExpander BuildinTime::m_expander;

BuildinTime::BuildinTime()
: base( L"__TIME__", BuildinTime::m_expander )
{
}


// --------------------------------------------------------------------
// BuildinTimestamp
// --------------------------------------------------------------------

class BuildinTimestamp::BuildinTimestampExpander : public sqtpp::MacroExpander
{
	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__TIMESTAMP__" );
		assert( argumentValues.size() == 0 );

		const Options& options    = processor.getOptions();
		const wstring  sFormat    = options.getTimestampFormat();
		wchar_t        wcBuffer[128];
		tm             localTime;

		processor.getTimestamp( localTime );

		// Convert date to string.
		size_t length = wcsftime( wcBuffer, sizeof(wcBuffer) / sizeof( wchar_t ), sFormat.c_str(), &localTime );
		if ( length == 0 ) {
			throw RuntimeError( "The time format is to long." );
		}

		wstring sTimestamp( wcBuffer, length );
		TokenExpression token( TOK_STRING, processor.getContext(), sTimestamp );

		result.push_back( token );
	}
};

BuildinTimestamp::BuildinTimestampExpander BuildinTimestamp::m_expander;

BuildinTimestamp::BuildinTimestamp()
: base( L"__TIMESTAMP__", BuildinTimestamp::m_expander )
{
}


// --------------------------------------------------------------------
// BuildinUser
// --------------------------------------------------------------------

class BuildinUser::BuildinUserExpander : public sqtpp::MacroExpander
{
	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__USER__" );
		assert( argumentValues.size() == 0 );

		const Options& options = processor.getOptions();

#		ifdef _WIN32
		const wchar_t* pwzsEnvironment = _wgetenv( L"USERNAME" );
#		else
#		error Adjust environment variable.
#		endif

		wstring sUser( pwzsEnvironment );
		wstring sDelimiter;

		if ( sUser.empty() ) {
			sUser = L"unkown";
		}

		switch ( options.getStringDelimiter() ) {
			case Options::STRD_DOUBLE:
				sDelimiter = L"\"";
				break;
			case Options::STRD_SINGLE:
				sDelimiter = L"\'";
				break;
			default:
				throw UnexpectedSwitchError();
		}
		sUser = sDelimiter + sUser + sDelimiter;
		TokenExpression token( TOK_STRING, processor.getContext(), sUser );

		result.push_back( token );
	}
};

BuildinUser::BuildinUserExpander BuildinUser::m_expander;

BuildinUser::BuildinUser()
: base( L"__USER__", BuildinUser::m_expander )
{
}

// --------------------------------------------------------------------
// BuildinHost
// --------------------------------------------------------------------
class BuildinHost::BuildinHostExpander : public sqtpp::MacroExpander
{
	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__HOST__" );
		assert( argumentValues.size() == 0 );

		const Options& options = processor.getOptions();

#		ifdef _WIN32
		const wchar_t* pwzsEnvironment = _wgetenv( L"COMPUTERNAME" );
#		else
#		error Adjust environment variable.
#		endif

		wstring sHost( pwzsEnvironment );
		wstring sDelimiter;

		if ( sHost.empty() ) {
			sHost = L"unkown";
		}

		switch ( options.getStringDelimiter() ) {
			case Options::STRD_DOUBLE:
				sDelimiter = L"\"";
				break;
			case Options::STRD_SINGLE:
				sDelimiter = L"\'";
				break;
			default:
				throw UnexpectedSwitchError();
		}
		sHost = sDelimiter + sHost + sDelimiter;
		TokenExpression token( TOK_STRING, processor.getContext(), sHost );

		result.push_back( token );
	}
};

BuildinHost::BuildinHostExpander BuildinHost::m_expander;

BuildinHost::BuildinHost()
: base( L"__HOST__", BuildinHost::m_expander )
{
}


// --------------------------------------------------------------------
// BuildinCounter
// --------------------------------------------------------------------

class BuildinCounter::BuildinCounterExpander : public sqtpp::MacroExpander
{
	int m_nCounter;

public:
	BuildinCounterExpander() {}


	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__COUNTER__" );
		assert( argumentValues.size() == 0 );

		const File& file = processor.getFile();
		size_t      cntr = file.getNextCounter();
		wstring sCounter = lexical_cast<wstring>((unsigned int)cntr);


		TokenExpression token( TOK_NUMBER, processor.getContext(), sCounter );

		result.push_back( token );

		++m_nCounter;
	}

};

BuildinCounter::BuildinCounterExpander BuildinCounter::m_expander;

BuildinCounter::BuildinCounter()
: base( L"__COUNTER__", BuildinCounter::m_expander )
{
}


// --------------------------------------------------------------------
// BuildinEval
// --------------------------------------------------------------------

class BuildinEval::BuildinEvalExpander : public sqtpp::MacroExpander
{
private:
	typedef sqtpp::MacroExpander base;
public:
	BuildinEvalExpander() {}

	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__EVAL" );
		assert( argumentValues.size() == 1 );

		base::expand( macro, processor, argumentValues, result );

		Expression evaluator;
		evaluator.build( result );
		const Expression::Value& value = evaluator.evaluate( &processor.getMacros() );
		wstring tokenText = lexical_cast<wstring>(value.getInteger());
		TokenExpression tokenExpression( TOK_NUMBER, CTX_DEFAULT, tokenText );
		result.clear();
		result.push_back( tokenExpression );
	}

};

BuildinEval::BuildinEvalExpander BuildinEval::m_expander;

BuildinEval::BuildinEval()
: base( L"__EVAL", BuildinEval::m_expander )
{
	MacroArguments   arguments;
	TokenExpressions expressions;
	expressions.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"__EXPR__" ) );
	arguments.push_back( MacroArgument( L"__EXPR__" ) );
	this->setArguments( arguments );
	this->setExpression( expressions, L"" );
}


// --------------------------------------------------------------------
// BuildinQuote
// --------------------------------------------------------------------

class BuildinQuote::BuildinQuoteExpander : public sqtpp::MacroExpander
{
private:
	typedef sqtpp::MacroExpander base;
public:
	BuildinQuoteExpander() {}

	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& processor, const MacroArgumentValues& argumentValues, TokenExpressions& result )
	{
		assert( macro.getIdentifier() == L"__QUOTE" );
		assert( argumentValues.size() == 1 );

		base::expand( macro, processor, argumentValues, result );

		const Options& options     = processor.getOptions();
		const wchar_t  delimiter   = (wchar_t)options.getStringDelimiter();
		const wchar_t  escape      = options.getStringQuoting() == Options::QUOT_DOUBLE ? delimiter : L'\\';
		wstring resultString       = result.stringize( delimiter, escape );

		result.clear();
		result.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, resultString ) );

	}

};

BuildinQuote::BuildinQuoteExpander BuildinQuote::m_expander;

BuildinQuote::BuildinQuote()
: base( L"__QUOTE", BuildinQuote::m_expander )
{
	MacroArguments   arguments;
	TokenExpressions expressions;
	expressions.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"__EXPR__" ) );
	arguments.push_back( MacroArgument( L"__EXPR__" ) );
	this->setArguments( arguments );
	this->setExpression( expressions, L"" );
}



// --------------------------------------------------------------------
// BuildinUndef
// --------------------------------------------------------------------

class BuildinUndefAll::BuildinUndefAllExpander : public sqtpp::MacroExpander
{
private:
	typedef sqtpp::MacroExpander base;
public:
	BuildinUndefAllExpander() {}

	/// Expand a macro expression.
	virtual void expand( const Macro& macro, const Processor& /*processor*/, const MacroArgumentValues& argumentValues, TokenExpressions& /* result */ )
	{
		assert( macro.getIdentifier() == L"__UNDEF_ALL" );
		assert( argumentValues.size() == 1 );

		// TODO
		assert( false );

	}

};

BuildinUndefAll::BuildinUndefAllExpander BuildinUndefAll::m_expander;

BuildinUndefAll::BuildinUndefAll()
: base( L"__UNDEF_ALL", BuildinUndefAll::m_expander )
{
	MacroArguments   arguments;
	TokenExpressions expressions;
	expressions.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"__EXPR__" ) );
	arguments.push_back( MacroArgument( L"__EXPR__" ) );
	this->setArguments( arguments );
	this->setExpression( expressions, L"" );
}



} // namespace

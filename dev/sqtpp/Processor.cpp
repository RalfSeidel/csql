#include "stdafx.h"
#include <errno.h>
#include <ctime>
#include <cassert>
#include <fstream>
#include "Logger.h"
#include "Location.h"
#include "Options.h"
#include "Directive.h"
#include "File.h"
#include "Output.h"
#include "Exceptions.h"
#include "Expression.h"
#include "Error.h"
#include "Macro.h"
#include "Buildin.h"
#include "Token.h"
#include "Context.h"
#include "Util.h"
#include "Scanner.h"
#include "CodePage.h"
#include "CodePageConverter.h"
#include "Processor.h"


namespace sqtpp {

/**
** @brief The tokens of an expanded macro.
*/
class Processor::MacroExpansion : public ITokenStream
{
private:
	const TokenExpressions* m_pExpressions;
	size_t                  m_nTokenIndex;
	bool                    m_bExpandMacros;
public:
	MacroExpansion( const TokenExpressions& tokenExpressions )
	: m_pExpressions( &tokenExpressions )
	, m_nTokenIndex( 0 )
	, m_bExpandMacros( true )
	{
	}

	/// No recursive macro expansion.
	bool  expandMacros() const throw()
	{
		return false;
	}

	/// Just iterate through the tokens an return the next one.
	Token getNextToken( wistream&, TokenExpression& tokenExpression )
	{
		if ( m_nTokenIndex < m_pExpressions->size() ) {
			tokenExpression = (*m_pExpressions)[m_nTokenIndex];
			m_nTokenIndex++;
			return tokenExpression.token;
		} else {
			return TOK_END_OF_FILE;
		}
	}
};

/**
** @brief the processor constructor.
**
** @param options The preprocessor options.
*/
Processor::Processor( Options& options )
: m_options( options )
, m_bOptionsApplied( false )
, m_logger( *new Logger() )
, m_pScanner( NULL )
, m_fileStack( *new FileStack() )
, m_includeOnceFiles( *new StringSet() )
, m_macros( *new MacroSet() )
, m_tokenExpression( *new TokenExpression() )
, m_tokenStreamStack( *new TokenStreamStack() )
, m_conditionalStack( *new LocationStack() )
, m_nProcessedLines( 0 )
, m_nProcessedTokenId( 0 )
, m_nMaxMsgSeverity( error::Error::SEV_UNDEFINED )
, m_nErrorCount( 0 )
, m_nWarningCount( 0 )
, m_nOutputLineNumber( 1 )
, m_nSkippedLineCount( 0 )
, m_pTokenStream( NULL )
, m_pOutput( NULL )
, m_bExternalOutput( false )
, m_pTestTimestamp( NULL )
//, m_pIStream( NULL )
{
	//m_pOutStream->
}

/**
** @brief d'tor.
*/
Processor::~Processor()
{
	if ( !m_bExternalOutput )
		delete m_pOutput;
	delete m_pScanner;
	delete &m_conditionalStack;
	delete &m_tokenStreamStack;
	delete &m_tokenExpression;
	delete &m_macros;
	delete &m_includeOnceFiles;
	delete &m_fileStack;
	delete &m_logger;
}

/**
** @brief Close the output file if it has been created by this instance.
*/
void Processor::close()
{
	if ( m_pOutput != NULL ) {
		m_pOutput->close();
	}
}

/**
** @brief Set the output used for emitting the code and error messages.
*/
// 
void Processor::setOutput( Output* pOutput )
{
	if ( !m_bExternalOutput ) 
		delete m_pOutput;
	m_pOutput = pOutput;
	m_bExternalOutput = true;
}


/**
** @brief Set the stream to which the processor will write the result.
**
** @attention The processor stores a pointer to the submitted reference.
** Do not pass a local variable.
*/
void Processor::setOutStream( std::wostream& output )
{
	Output* pOutput = Output::createOutput( output );
	if ( !m_bExternalOutput ) 
		delete m_pOutput;
	m_pOutput = pOutput;
	m_bExternalOutput = false;
}


/**
** @brief Get the current scan/processing context.
*/
Context Processor::getContext() const throw()
{
	return m_tokenExpression.getContext();
}

/**
** @brief Get the current file we are processing.
*/
void Processor::getTimestamp( tm& timestamp ) const
{
	if ( m_pTestTimestamp != NULL ) {
		timestamp = *m_pTestTimestamp;
	} else {
		Util::getLocalTime( timestamp );
	}
}

/**
** @brief Get the current file we are processing.
*/
File& Processor::getFile()
{
	File& file = m_fileStack.top();

	return file;
}

const File& Processor::getFile() const
{
	File& file = m_fileStack.top();

	return file;
}





/**
** @brief Check if the given identifier is the name of a predefined / Buildin macro.
*/
bool Processor::isBuildinMacro( const wstring& identifier ) const
{
	MacroSet::const_iterator it = this->m_macros.find( identifier );
	if ( it != m_macros.end() ) {
		const Macro& macro = it->second;
		return macro.isBuildin();
	} else {
		return false;
	}
}



/**
** @brief Emit line and file information to the output stream.
**
** The methode will produce output in a form like 
** @code
** #line n "filename"
** @endcode
*/
void Processor::emitLine( std::wostream& output )
{
	// #line n "filename"
	const File&    file     = getFile();
	const wstring  fileName = file.getPath();
	const wstring  sNewLine = file.getDefaultNewLine(); 
	const size_t   line     = file.getLine();
	const wchar_t  quote    = m_options.getStringDelimiter() == Options::STRD_DOUBLE ? L'"' : '\'';

	// The stream operator << inserts thousand grouping separators. I haven't found a way
	// to avoid this (expected classic locale wouldn't do so). The workaround is to do
	// the formatting manually.
	wchar_t lineBuffer[20];
	_itow( line, lineBuffer, 10 );

	output << L"#line " << lineBuffer << L' ' << quote;

	if ( m_options.getStringQuoting() == Options::QUOT_ESCAPE ) {
		// Replace backslash in file name with double backshlash.
		wstring::const_iterator it = fileName.begin();
		while ( it != fileName.end() ) {
			const wchar_t ch = *it;
			if ( ch == L'\\' ) {
				output << ch;
			}
			output << ch;
			++it;
		}
	} else {
		output << fileName;
	}
	output << quote;

	emitLineFeed( output, sNewLine );
}

/**
** @brief Emit a line feed.
**
** @param sNewLine The line feed found in the input.
*/
void Processor::emitLineFeed( std::wostream& output, const wstring& sNewLine )
{
	switch ( m_options.getNewLineOutput() ) {
		case Options::NLO_AS_IS:
			if ( sNewLine.empty() ) {
				output << endl;
			} else {
				output << sNewLine;
			}
			break;
		case Options::NLO_OS_DEFAULT:
			output << endl;
			break;
		case Options::NLO_LF:
			output << L'\n';
			break;
		case Options::NLO_CR:
			output << L'\r';
			break;
		case Options::NLO_CRLF:
			output << L"\r\n";
			break;
		default:
			throw UnexpectedSwitchError();
			//throw UnexpectedSwitchError( "NewLineOutput" );
	}
}

/**
** @brief Emit a message, warning or error.
*/
void Processor::emitMessage( error::Error& error ) const
{
	if ( m_fileStack.size() != 0 ) {
		const File& file = getFile();
		error.setFileInfo( file );
	}

	m_pOutput->getErrStream() << error;

	if ( error.getSeverity() > m_nMaxMsgSeverity )
		m_nMaxMsgSeverity = error.getSeverity();
	if ( error.getSeverity() > error::Error::SEV_ERROR ) 
		++m_nErrorCount;
	if ( error::Error::SEV_INFO < error.getSeverity() && error.getSeverity() < error::Error::SEV_ERROR ) 
		++m_nWarningCount;
}

/**
** @brief Emit the current output buffer.
**
** @param output The output stream to emit to.
** @returns Number of characters written.
*/
size_t Processor::emitBuffer( wostream& output )
{
	wstring line   = m_outputBuffer.str();
	bool    bEmit  = true;
	size_t  nCount = 0;

	if ( m_options.trimTrailingBlanks() ) {
		//line = line.trimLeft();
	}
	if ( m_options.trimLeadingBlanks() ) {
		//line = line.trimRight();
	}

	// Optionaly skip empty lines (if not in the middle of a string).
	if ( getContext() != CTX_DQUOTE_STRING && getContext() != CTX_SQUOTE_STRING ) {
		if ( m_options.eliminateEmptyLines() && line == L""  ) {
			bEmit = false;
		}
	}

	if ( bEmit ) {
		if ( line.length() > 0 ) {
			output << line;
			nCount = line.length();
		}
		m_prevLine = line;
	}

	m_outputBuffer.str( wstring() );
	m_outputBuffer.clear();

	return nCount;
}


/**
** @brief Process the input.
*/
void Processor::processStream( std::wistream& input )
{
	File file;
	try {
		file.attach( input );
		m_fileStack.push( file );
		m_nOutputLineNumber = 1;

		processInput();
		emitBuffer( m_pOutput->getStream() );
		m_fileStack.pop();
	} catch ( error::Error& error ) {
		m_fileStack.pop();

		if ( error.getFilePath().empty() ) {
			error.setFileInfo( file );
			m_pOutput->getErrStream() << error;
		}
		if ( !m_fileStack.empty() ) {
			throw;
		}
	} catch ( const std::exception& ex ) {
		wstring message = Convert::str2wcs( ex.what() );
		error::C1001 error( message );
		error.setFileInfo( file );
		m_pOutput->getErrStream() << error;
		if ( !m_fileStack.empty() ) {
			throw error;
		}
	}
}


/**
** @brief open and process file.
*/
void Processor::processFile( const std::wstring& fileName )
{
	File      file;
	m_fileStack.push( file );

	try {
		wistream& input = file.open( fileName );

		if ( !input.good() ) {
			// Unable to open file 
			throw error::C1068( fileName );
		}

		// Reset the output line number counter to force emitting
		// the #line directive for the next non empty line.
		m_nOutputLineNumber = 0;
		processInput();

		// If an include file doesn't end with an empty line 
		// emit the remaining part.
		if ( m_outputBuffer.tellp() > 0 ) {
			const wstring* psNewLine = m_fileStack.size() > 1 ? &file.getDefaultNewLine() : NULL;
			processNewLine( psNewLine );
		}

		if ( file.isAutoIncludedOnce() ) {
			file.setIncludeOnce( true );
		}
		if ( file.isIncludeOnce() ) {
			m_includeOnceFiles.insert( file.getPath() );
		}
		m_fileStack.pop();
		m_nOutputLineNumber = 0;
	} catch ( error::Error& error ) {
		m_fileStack.pop();

		if ( error.getFilePath().empty() ) {
			error.setFileInfo( file );
			m_pOutput->getErrStream() << error;
		}
		if ( !m_fileStack.empty() ) {
			throw;
		}
	} catch ( const std::exception& ex ) {
		wstring message = Convert::str2wcs( ex.what() );
		error::C1001 error( message );
		error.setFileInfo( file );
		if ( m_fileStack.empty() ) {
			m_pOutput->getErrStream() << error;
		} else {
			throw error;
		}
	}
}


/**
** @brief Get the next token from the lexer and put the scanned text into tokenExpression.
*/
Token Processor::getNextToken( TokenExpression& tokenExpression )
{
	File&     currentFile   = getFile();
	wistream& fileStream    = currentFile.getStream();
	Token     token         = TOK_UNDEFINED;
	bool      bSetFileToken = getContext() == CTX_DEFAULT;

	assert( m_pTokenStream != NULL );

	tokenExpression.setTokenId( ++m_nProcessedTokenId );
	token = m_pTokenStream->getNextToken( fileStream, tokenExpression );

	if ( bSetFileToken && !m_fileStack.empty() ) {
		File& currentFile = getFile();
		currentFile.setLastToken( token );
	}
	return token;
}

/**
** @brief Get the next token and the token expression from the lexer.
**
** After this call m_tokenExpression will hold the last text fetched by the scanner.
*/
Token Processor::getNextToken()
{
	Token token = getNextToken( m_tokenExpression );
	return token;
}


/**
** @brief Scan until an identifier has been found.
**
** Allow space, block comments and new line characters only.
*/
const wstring Processor::getNextIdentifier()
{
	wstring identifier;
	Token   token     = TOK_UNDEFINED;
	bool    bContinue = true;

	while ( bContinue ) {
		token = getNextToken();
		switch ( token ) {
			case TOK_SPACE:
				/// ignore
				break;
			case TOK_IDENTIFIER:
				identifier = m_tokenExpression.identifier;
				bContinue  = false;
				break;
			case TOK_BLOCK_COMMENT:
				/// ignore
				break;
			case TOK_NEW_LINE:
				if ( m_tokenExpression.getContext() == CTX_BLOCK_COMMENT ) {
					// OK, allow new lines in block comments.
				} else {
					// #pragma line has ended.
					bContinue = false;
				}
				break;
			case TOK_END_OF_FILE:
				/// Unexpected end of file.
				throw error::C1004();
				break;
			default:
				throw UnexpectedSwitchError();
		}
	}
	return identifier;
}


/**
** @brief Process the current input stream.
*/
void Processor::applyOptions()
{
	//Trace( "Processor::applyOptions" << endl );

	// don't call this methode twice.
	assert( m_bOptionsApplied == false );
	m_bOptionsApplied = true;

	m_pScanner     = Scanner::createScanner( m_options );
	m_pTokenStream = m_pScanner;

	if ( m_pOutput == NULL ) {
		m_pOutput = Output::createOutput( m_options );
		m_bExternalOutput = false;
	}

	// Add buildin macros.
	if ( !m_options.undefAllBuildin() ) {
		BuildinMacro::addBuildinMacros( m_options, m_macros );
	}

	const StringArray& undefs = m_options.getUndefines();
	for ( StringArray::const_iterator itUndef = undefs.begin(); itUndef != undefs.end(); ++itUndef ) {
		const wstring& identifier  = *itUndef;
		MacroSet::iterator itMacro = m_macros.find( identifier );
		if ( itMacro != m_macros.end() ) {
			m_macros.erase( identifier );
		} else {
			// Warning?
		}
	}

	const StringDictionary& defines = m_options.getDefines();
	for ( StringDictionary::const_iterator itDefine = defines.begin(); itDefine != defines.end(); ++itDefine ) {
		const wstring& identifier = itDefine->first;
		const wstring& expression = itDefine->second;

		wstring       code = wstring(L"#define ") + identifier + L" " + expression;
		wstringstream input(code);
		// Trace( L"processing macro definition found at command line:" << code );
		bool bEmitLineBackup  = m_options.emitLine();
		bool bEmtyLinesBackup = m_options.eliminateEmptyLines();
		m_options.emitLine( false );
		m_options.eliminateEmptyLines( true );
		processStream( input );
		m_options.eliminateEmptyLines( bEmtyLinesBackup );
		m_options.emitLine( bEmitLineBackup );
	}
}


/**
** @brief Evaluate the expression of an \#if or \#elif directive.
*/
bool Processor::evaluateConditionalDirective()
{
	TokenExpressions expressions;
	bool             bContinue      = true;
	bool             bIgnoreNewLine = false;
	bool             bExpandMacro   = true;
	int              nParenthesis   = 0;

	while ( bContinue ) {
		Token token = getNextToken();

		switch ( token ) {
			case TOK_NEW_LINE:
				if ( m_tokenExpression.context == CTX_BLOCK_COMMENT ) {
					// continue until comment ends
				} else if ( !bIgnoreNewLine ) {
					bContinue = false;
				}
				processNewLine();
				break;
			case TOK_EOL_BACKSLASH:
				bIgnoreNewLine = true;
				break;
			case TOK_BLOCK_COMMENT:
				break;
			case TOK_LINE_COMMENT:
				break;
			case TOK_SPACE:
				// space is just noise.
				break;
			case TOK_OP_DEFINED:
				expressions.push_back( m_tokenExpression );
				bExpandMacro = false;
				break;
			case TOK_LEFT_PARENTHESIS:
				expressions.push_back( m_tokenExpression );
				++nParenthesis;
				break;
			case TOK_RIGHT_PARENTHESIS:
				if ( nParenthesis == 0 ) {
					// Unmatched parenthesis.
					throw error::C1012();
				} else {
					expressions.push_back( m_tokenExpression );
					--nParenthesis;
					bExpandMacro = true;
				}
				break;
			case TOK_IDENTIFIER:
				if ( bExpandMacro ) {
					const wstring& identifier = m_tokenExpression.getText();
					MacroSet::iterator itMacro  = m_macros.find( identifier );

					// no macro: return as is.
					if ( itMacro == m_macros.end() ) {
						expressions.push_back( m_tokenExpression );
					} else {
						Macro&           macro = itMacro->second;
						TokenExpressions macroExpressions;
						expandMacro( macro, macroExpressions );
						expressions.insert( expressions.end(), macroExpressions.begin(), macroExpressions.end() );
					}
				} else {
					expressions.push_back( m_tokenExpression );
					bExpandMacro = true;
				}
				break;
			case TOK_END_OF_FILE:
				bContinue = false;
				break;
			default:
				expressions.push_back( m_tokenExpression );
				bExpandMacro = true;
				break;
		} // switch

		// reset ignore end of line flag.
		if ( token != TOK_EOL_BACKSLASH ) {
			bIgnoreNewLine = false;
		}
	}
	if ( nParenthesis > 0 ) {
		throw error::C1012();
	}

	Expression evaluator;
	evaluator.build( expressions );
	const Expression::Value& value = evaluator.evaluate( &m_macros );
	bool  isTrue = value.getInteger() != 0;
	return isTrue;
}

/**
** @brief Process the current input stream.
*/
void Processor::processInput()
{
	Token  token = TOK_END_OF_FILE;
	size_t conditionalStackSize = m_conditionalStack.size();


	if ( !m_bOptionsApplied ) {
		applyOptions();
	}

	do {
		token = getNextToken();
		processToken( token );
	} while ( token != TOK_END_OF_FILE );


	// Check for unmatched conditionals
	if ( conditionalStackSize != m_conditionalStack.size() ) {
		const Location* pLocation = NULL;
		if ( m_conditionalStack.size() > conditionalStackSize ) {
			pLocation = &m_conditionalStack.container()[conditionalStackSize];
		}
		throw error::C1070( pLocation );
	}
}

/**
** @brief Process the scanner token.
*/
bool Processor::processToken( int scannerToken )
{
	const wstring& tokenText = m_tokenExpression.text;
	bool           bContinue = true;

	switch( scannerToken ) {
		case TOK_UNDEFINED:
			throw RuntimeError( "Unexpected token." );
		case TOK_LINE_COMMENT:
			if ( m_options.keepLineComments() ) {
				// Translate C++ // comments into SQL -- comments.
				if ( m_options.getLanguage() == Options::LNG_SQL && tokenText.substr( 0, 2 ) == L"//" ) {
					m_outputBuffer << L"--" << tokenText.substr(2);
				} else {
					m_outputBuffer << tokenText;
				}
			}
			break;
		case TOK_BLOCK_COMMENT:
			processBlockComment();
			break;
		case TOK_SQL_LINE_COMMENT:
			m_outputBuffer << tokenText;
			break;
		case TOK_NEW_LINE:
			processNewLine( &tokenText );
			break;
		case TOK_SPACE:
			m_outputBuffer << tokenText;
			break;
		case TOK_STRING:
			m_outputBuffer << tokenText;
			break;
		case TOK_NUMBER:
			m_outputBuffer << tokenText;
			break;
		case TOK_IDENTIFIER:
			processIdentifier();
			break;
		case TOK_DIRECTIVE:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processDirective();
			break;
		case TOK_DIR_DEFINE:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processDefineDirective();
			break;
		case TOK_DIR_UNDEF:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processUndefDirective();
			break;
		case TOK_DIR_UNDEFALL:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processUndefallDirective();
			break;
		case TOK_DIR_IF:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processIfDirective();
			break;
		case TOK_DIR_IFDEF:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processIfdefDirective();
			break;
		case TOK_DIR_IFNDEF:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processIfndefDirective();
			break;
		case TOK_DIR_ELIF:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processElifDirective();
			break;
		case TOK_DIR_ELSE:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processElseDirective();
			break;
		case TOK_DIR_ENDIF:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processEndifDirective();
			break;
		case TOK_DIR_INCLUDE:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processIncludeDirective();
			break;
		case TOK_DIR_LINE:
			processLineDirective();
			break;
		case TOK_DIR_ERROR:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processErrorDirective();
			break;
		case TOK_DIR_PRAGMA:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processPragmaDirective();
			break;
		case TOK_DIR_IMPORT:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processImportDirective();
			break;
		case TOK_DIR_USING:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processUsingDirective();
			break;
		case TOK_DIR_MESSAGE:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processMessageDirective();
			break;
		case TOK_DIR_EXEC:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processExecDirective();
			break;
		case TOK_ADSALESNG_DIRECTIVE:
			// Directives may only come from the input file (not from expanded macros).
			// Current token source has to be the input lexer..
			assert( m_pTokenStream == m_pScanner );
			processAdSalesNGDirective();
			break;
		case TOK_OTHER:
			m_outputBuffer << tokenText;
			break;
		case TOK_EOL_BACKSLASH:
			// to not emit. 
			break;
		case TOK_END_OF_FILE:
			bContinue = false;
			break;
		default:
			m_outputBuffer << tokenText;
			break;

	}
	return bContinue;
}


/**
** @brief Process the input stream to collect the macro arguments.
**
** @param macro The macro for which the arguments are to be collected.
** @param argumentValues The arguments found.
** @param tokenExpressions If no argument can been found the tokens
**        scanned while searching for the arguments are placed 
**        into this container.
** @returns true if an argument list has been found. false if not.
*/
bool Processor::collectMacroArgumentValues( const Macro& macro, MacroArgumentValues& argumentValues, TokenExpressions& tokenExpressions )
{
	assert( macro.hasArguments() );
	assert( argumentValues.size() == 0 );

	bool bArgumentsFound = true;
	bool bContinue       = true;

	while ( bContinue ) {
		Token           token = getNextToken();
		tokenExpressions.push_back( m_tokenExpression );
		switch ( token ) {
			case TOK_NEW_LINE:
				if ( this->m_pTokenStream == m_pScanner ) {
					File&  file = getFile();
					file.incLine();
				}
				break;
			case TOK_SPACE:
			case TOK_LINE_COMMENT:
			case TOK_BLOCK_COMMENT:
			case TOK_EOL_BACKSLASH:
				// ignore these
				break;
			case TOK_LEFT_PARENTHESIS:
				// found start of macro parameters.
				bContinue = false;
				break;
			default:
				// Anything else: no parameters/no macro expansion.
				bArgumentsFound = false;
				bContinue       = false;
				break;
		} // switch token
	}

	if ( bArgumentsFound ) {
		// Left parenthesis found --> Discard any other token.
		tokenExpressions.clear();

		bool  bContinue = true;
		int   nesting   = 0;
		while ( bContinue ) {
			TokenExpression          tokenExpression;
			MacroSet::const_iterator itNestedMacro;

			Token token = getNextToken( tokenExpression );

			switch ( token ) {
				case TOK_OP_COMMA:
					if ( nesting == 0 ) {
						tokenExpressions.trim( true, !m_options.keepBlockComments(), !m_options.keepLineComments(), !m_options.keepSqlComments() );
						argumentValues.push_back( tokenExpressions );
						tokenExpressions.clear();
					} else {
						tokenExpressions.push_back( tokenExpression );
					}
					break;
				case TOK_IDENTIFIER:
					if ( m_options.expandMacroArguments() ) {
						throw NotSupportedError();

						/*
						itNestedMacro = m_macros.find( tokenExpression.getText() );
					
						if (  itNestedMacro != m_macros.end() && !itNestedMacro->second.isExpanding() ) {
							const Macro& nestedMacro = itNestedMacro->second;
							TokenExpressions nestedExpressions;
							expandMacro( nestedMacro, nestedExpressions );
							// TODO: just inserting doesn't work reliable. Put expression on stack an used them in getNextToken()
							// until they are exhausted.
							tokenExpressions.insert( tokenExpressions.end(), nestedExpressions.begin(), nestedExpressions.end() );
						} else {
							tokenExpressions.push_back( tokenExpression );
						}
						*/
					} else {
						// Avoid recursive expansion by replacing TOK_IDENTIFIER with TOK_OTHER.
						tokenExpression.token = TOK_IDENTIFIER;
						tokenExpressions.push_back( tokenExpression );
					}
					break;
				case TOK_LEFT_PARENTHESIS:
					nesting++;
					tokenExpressions.push_back( tokenExpression );
					break;

				case TOK_RIGHT_PARENTHESIS:
					if ( nesting == 0 ) {
						// Keep comments if and only if the macro has at least one argument. 
						// If the macro does not have  an argument we can't insert them. 
						// So ignore them in this case.
						const size_t macroArgCount = macro.getArguments().size();
						const bool removeBlockComments  = !m_options.keepBlockComments() || macroArgCount == 0;
						const bool removeLineComments  = !m_options.keepLineComments() || macroArgCount == 0;
						const bool removeSqlComments  = !m_options.keepSqlComments() || macroArgCount == 0;
						tokenExpressions.trim( true, removeBlockComments, removeLineComments, removeSqlComments  );
						if ( tokenExpressions.size() > 0 || argumentValues.size() == macro.getArguments().size() - 1 ) {
							argumentValues.push_back( tokenExpressions );
						}
						tokenExpressions.clear();
						bContinue = false;
					} else {
						nesting--;
						tokenExpressions.push_back( tokenExpression );
					}
					break;
				case TOK_END_OF_FILE:
					// Unexpected end of file while collecting arguments for the macro {1}.
					throw error::C1057( macro.getIdentifier() );
					break;
				default:
					tokenExpressions.push_back( tokenExpression );
					break;
			} // switch token.
		}
	}
	return bArgumentsFound;
}



/**
** @brief Collect macro arguments form the given token stream.
**
** @todo Expand macros in macro arguments.
**
** @param macro            The macro to expand.
** @param tokenExpressions The resulting macro tokens.
** @returns true if the macro has been translated.
*/
bool Processor::collectMacroArgumentValues( const Macro& macro, ITokenStream& tokenStream, MacroArgumentValues& argumentValues ) const
{
	Processor* pThis = const_cast<Processor*>(this);
	ITokenStream* previousStream = pThis->m_pTokenStream;
	bool argumentsFound = false;
	try {
		TokenExpressions dummy;
		pThis->m_pTokenStream = &tokenStream;
		argumentsFound = pThis->collectMacroArgumentValues( macro, argumentValues, dummy );
		pThis->m_pTokenStream = previousStream;
	}
	catch ( ... ) {
		pThis->m_pTokenStream = previousStream;
		throw;
	}
	return argumentsFound;
}


/**
** @brief Expand the given macro.
**
** @todo Expand macros in macro arguments.
**
** @param macro            The macro to expand.
** @param tokenExpressions The resulting macro tokens.
** @returns true if the macro has been translated.
*/
bool Processor::expandMacro( const Macro& macro, TokenExpressions& tokenExpressions )
{
	MacroArgumentValues argumentValues;
	bool                bHasArguments = macro.hasArguments();
	bool                bExpand       = true;

	if ( bHasArguments ) {
		// collect macro arguments.
		bHasArguments = collectMacroArgumentValues( macro, argumentValues, tokenExpressions );
		if ( !bHasArguments ) {
			TokenExpression identifierExpression( TOK_OTHER, CTX_DEFAULT, macro.getIdentifier() );
			tokenExpressions.insert( tokenExpressions.begin(), identifierExpression );
			bExpand = false;
		}
	}

	// Check argument list.
	if ( bHasArguments ) {
		if ( argumentValues.size() < macro.getArguments().size() ) {
			// Not enough parameters for macro {1}.
			throw error::C4003( macro.getIdentifier() );
		} else if ( argumentValues.size() > macro.getArguments().size() && !macro.hasVarArgs() ) {
			// To many parameters for macro '{1}'.
			throw error::C4002( macro.getIdentifier() );
		}
	}

	if ( bExpand ) {
		macro.expand( *this, argumentValues, tokenExpressions );
	}
	return bExpand;
}

/**
** @brief Scanner has found an identifier - expand macro if existent.
*/
void Processor::processIdentifier()
{
	const wstring      identifier = m_tokenExpression.identifier;

	if ( !m_pTokenStream->expandMacros() ) {
		// Currently processing an expanded macro: no recursion here.
		m_outputBuffer << identifier;
		return;
	}

	MacroSet::iterator itMacro    = m_macros.find( identifier );

	if ( itMacro == m_macros.end() ) {
		// no macro: return as is.
		m_outputBuffer << identifier;
		return;
	}


	Macro&  macro = itMacro->second;

	// No recursive macro expansion
	if ( macro.isExpanding() ) {
		m_outputBuffer << identifier;
		return;
	}

	TokenExpressions tokenExpressions;

	bool isExpanded   = expandMacro( macro, tokenExpressions );

	TokenExpression nextExpressionToProcess;
	if ( !isExpanded && tokenExpressions.size() > 0 ) {
		nextExpressionToProcess = *tokenExpressions.rbegin();
		tokenExpressions.pop_back();
	}

	ITokenStream* pPrevMacroExpansion = m_pTokenStream;
	try {
		macro.setExpanding( isExpanded  );
		MacroExpansion  macroExpansion( tokenExpressions );
		m_pTokenStream = &macroExpansion;

		// Don't emit line informations in the middle of a macro
		//bool emitLineSave = m_options.emitLine();
		//m_options.emitLine( false );
		processInput();
		macro.setExpanding( false );
		//m_options.emitLine( emitLineSave );
		m_pTokenStream = pPrevMacroExpansion;
	}
	catch( ... ) {
		macro.setExpanding( false );
		m_pTokenStream = pPrevMacroExpansion;
		throw;
	}

	if ( nextExpressionToProcess.token != TOK_UNDEFINED ) {
		m_tokenExpression = nextExpressionToProcess;
		processToken( nextExpressionToProcess.token );
	}
}



/**
** @brief Process the new line token.
**
** @param psNewLine The actual new line string found in the input file
** or NULL if the file is the last input fille
*/
void Processor::processNewLine( const wstring* psNewLine )
{
	File&  file = getFile();

	if ( psNewLine != NULL && !psNewLine->empty() && file.getDefaultNewLine().empty() ) {
		file.setDefaultNewLine( *psNewLine );
	}

	wostringstream lineOutput;
	const size_t nCharCount = emitBuffer( lineOutput );

	if ( nCharCount != 0 && this->m_pTokenStream == m_pScanner ) {
		if ( m_options.emitLine() && (m_nOutputLineNumber != file.getLine() || file.getLine() == 1 ) ) {
			emitLine( m_pOutput->getStream() );
			m_nOutputLineNumber = file.getLine();
		}
	}


	if ( psNewLine != NULL ) {
		if ( nCharCount != 0 || !m_options.eliminateEmptyLines() ) {
			emitLineFeed( lineOutput, *psNewLine );

			++m_nOutputLineNumber;
			m_nSkippedLineCount = 0;
		} else {
			++m_nSkippedLineCount;
		}
	}

	if ( lineOutput.tellp() > 0 ) {
		const wstring line = lineOutput.str();
		m_pOutput->getStream() << line ;
		m_pOutput->getStream().clear();
	}

	if ( this->m_pTokenStream == m_pScanner ) {
		file.incLine();
	}
	++m_nProcessedLines;
}


/**
** @brief Process the last new line token retrieved from the scanner..
*/
void Processor::processNewLine()
{
	processNewLine( &m_tokenExpression.text );
}


/**
** @brief Process a block comment.
**
** Expects sequence of block comments or new lines. Pass anything else
** back to the caller.
*/
void Processor::processBlockComment()
{
	Token token = TOK_BLOCK_COMMENT;

	assert( m_tokenExpression.token == TOK_BLOCK_COMMENT );

	for ( ;; ) {
		switch ( token ) {
			case TOK_BLOCK_COMMENT:
				if ( m_options.keepBlockComments() ) {
					const wstring& commentText = m_tokenExpression.text;
					m_outputBuffer << commentText;
				}
				break;
			case TOK_NEW_LINE:
				processNewLine();
				break;
			case TOK_END_OF_FILE:
				// End of file in comment.
				throw error::C1071();
				break;
			default:
				// Inside block comments the scanner should return 
				// comment and new line tokens only.
				throw UnexpectedSwitchError();
		}
		if ( m_tokenExpression.getContext() == CTX_BLOCK_COMMENT ) {
			token = getNextToken();
		} else {
			break;
		}
	} 
}

/**
** @brief Finish processing a directive which is complete.
**
** @param bEmit Emit the expressions found? 
**        Set this parameter to true for directives like \#error or \#message.
**        If false with methode will emit a warning (C4067) if anything else
**        than a comment or space has been found.
*/
void Processor::finishDirective( bool bEmit )
{
	bool bContinue      = true;
	bool bDidWarn       = false;
	bool bIgnoreNewLine = false;

	while ( bContinue ) {
		Token token = getNextToken();

		switch ( token ) {
			case TOK_NEW_LINE:
				if ( m_tokenExpression.context == CTX_BLOCK_COMMENT ) {
					// continue until comment ends
				} else if ( !bIgnoreNewLine ) {
					bContinue = false;
				}
				processNewLine();
				break;
			case TOK_SPACE:
			case TOK_BLOCK_COMMENT:
			case TOK_LINE_COMMENT:
				if ( bEmit ) {
					m_outputBuffer << m_tokenExpression.getText();
				}
				break;
			case TOK_EOL_BACKSLASH:
				bIgnoreNewLine = true;
				break;
			case TOK_END_OF_FILE:
				bContinue = false;
				break;
			default:
				if ( bEmit ) {
					m_outputBuffer << m_tokenExpression.getText();
				} else if ( !bDidWarn ) {
					// Unexpected text following preprocessor directive - expected a newline.
					error::C4067C warning;
					this->emitMessage( warning );
					bDidWarn = true;
				}
		} // switch

		// reset ignore end of line flag.
		if ( token != TOK_EOL_BACKSLASH ) {
			bIgnoreNewLine = false;
		}
	}
}


/**
** @brief Process a unkown directive.
**
** If the input langugage is T-SQL the directive maybe the name
** of a temporary. The token is retured as found in the input.
** If processing c / c++ an error is thrown (C1021).
*/
void Processor::processDirective()
{
	const wstring& identifier = m_tokenExpression.getIdentifier();
	if ( identifier.empty() ) {
		// ignore
	} else {
		if ( m_options.getLanguage() == Options::LNG_SQL ) {
			// Ignore unknown directive because it may be the name of an temporary 
			// table like in the following example:
			//	select * from 
			//			#temptable
			m_outputBuffer << m_tokenExpression.getText();
			return;
		} else {
			// Unknown preprocessor directive {1}.
			throw error::C1021( identifier );
		}
	}

	bool bIgnoreNewLine = false;
	bool bContinue      = true;
	while ( bContinue ) {
		Token token = getNextToken();

		switch ( token ) {
			case TOK_NEW_LINE:
				if ( !bIgnoreNewLine ) {
					bContinue = false;
				} else {
					processNewLine();
					bIgnoreNewLine = false;
				}
				break;
			case TOK_OTHER:
				bIgnoreNewLine = m_tokenExpression.text == L"\\";
				break;
			case TOK_BLOCK_COMMENT:
				processBlockComment();
				break;
			case TOK_SPACE:
				break;
			case TOK_END_OF_FILE:
				bContinue = false;
				return;
			default:
				bIgnoreNewLine = false;
				break;
		}
	}
}

/**
** @brief Check if the macro tokens are valid.
*/
void Processor::validateMacroDefinition( const TokenExpressions& tokenExpressions, const MacroArguments& arguments )
{
	if ( tokenExpressions.empty() ) 
		return;

	const TokenExpression& firstExpr = *tokenExpressions.begin();
	const TokenExpression& lastExpr  = *tokenExpressions.rbegin();
	const Token firstToken = firstExpr.getToken();
	const Token lastToken  = lastExpr.getToken();

	if ( firstToken == TOK_SHARP_SHARP ) {
		// Missing first argument for the \#\# concate operator.
		error::C2160 err;
		//emitMessage( err );
		throw err;
	}

	if ( lastToken == TOK_SHARP_SHARP ) {
		// Missing second argument for the \#\# concate operator.
		error::C2161 err;
		//emitMessage( err );
		throw err;
	}

	if ( lastToken == TOK_SHARP ) {
		// The token following a stringizing operator (#) has to be a macro argument.
		error::C2162A err;
		//emitMessage( err );
		throw err;
	}

	if ( lastToken == TOK_SHARP_AT ) {
		// The token following a charizing operator (#@) has to be a macro argument.
		error::C2162B err;
		//emitMessage( err );
		throw err;
	}

	for ( TokenExpressions::const_iterator it = tokenExpressions.begin(); it != tokenExpressions.end(); ++it ) {
		const TokenExpression& tokenExpr = *it;
		const Token token = tokenExpr.getToken();
		if ( token == TOK_SHARP || token == TOK_SHARP_AT ) {
			bool bNextIsArg = false;
			TokenExpressions::const_iterator itNext = it+1;
			if ( itNext != tokenExpressions.end() ) {
				const TokenExpression& nextExpr   = *itNext;
				const Token            nextToken  = nextExpr.getToken();
				if ( nextToken == TOK_IDENTIFIER ) {
					const wstring& identifier = nextExpr.getText();
					bNextIsArg = arguments.find( identifier ) != arguments.end();
				}
			}
			if ( !bNextIsArg ) {
				if ( token == TOK_SHARP  ) {
					// The token following a stringizing operator (#) has to be a macro argument. Found
					error::C2162A err;
					//emitMessage( err );
					throw err;
				} else {
					// The token following a charizing operator (#@) has to be a macro argument.
					error::C2162B err;
					//emitMessage( err );
					throw err;
				}
			}
		}
	}
}


/**
** @brief Process the \c \#define directive.
*/
void Processor::processDefineDirective()
{
	wstring identifier = getNextIdentifier();

	if ( identifier.empty() ) {
		// Missing identifier for #define directive.
		error::C2007 warning;
		emitMessage( warning );
	} 

	const File& file = this->getFile();
	Macro macro( identifier, file.getPath(), file.getLine() );


	// Collect parameters
	MacroArguments   arguments;
	TokenExpressions tokens;
	wstringstream    macroTextBuffer;
	Token            prevToken      = TOK_UNDEFINED;
	bool             bIgnoreNewLine = false;
	bool             bHasArgs       = false;
	bool             bContinue      = true;

	Token token = getNextToken();
	prevToken   = token;
	



	// If a '(' is following immediatly after the identifier collect the argument list. The
	// argument list is mandantory and has  to be well formed ( [IDENTIFIER] [,[IDENTIFIER]]+ ).
	// Otherwise the macro does not have any arguments the the token just scanned is the 
	// first token of the macro expression.

	if ( token == TOK_LEFT_PARENTHESIS ) {
		macroTextBuffer << m_tokenExpression.getText();
		bHasArgs = true;
		while ( bContinue ) {
			Token token = getNextToken();

			switch ( token ) {
				case TOK_RIGHT_PARENTHESIS:
					switch ( prevToken ) {
						case TOK_HELLIP:
						case TOK_LEFT_PARENTHESIS:
						case TOK_IDENTIFIER:
							// All parameters collected.
							bContinue = false;
							macroTextBuffer << m_tokenExpression.getText();
							break;
						default:
							// Unexpected expression in macro parameter list: '{1}'.
							throw error::C2010( m_tokenExpression.getText() );
					}
					break;

				case TOK_IDENTIFIER:
					if ( prevToken == TOK_LEFT_PARENTHESIS || prevToken == TOK_OP_COMMA ) {
						// Check if argument identifier is unique.
						MacroArguments::const_iterator itArg;
						const wstring&                 identifier = m_tokenExpression.getIdentifier();
						for ( itArg = arguments.begin(); itArg != arguments.end(); ++itArg ) {
							const MacroArgument& arg = *itArg;
							if ( arg.getIdentifier() == identifier ) {
								// Macro argument specified more than once: {1}.
								throw error::C2009( arg.getIdentifier() );
							}
						}
						arguments.push_back( identifier );
						macroTextBuffer << m_tokenExpression.getText();
					} else {
						// Unexpected expression in macro parameter list: '{1}'.
						throw error::C2010( m_tokenExpression.getText() );
					}
					break;

				case TOK_OP_COMMA:
					if ( prevToken == TOK_IDENTIFIER ) {
						prevToken = token;
						macroTextBuffer << m_tokenExpression.getText();
					} else {
						// Unexpected expression in macro parameter list: '{1}'.
						throw error::C2010( m_tokenExpression.getText() );
					}
					break;

				case TOK_LINE_COMMENT:
					if ( !bHasArgs ) {
						bContinue = false;
					} else {
						// Error
					}
					break;

				case TOK_BLOCK_COMMENT:
					// Ignore
					token = prevToken;
					break;

				case TOK_SPACE:
					// Ignore
					token = prevToken;
					break;

				case TOK_EOL_BACKSLASH:
					if ( m_tokenExpression.getContext() == CTX_LINE_COMMENT ) {
						error::C4010 warning;
						emitMessage( warning );
					} else {
						bIgnoreNewLine = true;
					}
					break;

				case TOK_NEW_LINE:
					if ( bIgnoreNewLine ) {
						if ( !m_options.multiLineMacroExpansion() ) {
							m_tokenExpression = TokenExpression( TOK_SPACE, m_tokenExpression.getContext(), L" " );
						}
					} else if ( m_tokenExpression.getContext() == CTX_BLOCK_COMMENT ) {
						// ignore 
						continue;
					} else  {
						// Unexpected expression in macro parameter list: '{1}'.
						throw error::C2010( L"new line" );
					}
					processNewLine();
					break;

				case TOK_END_OF_FILE:
					// Unexpected end of file
					throw error::C1004();

				default:
					// Unexpected expression in macro parameter list: '{1}'.
					throw error::C2010( m_tokenExpression.getText() );
			}

			prevToken = token;

			// reset ignore end of line flag.
			if ( token != TOK_EOL_BACKSLASH ) {
				bIgnoreNewLine = false;
			}
		}
		macro.setArguments( arguments );
	} 

	bContinue = prevToken != TOK_NEW_LINE && prevToken != TOK_END_OF_FILE;

	if ( bContinue && !bHasArgs ) {
		// If the macro does not have a argument list the just collected token 
		// is the begin of the macro expressions.
		tokens.push_back( m_tokenExpression );
		macroTextBuffer << m_tokenExpression.getText();
	}
	while ( bContinue ) {
		Token token = getNextToken();


		switch ( token ) {
			case TOK_LINE_COMMENT:
				// Ignore.
				break;

			case TOK_BLOCK_COMMENT:
				// Ignore
				// assert( tokenExpression.getContext() == CTX_BLOCK_COMMENT );
				token = prevToken;
				break;

			case TOK_EOL_BACKSLASH:
				if ( m_tokenExpression.getContext() == CTX_LINE_COMMENT ) {
					// Continue line character (\\) found inside a line comment.
					error::C4010 warning;
					emitMessage( warning );
				}
				bIgnoreNewLine = true;
				break;

			case TOK_NEW_LINE:
				if ( bIgnoreNewLine ) {
					// ignore. MacroExpander will take care for replacement of newlines by spaces.
					//if ( !m_options.multiLineMacroExpansion() ) {
					//	tokenExpression = TokenExpression( TOK_SPACE, m_tokenExpression.context, L" " );
					//}
				} else if ( m_tokenExpression.getContext() == CTX_BLOCK_COMMENT ) {
					// ignore 
				} else {
					bContinue = false;
				}
				processNewLine( &m_tokenExpression.getText() );
				break;

			case TOK_END_OF_FILE:
				bContinue = false;
				break;
			default:
				// Error
				break;
		}

		bool isMacroExpression = token != TOK_END_OF_FILE 
		                      && token != TOK_EOL_BACKSLASH
		                      && token != TOK_LINE_COMMENT
		                      && token != TOK_BLOCK_COMMENT
		                      && token != TOK_END_OF_FILE
		                      && (token != TOK_NEW_LINE || bIgnoreNewLine);
		if ( isMacroExpression ) {
			tokens.push_back( m_tokenExpression );
			macroTextBuffer << m_tokenExpression.getText();
		}
		// reset ignore end of line flag.
		if ( token != TOK_EOL_BACKSLASH ) {
			bIgnoreNewLine = false;
		}
	}


	const wstring macroDefText = Util::trim( macroTextBuffer.str() );
	tokens.trim( false, !m_options.keepBlockComments(), !m_options.keepLineComments(), !m_options.keepSqlComments() );
	validateMacroDefinition( tokens, arguments );
	macro.setExpression( tokens, macroDefText );
	

	// check if already defined.
	MacroSet::const_iterator it = m_macros.find( macro.getIdentifier() );
	if ( it != m_macros.end() ) {
		const pair<wstring, Macro>& entry = *it;
		const Macro&   macro2   = entry.second;
		const wstring  def2Text = macro2.getDefineText();
		if ( def2Text != macroDefText ) {
			wstringstream buffer;
			buffer << macro2.getDefineFile() << L" ("  << (unsigned int)macro2.getDefineLine() << L')';
			buffer << L"\nOld macro definition:" << def2Text;
			buffer << L"\nNew macro definition:" << macroDefText;
			const wstring prevDefinition = buffer.str();
			// Redefinition of macro {1}. The macro has already been defined in {2}.
			error::C4005 warning( identifier, prevDefinition );
			this->emitMessage( warning );
		}
	}

	m_macros[macro.getIdentifier()] = macro;
}

/**
** @brief Process the \c \#undef directive.
*/
void Processor::processUndefDirective()
{
	wstring identifier = getNextIdentifier();

	if ( identifier.empty() ) {
		// Missing identifier for #undef directive.
		error::C4006 warning;
		emitMessage( warning );
	} else {
		if ( isBuildinMacro( identifier ) ) {
			// Can't #undef buildin macro {1}.
			error::C4117 warning( identifier );
			emitMessage( warning );
		} else {
			m_macros.erase( identifier );
		}
	}
}

/**
** @brief Process the \c \#undefall directive.
*/
void Processor::processUndefallDirective()
{
	wstring prefix = getNextIdentifier();

	if ( prefix.empty() ) {
		// Missing identifier for #undef directive.
		error::C4006 warning;
		emitMessage( warning );
	} else {
		const size_t length = prefix.length();
		std::set<wstring> undefs;

		// Collect all macros that begin with the specified prefix.
		for( MacroSet::const_iterator it = m_macros.begin(); it != m_macros.end(); ++it ) {
			const std::wstring& name = it->first;
			if ( name.length() < length )
				continue;

			if ( name.substr( 0, length ) != prefix )
				continue;

			if ( isBuildinMacro( name ) ) {
				// Can't #undef buildin macro {1}.
				error::C4117 warning( name );
				emitMessage( warning );
			} else {
				undefs.insert( name );
			}
		}

		for ( std::set<wstring>::const_iterator it = undefs.begin(); it != undefs.end(); ++it ) {
			const std::wstring& name = *it;
			m_macros.erase( name );
		}
	}
}

/**
** @brief Process the \c include directive.
*/
void Processor::processIncludeDirective()
{
	wstring sFilePath;
	bool    bSearchCwd = true;

	assert( m_pTokenStream == m_pScanner );
	m_pScanner->pushContext( CTX_INCLUDE_DIRECTIVE );

	try {
		bool    bContinue  = true;
		while ( bContinue ) {
			Token token = getNextToken();

			switch ( token ) {
				case TOK_SPACE:
					// ignore
					break;
				case TOK_BLOCK_COMMENT:
					// ignore
					break;
				case TOK_NEW_LINE:
					if ( m_tokenExpression.getContext() == CTX_BLOCK_COMMENT ) {
						// ignore 
					} else {
						// Missing file path for #include directive. Found {1}.
						throw error::C2006( L"new line" );
					}
					break;
				case TOK_STRING:
					sFilePath  = m_tokenExpression.getText();
					bSearchCwd = true;
					bContinue  = false;
					break;

				case TOK_SYS_INCLUDE:
					sFilePath  = m_tokenExpression.getText();
					bSearchCwd = false;
					bContinue  = false;
					break;
				case TOK_END_OF_FILE:
					// Missing file path for #include directive. Found {1}.
					throw error::C2006( L"end of file" );

				default:
					// Missing file path for #include directive. Found {1}.
					throw error::C2006( m_tokenExpression.getText() );
			}
		}

		if ( !sFilePath.empty() && sFilePath.length() >= 2 ) {
			sFilePath  = sFilePath.substr( 1, sFilePath.length() - 2 );
		}

		m_pScanner->popContext( CTX_INCLUDE_DIRECTIVE );
	}
	catch( ... ) {
		m_pScanner->popContext( CTX_INCLUDE_DIRECTIVE );
		throw;
	}

	bool    bDoInclude  = true;
	File&   currentFile = getFile();
	wstring sFullPath   = currentFile.findFile( sFilePath, m_options.getIncludeDirectories(), bSearchCwd );

	if ( sFullPath.empty() ) {
		// Cannot open file: '{1}': No such file or directory.
		error::C1083 err( sFilePath );
		emitMessage( err );
		bDoInclude  = false;
	}

	if ( bDoInclude ) {
		bDoInclude  = m_includeOnceFiles.count( sFullPath ) == 0;
	}

	int   includedCt  = 0;
	const FileStack::container_type& c = m_fileStack.container();
	for ( FileStack::container_type::const_iterator it = c.begin(); it != c.end(); ++it ) {
		const File& file = *it;
		if ( file.getPath() == sFullPath ) {
			if ( file.isIncludeOnce() ) {
				bDoInclude = false;
				break;
			} else {
				m_pOutput->getLogStream() << L"Warning: " << sFilePath << L" has already been included." << endl;
				includedCt++;
			}
		}
	}

	if ( bDoInclude ) {
		File::checkFile( sFullPath );
	}

	// Finish directive now (not earlier) so that the current line number 
	// will be the next line of the current file.
	// Do not finish the directive earlier because otherwise any occuring
	// error when opening the fill will be returned with the wrong line number.
	finishDirective( false );

	if ( bDoInclude ) {
		if (m_options.verbose())
			m_pOutput->getLogStream() << L"including " << sFilePath << endl;

		if ( includedCt > 1 ) {
			throw error::C1014( sFilePath );
		}

		if ( this->m_includeOnceFiles.count( sFilePath ) == 0 ) {
			processFile( sFullPath );
		}
	}

}

/**
** @brief Process the \c if directive.
**
** @example input/If1Test.h
** @example input/If2Test.h
*/
void Processor::processIfDirective()
{
	Location location( getFile() );
	bool isTrue = evaluateConditionalDirective();

	if ( !isTrue ) {
		assert( m_pTokenStream == m_pScanner );
		m_pScanner->pushContext( CTX_CONDITIONAL_FALSE );
	}

	m_conditionalStack.push( location );
}

/**
** @brief Process the \c \#ifdef directive.
**
** @example input/Ifdef1Test.h
*/
void Processor::processIfdefDirective()
{
	Location location( getFile() );
	wstring identifier = getNextIdentifier();

	if ( identifier.empty() ) {
		// Excpected macro identifier for preprocessor conditional {1}.
		throw error::C1016( DirectiveInfo::Ifdef.getIdentifier() );
		// emitMessage( err );
	}

	finishDirective( false );

	if ( m_tokenExpression.token == TOK_END_OF_FILE ) {
		// Unexpected end of file.
		throw error::C1004();
	}

	m_conditionalStack.push( location );

	MacroSet::const_iterator itMacro = m_macros.find( identifier );
	if ( itMacro == m_macros.end() ) {
		// Not found: set scanner into conditional false mode.
		assert( m_pTokenStream == m_pScanner );
		m_pScanner->pushContext( CTX_CONDITIONAL_FALSE );
	}
}

/**
** @brief Process the \c \#ifndef directive.
*/
void Processor::processIfndefDirective()
{
	Location location( getFile() );
	wstring  identifier = getNextIdentifier();

	if ( identifier.empty() ) {
		// Excpected macro identifier for preprocessor conditional {1}.
		throw error::C1016( DirectiveInfo::Ifdef.getIdentifier() );
		// emitMessage( err );
	}

	finishDirective( false );

	if ( m_tokenExpression.token == TOK_END_OF_FILE ) {
		// Unexpected end of file.
		throw error::C1004();
	}

	m_conditionalStack.push( location );

	MacroSet::const_iterator itMacro = m_macros.find( identifier );

	if ( itMacro != m_macros.end() ) {
		// Not found: set scanner into conditional false mode.
		assert( m_pTokenStream == m_pScanner );
		m_pScanner->pushContext( CTX_CONDITIONAL_FALSE );
	}

	
}

/**
** @brief Process the \c \#else directive.
**
** @example input/Ifdef1Test.h
** @example input/Ifndef1Test.h
** @example input/If1Test.h
** @example input/If2Test.h
** 
*/
void Processor::processElseDirective()
{
	if ( m_conditionalStack.size() == 0 ) {
		// Unexpected #else
		throw error::C1019();
	}

	Context context = m_tokenExpression.getContext();

	if ( context == CTX_CONDITIONAL_FALSE ) {
		m_pScanner->popContext( CTX_CONDITIONAL_FALSE );
	} else {
		m_pScanner->pushContext( CTX_CONDITIONAL_DONE );
	}
}

/**
** @brief Process the \c \#elif directive.
*/
void Processor::processElifDirective()
{
	if ( m_conditionalStack.size() == 0 ) {
		// Unexpected #elif
		throw error::C1018();
	}

	Context context = m_tokenExpression.getContext();
	if ( context == CTX_CONDITIONAL_FALSE ) {
		m_pScanner->popContext( CTX_CONDITIONAL_FALSE );

		bool isTrue = evaluateConditionalDirective();

		if ( !isTrue ) {
			m_pScanner->pushContext( CTX_CONDITIONAL_FALSE );
		}
	} else {
		m_pScanner->pushContext( CTX_CONDITIONAL_DONE );
	}
}

/**
** @brief Process the \c \#endif directive.
*/
void Processor::processEndifDirective()
{
	Context context = m_tokenExpression.getContext();

	if ( m_conditionalStack.size() == 0 ) {
		// Unexpected #endif
		throw error::C1020();
	}

	m_conditionalStack.pop();
	if ( context == CTX_CONDITIONAL_FALSE ) {
		m_pScanner->popContext( CTX_CONDITIONAL_FALSE );
	} else if ( context == CTX_CONDITIONAL_DONE ) {
		m_pScanner->popContext( CTX_CONDITIONAL_DONE );
	}

	finishDirective( false );


}

/**
** @brief Process the \c \#message directive.
*/
void Processor::processMessageDirective()
{
	finishDirective( true );
}

/**
** @brief Process the \c \#error directive.
*/
void Processor::processErrorDirective()
{
	throw error::C1189(L"User defined error");
}

/**
** @brief Process the \c \#line directive.
*/
void Processor::processLineDirective()
{
	finishDirective( true );
}

/**
** @brief Process the \c \#pragma directive.
**
** @todo Currently only \#pragma once is supported. Make options
**       ready for pushing and poping options. Support 
**       - \#pragma push( option )
**       - \#pragma option( id, value )
**       - \#pragma pop( option )
*/
void Processor::processPragmaDirective()
{
	const wstring identifier = getNextIdentifier();

	if ( identifier == L"once" ) {
		processPragmaOnceDirective();
	} else if ( identifier == L"message" ) {
		processPragmaMessageDirective();
	}
}

/**
** @brief Process the \c \#pragam once directive.
*/
void Processor::processPragmaOnceDirective()
{
	File& file = getFile();
	file.setIncludeOnce( true );
	m_includeOnceFiles.insert( file.getPath() );
}

/**
** @brief Process the \c \#pragam message directive.
*/
void Processor::processPragmaMessageDirective()
{
	wstringstream messageBuffer;
	bool bIgnoreNewLine = false;
	bool bContinue      = true;
	while ( bContinue ) {
		Token token = getNextToken();

		switch ( token ) {
			case TOK_NEW_LINE:
				if ( !bIgnoreNewLine ) {
					processNewLine();
					bContinue = false;
				} else {
					processNewLine();
					bIgnoreNewLine = false;
				}
				break;
			case TOK_OTHER:
				bIgnoreNewLine = m_tokenExpression.text == L"\\";
				break;
			case TOK_BLOCK_COMMENT:
				processBlockComment();
				break;
			case TOK_SPACE:
				messageBuffer << m_tokenExpression.text;
				break;
			case TOK_END_OF_FILE:
				bContinue = false;
				return;
			default:
				messageBuffer << m_tokenExpression.text;
				bIgnoreNewLine = false;
				break;
		}
	}
}

/**
** @brief Process the \c \#import directive.
**
** @todo Currently don't know if this is anything good for.
*/
void Processor::processImportDirective()
{
	throw NotSupportedError();
}

/**
** @brief Process the \c \#using directive.
**
** @todo Windows/.NET: Load dll and go through enumerations. 
**       Make all enumerations values global macros.
**       Before this is done internal macro collection must be made
**       namespace ready.\p
**       Java: Load class file name make all static final int variables
**       global macros.
*/
void Processor::processUsingDirective()
{
	throw NotSupportedError();
}

/**
** @brief Process the \c \#exec directive.
** 
** Read the rest of the line an try to execute it as a program.
** Redirect stdout to the preprocessor ouput.
** 
** @todo Collect command line and execute program. Redirect stdout and stderr to sqtpp streams.
*/
void Processor::processExecDirective()
{
	throw NotSupportedError();
}



/**
** @brief Process the S4M AdSalesNG directive: --[identifier] 
** 
*/
void Processor::processAdSalesNGDirective()
{
	assert( m_options.supportAdSalesNG() );
	wstring identifier = m_tokenExpression.getIdentifier();

	if ( identifier == L"MODPROC" ) {
		// --[MODPROC] procedure / file name
		Token   token = getNextToken();
		if ( token == TOK_SPACE ) {
			token = getNextToken();
		}
		if ( token != TOK_IDENTIFIER ) {
			// Missing file path for \#include directive. Found {1}.
			throw error::C2006( m_tokenExpression.getText() );
		}
		wstring sFilePath   = m_tokenExpression.getText() + L".syb";
		File&   currentFile = getFile();
		wstring sFullPath   = currentFile.findFile( sFilePath, m_options.getIncludeDirectories(), true );

		if ( sFullPath.empty() ) {
			// Cannot open file: '{1}': No such file or directory.
			throw error::C1083( sFilePath );
		}

		File::checkFile( sFullPath );
		finishDirective( false );
		processFile( sFullPath );
	} else {
		m_outputBuffer << m_tokenExpression.getText();
	}
}


} // namespace sqtpp

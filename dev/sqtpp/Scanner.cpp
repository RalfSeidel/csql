/**
** @file
** @author Ralf Seidel
** @brief Definition of the lexical scanner (#sqtpp::Scanner).
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#include "stdafx.h"
#include "Context.h"
#include "Directive.h"
#include "Exceptions.h"
#include "Options.h"
#include "Logger.h"
#include "Error.h"
#include "TextScanner.h"
#include "Scanner.h"

namespace sqtpp {

/// The start of a multi line comment.
const wchar_t* const Scanner::m_szBlockCommentStart = L"/*";

/// The end of a multi line comment.
const wchar_t* const Scanner::m_szBlockCommentEnd   = L"*/";


/**
** @brief Scanner constructor.
*/
Scanner::Scanner( const sqtpp::Options& options )
: m_options( options )
, m_context( CTX_DEFAULT )
, m_wcFirstNonSpaceChar( L'\0' )
, m_wcLastNonSpaceChar( L'\0' )
, m_lastToken( *new TokenExpression() )
{
}

/**
** @brief Scanner destructor.
*/
Scanner::~Scanner()
{
	delete &m_lastToken;
}


/**
** @brief Static scanner creator.
*/
Scanner* Scanner::createScanner( const Options& options )
{
	Options::Language language = options.getLanguage();
	switch( language ) {
		case Options::LNG_UNDEFINED:
			throw UnexpectedSwitchError();
		case Options::LNG_TEXT:
			return new TextScanner( options );
		case Options::LNG_XML:
			return new HtmlScanner( options );
		case Options::LNG_C:
			return new Scanner( options );
		case Options::LNG_CPP:
			return new Scanner( options );
		case Options::LNG_ASM:
			// return new Scanner( options );
			throw NotSupportedError();
		case Options::LNG_RC:
			// return new Scanner( options );
			throw NotSupportedError();
		case Options::LNG_SQL:
			return new Scanner( options );
		default:
			throw UnexpectedSwitchError();
	}
}

/**
** @brief Get the locale for character classification.
*/
const std::locale& Scanner::getLocale() const
{
	return std::locale::classic();
}


/**
** @brief Get next character from input stream if equal to given character.
**
** @return - true if next character is equal to the given character. In this
**           case the read position advances and the character is put into
**           the token string buffer.
**         - false otherwise. The seek position in the input stream stays 
**           where it is.
*/
bool Scanner::readIfEqual( std::wistream& input, wchar_t next )
{
	bool bNextIsEqual = false;
	wchar_t ch = input.peek();
	if ( !input.eof() ) {
		bNextIsEqual = ch == next;
	}
	if ( bNextIsEqual ) {
		input.get();
		m_tokenBuffer << ch;
	}
	return bNextIsEqual;
}


/**
** @brief Get next character from input stream if equal to given character.
**
** @return - true if next character is equal to the given character. In this
**           case the read position advances and the character is put into
**           the token string buffer.
**         - false otherwise. The seek position in the input stream stays 
**           where it is.
*/
wchar_t Scanner::readIfOneOf( std::wistream& input, const wchar_t* next )
{
	wchar_t ch = input.peek();
	if ( input.eof() ) {
		return L'\0';
	}

	if ( wcschr( next, ch ) == NULL ) {
		ch = L'\0';
	} else {
		input.get();
		m_tokenBuffer << ch;
	}
	return ch;
}

/**
** @brief Read an identifier.
*/
void Scanner::readIdentifier( std::wistream& input, std::wostream& output  )
{
	for( ;; ) {
		wchar_t ch = input.peek();
		if ( input.eof() ) {
			break;
		} else if ( this->isIdentifierContinued( ch ) ) {
			ch = input.get();
			output.put( ch );
		} else {
			break;
		}
	}
}

/**
** @brief Read an identifier token.
*/
void Scanner::readIdentifier( std::wistream& input )
{
	readIdentifier( input, m_tokenBuffer );
}


/**
** @brief Check if given identifier is a keyword
** 
** @returns if the identifier is a keyword the method returns the 
**          corresponding token. Otherwise TOK_IDENTIFIER is returned.
*/
Token Scanner::translateIdentifier( const wstring& identifier ) const
{
	Token token = TOK_IDENTIFIER;

	if ( identifier == L"defined" ) {
		token = TOK_OP_DEFINED;
	}
	return token;
}


/**
** @brief Read a whole integer.
**
** @param input The input stream.
** @param wcCurrent The first character of the number (last character read).
*/
void Scanner::readNumber( std::wistream& input, wchar_t wcCurrent )
{
	wchar_t ch    = wcCurrent;
	bool    isHex = false;

	if ( input.eof() ) {
		return;
	}

	// Check if hexadecimal or octal
	if ( ch == L'0' ) {
		ch = input.peek();
		isHex = ch == 'x' || ch == 'X';
		if ( isHex ) {
			input.get( ch );
			this->m_tokenBuffer.put( ch );
		}
	}

	for( ;; ) {
		if ( input.eof() ) {
			break;
		}
		wchar_t ch = input.peek();
		bool    isDigit;
		if ( isHex ) {
			isDigit = std::isxdigit( ch, this->getLocale() );
		} else {
			isDigit = std::isdigit( ch, this->getLocale() );
		}
		if ( isDigit ) {
			input.get();
			this->m_tokenBuffer.put( ch );
		} else {
			break;
		}
	}
}

/**
** @brief Read a whole string. 
*/
Token Scanner::continueString( std::wistream& input, wchar_t delimiter )
{
	Token token = TOK_UNDEFINED;

	if ( m_context != CTX_SQUOTE_STRING && m_context != CTX_DQUOTE_STRING ) {
		if ( delimiter == L'\'' ) {
			pushContext( CTX_SQUOTE_STRING );
		} else if ( delimiter == L'\"' ) {
			pushContext( CTX_DQUOTE_STRING );
		}
	}

	for( ;; ) {
		wchar_t ch = input.peek();

		if ( input.eof() ) {
			// Unexpected end of file.
			throw error::C1004();
		}

		if ( isNewLine( ch ) ) {
			if ( m_options.multiLineStringLiterals() ) {
				// First character scanned by this call?
				if ( token == TOK_UNDEFINED ) {
					token = TOK_NEW_LINE;
					input.get();
					readNewLine( input, ch );
				}
				break;
			} else {
				// String exceeds line.
				throw error::C2001();
			}
		}

 		token = TOK_STRING; 
		input.get();
		m_tokenBuffer.put( ch );
			
		if ( ch == delimiter ) {
			// End of string?
			// If next character is a quote again continue scanning.
			if ( m_options.getStringQuoting() == Options::QUOT_DOUBLE ) {
				if ( readIfEqual( input, delimiter ) ) {
					continue;
				}
			}
			// Revert to previous context.
			popContext( m_context );
			break;
		} else if ( ch == L'\\' && m_options.getStringQuoting() == Options::QUOT_ESCAPE ) {
			ch = input.peek();

			if ( isNewLine( ch ) ) {
				if ( !m_options.multiLineStringLiterals() ) {
					// String exceeds line.
					throw error::C2001();
				}
				break;
			} else {
				input.get();
				m_tokenBuffer.put( ch );
			}
		}
	}

	return token;
}


/**
** @brief Continue fetching a new line token.
** 
** @param input the input stream.
** @param wcCurrent The first new line character encountered (CR or LF)
*/
void Scanner::readNewLine( std::wistream& input, wchar_t wcCurrent )
{
	switch ( m_options.getNewLineOutput() ) {
		case Options::NLO_AS_IS:
			m_tokenBuffer.put( wcCurrent );
			break;
		case Options::NLO_OS_DEFAULT:
			m_tokenBuffer << m_options.getOsDefaultNewLine();
			break;
		case Options::NLO_LF: 
			m_tokenBuffer.put( L'\n' );
			break;
		case Options::NLO_CR:
			m_tokenBuffer.put( L'\r' );
			break;
		case Options::NLO_CRLF:
			m_tokenBuffer.write( L"\r\n", 2 );
			break;
		default:
			throw UnexpectedSwitchError();
	}

	// CRLF or CR only? (Windows or Mac?)
	if ( wcCurrent == L'\r' ) {
		// Discard next character.
		wchar_t wcNext = input.peek();
		if ( wcNext == L'\n' ) {
			input.get();
			if ( m_options.getNewLineOutput() == Options::NLO_AS_IS ) {
				m_tokenBuffer.put( wcNext );
			}
		}
	}
}

/**
** @brief Continue fetching white space characters.
** 
** @param input The input stream.
*/
void Scanner::readSpace( std::wistream& input )
{
	for(;;) {
		wchar_t wcNext = input.peek();

		if ( input.eof() ) {
			break;
		} 

		if ( isSpace( wcNext ) ) {
			input.get();
			m_tokenBuffer.put( wcNext );
		} else {
			break;
		}
	}
}


/**
** @brief Determine token which is introduced with a forward slash ('.').
**
** @return TOK_HELLIP if a sequence of three dots have been found. 
**         Otherwise TOK_OTHER is returned.
*/
Token Scanner::getDotToken( std::wistream& input )
{
	Token token = TOK_OTHER;

	if ( readIfEqual( input, L'.' ) && readIfEqual( input, L'.' ) ) {
		token = TOK_HELLIP;
	}
	return token;
}


/**
** @brief Determine token which is introduced with a forward slash ('/').
**
** @param  input The input stream.
** @return Token found (TOK_OTHER or TOK_LINE_COMMENT).
*/
Token Scanner::getSlashToken( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	wchar_t wcNext = readIfOneOf( input, L"*/" );

	switch ( wcNext ) {
		case L'/':
			token = continueLineComment( input, TOK_LINE_COMMENT, false );
			break;
		case L'*':
			token = TOK_BLOCK_COMMENT;
			continueBlockComment( input, m_szBlockCommentEnd, false );
			break;
		default:
			token = TOK_OP_DIVIDE;
			break;
	}
	return token;
}

/**
** @brief Determine token which is introduced with a dash ('-').
**
** @param  input The input stream.
** @return Token found (TOK_OTHER, TOK_SQL_LINE_COMMENT or TOK_ADSALESNG_DIRECTIVE).
*/
Token Scanner::getDashToken( std::wistream& input )
{
	if ( m_options.getLanguage() != Options::LNG_SQL )
		return TOK_OP_MINUS;

	if ( !readIfEqual( input, L'-' ) )
		return TOK_OP_MINUS;

	Token token = TOK_UNDEFINED;
	if ( m_options.supportAdSalesNG() ) {
		// Check for --[identifier]
		if ( readIfEqual( input, L'[' ) ) {
			// --[
			wchar_t wcNext = input.peek();
			if ( isIdentifierBegin( wcNext ) ) {
				// --[identifier
				wstringstream buffer;
				readIdentifier( input, buffer );
				wstring identifier = buffer.str();
				m_tokenBuffer << identifier;
				if ( readIfEqual( input, L']' ) ) {
					// --[identifier]
					m_tokenIdentifier = identifier;
					token = TOK_ADSALESNG_DIRECTIVE;
				}
			}
		}
	}
	// Not an AdSales NG directive? Return just a simple line comment token.
	if ( token == TOK_UNDEFINED ) {
		if ( m_options.keepSqlComments() ) {
			token = TOK_OP_MINUS;
		} else {
			// token = TOK_SQL_LINE_COMMENT;
			token = continueLineComment( input, TOK_SQL_LINE_COMMENT, false );
		}
	}
	return token;
}


/**
** @brief Determine token which is introduced with a sharp symbol ('#').
**
** If the # is the first (non blank) character in the current scanner 
** line it is expected that the next following characters are an 
** identifier. If not the methode will raise the "Unknown preprocessor directive"
** error (C1021). If the directive is well formed the scanner will examine
** the identifier and return the corresponding directive token (TOK_DIR_?). 
** The scanners current token identifier is set to the identifier found.
**
** @param  input The input stream.
** @return Token found.
*/
Token Scanner::getSharpToken( std::wistream& input )
{
	wchar_t wcNext = input.peek();

	if ( input.eof() ) {
		// EOF --> SHARP only
		return TOK_SHARP;
	}
	if ( wcNext == L'#' ) {
		// ##
		input.get();
		m_tokenBuffer.put( wcNext );
		return TOK_SHARP_SHARP;
	} else if ( wcNext == L'@' ) {
		// #@ (MS charize)
		input.get();
		m_tokenBuffer.put( wcNext );
		return TOK_SHARP_AT;
	}

	// If we are not at the beginning of a line a # ist just a #.
	if ( m_wcFirstNonSpaceChar != '\0' ) {
		return TOK_SHARP;
	}

	if ( isSpace( wcNext ) ) {
		// Skip space
		input.get();
		m_tokenBuffer.put( wcNext );
		readSpace( input );
		if ( input.eof() ) {
			return TOK_SHARP;
		}
		wcNext = input.peek();
		if ( input.eof() ) {
			return TOK_SHARP;
		}
	}

	// A # character without an identifier is allowed.
	if ( isNewLine( wcNext ) )
		return TOK_SHARP;

	if ( isIdentifierBegin( wcNext ) ) {
		wstringstream buffer;
		wstring       identifier;
		streamsize    length;

		input.get();
		buffer.put( wcNext );
		readIdentifier( input, buffer );
		identifier = buffer.str();
		length     = streamsize(identifier.length());

		m_tokenBuffer.write( identifier.c_str(), length );
		m_tokenIdentifier = identifier;

		// Skip any white space following the directive.
		readSpace( input );

		const DirectiveInfo* pDirectiveInfo = DirectiveInfo::findDirectiveInfo( identifier );
		if ( pDirectiveInfo == NULL ) {
			return TOK_DIRECTIVE;
		} else {
			return pDirectiveInfo->getToken();
		}
	} else {
		// Unknown preprocessor directive {1}.
		throw error::C1021( wstring( 1, wcNext ) );
	}
}


/**
** @brief Determine if the backslash ('\\') is the last symbol in the current line.
**
** @param  input The input stream.
** @return Token found (TOK_OTHER or TOK_EOL_BACKSLASH)
*/
Token Scanner::getBackSlashToken( std::wistream& input )
{
	wchar_t wcNext = input.peek();

	if ( input.eof() ) {
		// EOF --> \ only
		return TOK_OTHER;
	}
	if ( isNewLine( wcNext ) ) {
		return TOK_EOL_BACKSLASH;
	} else {
		return TOK_OTHER;
	}
}

/**
** @brief Determine the token introduced with a less than ('<') character.
**
** @param  input The input stream.
** @return Token found (TOK_OP_LT (<), TOK_OP_LE (<=) or TOK_OP_LSHIFT (<<)).
*/
Token Scanner::getLtToken( std::wistream& input )
{
	wchar_t wcNext       = input.peek();
	bool    bConsumeChar = true;
	Token   token;

	switch ( wcNext ) {
	case L'=':
		token = TOK_OP_LE;
		break;
	case L'<':
		token = TOK_OP_LSHIFT;
		break;
	default:
		token = TOK_OP_LT;
		bConsumeChar = false;
		break;
	} // switch

	if ( bConsumeChar ) {
		input.get();
		m_tokenBuffer << wcNext;
	}
	return token;
}


/**
** @brief Determine the token introduced with a greater than ('>') character.
**
** @param  input The input stream.
** @return Token found (TOK_OP_GT (>), TOK_OP_GE (>=) or TOK_OP_RSHIFT (>>)).
*/
Token Scanner::getGtToken( std::wistream& input )
{
	wchar_t wcNext       = input.peek();
	bool    bConsumeChar = true;
	Token   token;

	switch ( wcNext ) {
	case L'=':
		token = TOK_OP_GE;
		break;
	case L'>':
		token = TOK_OP_RSHIFT;
		break;
	default:
		token = TOK_OP_GT;
		bConsumeChar = false;
		break;
	} // switch

	if ( bConsumeChar ) {
		input.get();
		m_tokenBuffer << wcNext;
	}
	return token;
}

/**
** @brief Determine the token introduced with a equal ('=') character.
**
** @param  input The input stream.
** @return Token found (TOK_OP_ASSIGN (=) or TOK_OP_GE (==).
*/
Token Scanner::getEqToken( std::wistream& input )
{
	wchar_t wcNext       = input.peek();
	bool    bConsumeChar = true;
	Token   token        = TOK_UNDEFINED;

	switch ( wcNext ) {
	case L'=':
		token = TOK_OP_EQ;
		break;
	default:
		token = TOK_OP_ASSIGN;
		bConsumeChar = false;
		break;
	} // switch

	if ( bConsumeChar ) {
		input.get();
		m_tokenBuffer << wcNext;
	}
	return token;
}


/**
** @brief Determine the token introduced with a equal ('!') character.
*/
Token Scanner::getNotToken( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	if ( readIfEqual( input, L'=' ) ) {
		token = TOK_OP_NE;
	} else {
		token = TOK_OP_LOGICAL_NOT;
	}
	return token;
}

/**
** @brief Determine the token introduced with a equal ('&') character.
*/
Token Scanner::getAndToken( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	if ( readIfEqual( input, L'&' ) ) {
		token = TOK_OP_LOGICAL_AND;
	} else {
		token = TOK_OP_BIT_AND;
	}
	return token;
}

/**
** @brief Determine the token introduced with a equal ('|') character.
*/
Token Scanner::getOrToken( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	if ( readIfEqual( input, L'|' ) ) {
		token = TOK_OP_LOGICAL_OR;
	} else {
		token = TOK_OP_BIT_OR;
	}
	return token;
}


/**
** @brief Determine the token introduced with a caret ('^') character.
*/
Token Scanner::getXorToken( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	if ( readIfEqual( input, L'^' ) ) {
		token = TOK_OP_LOGICAL_XOR;
	} else {
		token = TOK_OP_BIT_XOR;
	}
	return token;
}


/**
** @brief Continue scanning in default mode / context.
*/
Token Scanner::continueDefault( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	if ( input.eof() ) {
		token = TOK_END_OF_FILE;
	} else {
		wchar_t wcNext = input.get();
		if ( input.eof() ) {
			token = TOK_END_OF_FILE;
		} else if ( isNewLine( wcNext ) ) {
			// don't put character to token text buffer because
			// it may be translated.
			token = TOK_NEW_LINE;
			readNewLine( input, wcNext );
		} else if ( this->isIdentifierBegin( wcNext ) ) {
			m_tokenBuffer.put( wcNext );
			token = TOK_IDENTIFIER;
			readIdentifier( input );

			const wstring identifier = m_tokenBuffer.str();
			token = translateIdentifier( identifier );
		} else if ( std::isdigit( wcNext, locale::classic() ) ) {
			m_tokenBuffer.put( wcNext );
			token = TOK_NUMBER;
			readNumber( input, wcNext );
		} else {
			m_tokenBuffer.put( wcNext );
			switch ( wcNext ) {
				case L'.':
					token = getDotToken( input );
					break;
				case L'#':
					token = getSharpToken( input );
					break;
				case L'/':
					token = getSlashToken( input );
					break;
				case L'-':
					token = getDashToken( input );
					break;
				case L'+':
					token = TOK_OP_PLUS;
					break;
				case L'*':
					token = TOK_OP_MULTIPLY;
					break;
				case L'%':
					token = TOK_OP_MODULUS;
					break;
				case L'\\':
					token = getBackSlashToken( input );
					break;
				case L'"':
				case L'\'':
					token = TOK_STRING;
					continueString( input, wcNext );
					break;
				case L'<':
					if ( getContext() == CTX_INCLUDE_DIRECTIVE ) {
						token = TOK_SYS_INCLUDE;
						continueString( input, L'>' );
					} else {
						token = getLtToken( input );
					}
					break;
				case L'>':
					token = getGtToken( input );
					break;
				case L'=':
					token = getEqToken( input );
					break;
				case L'!':
					token = getNotToken( input );
					break;
				case L'&':
					token = getAndToken( input );
					break;
				case L'|':
					token = getOrToken( input );
					break;
				case L'^':
					token = getXorToken( input );
					break;
				case L'~':
					token = TOK_OP_BIT_NOT;
					break;
				case L',':
					token = TOK_OP_COMMA;
					break;
				case L'(':
					token = TOK_LEFT_PARENTHESIS;
					break;
				case L')':
					token = TOK_RIGHT_PARENTHESIS;
					break;
				default:
					if ( isSpace( wcNext ) ) {
						readSpace( input );
						token = TOK_SPACE;
					} else {
						token = TOK_OTHER;
					}
					break;
			} // end switch
		}
	}

	return token;
}

/**
** @brief Continue scanning a line comment.
**
** @param input The input stream.
** @param contextToken The line comment token that has been found previously. 
**        This can be TOK_LINE_COMMENT or TOK_SQL_LINE_COMMENT.
** @param bFollowup False if this is the first call for a block comment found
**        i.e. if /* has just been read. True if the scanning continued in 
**        context CTX_LINE_COMMENT right from the scanner entry point (getNextToken).
*/
Token Scanner::continueLineComment( std::wistream& input, const Token contextToken, const bool bFollowup )
{
	Token   token = bFollowup ? TOK_UNDEFINED : contextToken;

	if ( input.eof() ) {
		return TOK_END_OF_FILE;
	}

	while ( input.good() ) {
		wchar_t wc = input.peek();
		if ( input.eof() ) {
			// error: eof of file in comment block.
			if ( token == TOK_UNDEFINED ) {
				token = TOK_END_OF_FILE;
			} else {
				// Some characters have alread been read. 
				// Return them to the caller as a line comment token
				// before returning the eof token.
			}
			break;
		}
		if ( isNewLine( wc ) ) {
			if ( token == TOK_UNDEFINED ) {
				input.get();
				readNewLine( input, wc );
				return TOK_NEW_LINE;
			} else {
				// Some characters have already been read. 
				// Return them to the caller as a line comment token.
				return contextToken;
			}
			break;
		} else if ( wc == L'\\' ) {
			if ( token == TOK_UNDEFINED ) {
				input.get();
				wchar_t wcNext = input.peek();
				if ( isNewLine( wcNext ) ) {
					return TOK_EOL_BACKSLASH;
				} else {
					m_tokenBuffer.put( wc );
					continue;
				}
			} else {
				// Some characters have already been read. 
				// Return them to the caller as a line comment token
				// and continue with the processing in the next call.
				return contextToken;
			}
		} else {
			input.get( wc );
			m_tokenBuffer.put( wc );
			token = contextToken;
		}
	}

	return token;
}

/**
** @brief Continue scanning a block comment.
**
** @param input The input stream.
** @param pszCommentEnd The string that makes the end of the comment.
** @param bFollowup False if this is the first call for a block comment found
**        i.e. if /* has just been read. True if the scanning continued in 
**        context CTX_BLOCK_COMMENT right from the scanner entry point (getNextToken).
*/
Token Scanner::continueBlockComment( std::wistream& input, const wchar_t* pszCommentEnd, bool bFollowup )
{
	Token    token      = bFollowup ? TOK_UNDEFINED : TOK_BLOCK_COMMENT;
	size_t   eocLength  = wcslen( pszCommentEnd );
	size_t   eocBytes   = (eocLength + 1 )* sizeof( wchar_t );
	wchar_t* pEocTest   = (wchar_t*)alloca( eocBytes  );
	size_t   eocIndex   = 0;

	wchar_t  wc;

	memset( pEocTest, 0, eocBytes  );

	pEocTest[eocLength] = L'\0';

	if ( m_context != CTX_BLOCK_COMMENT ) {
		m_contextStack.push( m_context );
		m_context = CTX_BLOCK_COMMENT;
	}


	while ( input.good() ) {
		wc = input.peek();
		if ( input.eof() ) {
			// error: eof of file in comment block.
		}

		if ( isNewLine( wc ) ) {
			if ( token == TOK_UNDEFINED ) {
				input.get();
				readNewLine( input, wc );
				token = TOK_NEW_LINE;
			} else {
				// Some characters have already been read. 
				// Return them to the caller as a line comment token.
			}
			break;
		}
		input.get();
		m_tokenBuffer.put( wc );

		if ( wc != '\0' ) {
			if ( eocIndex == eocLength ) {
				memmove( pEocTest, pEocTest + 1, eocBytes - sizeof( wchar_t ) * 2 );
				pEocTest[eocLength-1] = wc;
			} else {
				pEocTest[eocIndex] = wc;
				eocIndex++;
			}

			if ( wcscmp( pEocTest, pszCommentEnd ) == 0 ) {
				// Last n characters read have been equal to
				// the end of comment expression --> stop reading.
				popContext( CTX_BLOCK_COMMENT );
				break;
			}
		}
		token = TOK_BLOCK_COMMENT;
	}


	return token;
}

/**
** @brief Continue false evaluated conditional block.
*/
Token Scanner::continueConditional( std::wistream& input )
{
	Token   token = TOK_UNDEFINED;
	wchar_t wc;
	bool    bSpaceOnly = true;

	assert( m_context == CTX_CONDITIONAL_FALSE || m_context == CTX_CONDITIONAL_DONE );

	while ( input.good() ) {
		m_tokenBuffer.str( std::wstring() );
		m_tokenBuffer.clear();

		input.get( wc );
		if ( input.eof() ) {
			// Error: end of file while in conditional.
			return TOK_END_OF_FILE;
		}

		if ( isNewLine( wc ) ) {
			token = TOK_NEW_LINE;
			readNewLine( input, wc );
			return token;
		}

		if ( bSpaceOnly && wc == L'#' ) {
			token = getSharpToken( input );
			switch ( token ) {
				case TOK_DIR_IF:
				case TOK_DIR_IFDEF:
				case TOK_DIR_IFNDEF:
					// Conditionals can be nested.
					pushContext( m_context );
					break;
				case TOK_DIR_ELSE:
				case TOK_DIR_ELIF:
					// Remove nested conditionals from the context stack 
					// before returning token to the processor.
					if ( m_context == CTX_CONDITIONAL_DONE ) {
						// true branch has already been found -> wait for \#endif
						break;
					}
					if ( m_contextStack.size() == 1 ) {
						Context context = m_contextStack.top();
						if ( context != CTX_CONDITIONAL_FALSE ) {
							// Return to processor and let him decide what to do with the directive.
							return token;
						}
					}
					break;

				case TOK_DIR_ENDIF:
					if ( m_contextStack.size() <= 1 ) {
						return token;
					} else {
						popContext( m_context );
					}
					break;
			}
		}
		if ( !isSpace( wc ) ) {
			bSpaceOnly = false;
		}
	}
	return token;
}

/**
** @brief Public interface methode to get the next token.
**
** The methode will clear the scanners token buffer (m_tokenBuffer) 
** and call the core method (getNextTokenCore()). Inherited class
** should override getNextTokenCore if they have to extend or limit
** the token translation.
*/ 
Token Scanner::getNextToken( std::wistream& input, TokenExpression& tokenExpression )
{
	Token token;

	m_tokenBuffer.str( std::wstring() );
	m_tokenBuffer.clear();
	m_tokenIdentifier.clear();

	if ( input.eof() ) {
		token = TOK_END_OF_FILE;
	} else {
		token = getNextTokenCore( input );
	}

	m_lastToken = TokenExpression();
	m_lastToken.token   = token;
	m_lastToken.context = getContext();
	m_lastToken.text    = m_tokenBuffer.str();
	if ( m_tokenIdentifier.empty() ) {
		m_lastToken.identifier = m_lastToken.getText();
	} else {
		m_lastToken.identifier = m_tokenIdentifier;
	}

	if ( token == TOK_NEW_LINE || token == TOK_END_OF_FILE ) {
		// If the current line is empty clear the remembered last line character
		// of the previous line.
		if ( m_wcFirstNonSpaceChar == L'\0' ) {
			m_wcLastNonSpaceChar = L'\0';
		}
		m_wcFirstNonSpaceChar = L'\0';
	} else if ( token != TOK_SPACE && !m_lastToken.getText().empty() ) {
		const wstring& tokenText = m_lastToken.getText();

		m_wcLastNonSpaceChar = tokenText[tokenText.length() - 1];
		if ( m_wcFirstNonSpaceChar == L'\0' ) {
			m_wcFirstNonSpaceChar = tokenText[0];
		}
	}

	onTokenScanned();

	tokenExpression = m_lastToken;
	return token;
}

/**
** @brief Change current context.
*/ 
void Scanner::pushContext( Context newContext )
{
	m_contextStack.push( m_context );
	m_context = newContext;
}


/**
** @brief Restore previous context.
*/ 
void Scanner::popContext( Context context )
{
	assert( m_context == context );

	if ( m_contextStack.size() == 0 ) {
		throw error::C1001( L"Scanner::popContext - Scanner context text is empty" );
	}
	m_context = m_contextStack.top();
	m_contextStack.pop();
}

/**
** @brief Get the last token scanned.
*/
Token Scanner::getLastToken() const throw() 
{ 
	return m_lastToken.token; 
}

/**
** @brief Get the last character read.
*/
const wstring& Scanner::getLastTokenText() const throw() 
{ 
	return m_lastToken.getText(); 
}

/**
** @brief Get the last identifier read.
*/
const wstring& Scanner::getLastTokenIdentifier() const throw() 
{ 
	return m_lastToken.getIdentifier(); 
}



/**
** @brief Read the next characters from the input stream and translate them into 
** scanner tokens.
*/ 
Token Scanner::getNextTokenCore( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	switch ( m_context ) {
		case CTX_DEFAULT:
		case CTX_INCLUDE_DIRECTIVE:
			token = continueDefault( input );
			break;
		case CTX_LINE_COMMENT:
			token = continueLineComment( input, m_lastToken.token, true );
			break;
		case CTX_BLOCK_COMMENT:
			token = continueBlockComment( input, m_szBlockCommentEnd, true );
			break;
		case CTX_CONDITIONAL_FALSE:
		case CTX_CONDITIONAL_DONE:
			token = continueConditional( input );
			break;
		case CTX_SQUOTE_STRING:
			token = continueString( input, L'\'' );
			break;
		case CTX_DQUOTE_STRING:
			token = continueString( input, L'\"' );
			break;
		default:
			throw UnexpectedSwitchError( "Unknown context" );
			break;
	}
	return token;
}


/**
** @brief Check if the given character is white space but not new line
*/
bool Scanner::isSpace( wchar_t ch ) const
{
	return std::isspace( ch, this->getLocale() ) && !isNewLine( ch );
}

/**
** @brief Check if the given character is a new line character.
*/
bool Scanner::isNewLine( wchar_t ch ) const
{
	return ch == L'\r' || ch == L'\n';
}

/**
** @brief Check if the given character is one of the supported identifier start characters.
*/
bool Scanner::isIdentifierBegin( wchar_t ch ) const
{
	if ( std::isalpha( ch, this->getLocale() ) ) {
		return true;
	} 
	switch ( ch ) {
		case L'_':
			return true;
		case L'§':
		case L'$':
			// Special handling for AdSales NG (S4M Scripts)
			if ( m_options.getLanguage() == Options::LNG_SQL ) {
				return true;
			}
	}
	return false;
}

/**
** @brief Check if the given character is one of the supported identifier center characters.
**
*/
bool Scanner::isIdentifierContinued( wchar_t ch ) const
{
	return isIdentifierBegin( ch ) || std::isdigit( ch, this->getLocale() );
}



/**
** @brief Token scanned event handler.
**
** Inform delegates after a token has been scanned.
*/
void Scanner::onTokenScanned()
{
//	const TokenInfo& ti = TokenInfo::getTokenInfo( m_lastToken.getToken() );
//	wclog << ti.pwcSymbol << endl;
}

} // namespace sqtpp

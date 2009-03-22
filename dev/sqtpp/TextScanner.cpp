#include "stdafx.h"
#include "Context.h"
#include "Token.h"
#include "TextScanner.h"

namespace sqtpp {

// --------------------------------------------------------------------
// TextScanner
// --------------------------------------------------------------------

/**
** @brief Read the next characters from the input stream and translate them into 
** scanner tokens.
*/ 
Token TextScanner::getNextTokenCore( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	for (;;) {
		wchar_t wcNext = input.get();
		if ( input.eof() ) {
			break;
		}
		switch ( wcNext ) {
			case L'\r':
			case L'\n':
				if ( token == TOK_UNDEFINED ) {
					readNewLine( input, wcNext );
					token = TOK_NEW_LINE;
				} else {
					m_tokenBuffer.unget();
				}
				break;
			default:
				token = TOK_OTHER;
				m_tokenBuffer.put( wcNext );
				break;
		}
	}
	return token;
}


// --------------------------------------------------------------------
// HtmlScanner
// --------------------------------------------------------------------


/// The start of a HTML/XML multi line comment.
const wchar_t* const HtmlScanner::m_szBlockCommentStart = L"<!--";
/// The end of a HTML/XML multi line comment.
const wchar_t* const HtmlScanner::m_szBlockCommentEnd   = L"-->";

/**
** @brief Read the next characters from the input stream and translate them into 
** scanner tokens.
*/ 
Token HtmlScanner::getNextTokenCore( std::wistream& input )
{
	Token token = TOK_UNDEFINED;

	if ( getContext() == CTX_BLOCK_COMMENT ) {
		continueBlockComment( input, m_szBlockCommentEnd, true );
	} else {
		for (;;) {
			wchar_t wcNext = input.peek();
			if ( input.eof() ) {
				break;
			}
			switch ( wcNext ) {
				case L'\r':
				case L'\n':
					if ( token == TOK_UNDEFINED ) {
						input.get();
						readNewLine( input, wcNext );
						token = TOK_NEW_LINE;
					}
					break;
				case L'<':
					m_tokenBuffer.put( wcNext );
					token = getLeToken( input );
					break;
				default:
					token = TOK_OTHER;
					m_tokenBuffer.put( wcNext );
					break;
			}
		}
	}
	return token;
}

/**
** @brief Check token introduced with less operator ('<').
**
** 
*/
Token HtmlScanner::getLeToken( std::wistream& input )
{
	Token token = TOK_BLOCK_COMMENT;

	int index = 1;
	do {
		wchar_t wc = wchar_t(input.get());

		if ( input.eof() ) {
			token = TOK_OTHER;
			break;
		}
		m_tokenBuffer.put( wc );
		if ( wc == HtmlScanner::m_szBlockCommentStart[index] ) {
			++index;
		} else {
			token = TOK_OTHER;
			break;
		}
	} while ( HtmlScanner::m_szBlockCommentStart[index] != L'\0' );
	if ( token == TOK_BLOCK_COMMENT ) {
		continueBlockComment( input, m_szBlockCommentEnd, false );
	}
	return token;
}


} // namespace

#include "stdafx.h"
#include "Exceptions.h"
#include "Options.h"
#include "Context.h"
#include "Token.h"

namespace sqtpp {

// --------------------------------------------------------------------
// TokenInfo
// --------------------------------------------------------------------


/**
** Some informations about all defined scanner tokens.
*/
const TokenInfo TokenInfo::m_tokenInfo[] = {
	{ TOK_UNDEFINED,          L"TOK_UNDEFINED", L"Undefined / Unknown" },
	{ TOK_LINE_COMMENT,       L"TOK_LINE_COMMENT", L"Line comment // or -- (SQL)" },
	{ TOK_BLOCK_COMMENT,      L"TOK_BLOCK_COMMENT", L"Block comment /*...*/" },
	{ TOK_ORACLE_HINT,        L"TOK_ORACLE_HINT", L"Oracle optimiser hint: /*+...*/" },
	{ TOK_SHARP,              L"TOK_SHARP", L"#" },
	{ TOK_SHARP_SHARP,        L"TOK_SHARP_SHARP", L"##" },
	{ TOK_SHARP_AT,           L"TOK_SHARP_AT", L"#@" },
	{ TOK_DIRECTIVE,          L"TOK_DIRECTIVE", L"^:space:*#:space:*:alnum:+" },
	{ TOK_DIR_DEFINE,         L"TOK_DIR_DEFINE", L"#define" },
	{ TOK_DIR_UNDEF,          L"TOK_DIR_UNDEF", L"#undef" },
	{ TOK_DIR_UNDEFALL,       L"TOK_DIR_UNDEFALL", L"#undefall" },
	{ TOK_DIR_IF,             L"TOK_DIR_IF", L"#if" },
	{ TOK_DIR_IFDEF,          L"TOK_DIR_IFDEF", L"#ifdef" },
	{ TOK_DIR_IFNDEF,         L"TOK_DIR_IFNDEF", L"#ifndef" },
	{ TOK_DIR_ELIF,           L"TOK_DIR_ELIF", L"#elif" },
	{ TOK_DIR_ELSE,           L"TOK_DIR_ELSE", L"#else" },
	{ TOK_DIR_ENDIF,          L"TOK_DIR_ENDIF", L"#endif" },
	{ TOK_DIR_INCLUDE,        L"TOK_DIR_INCLUDE", L"#include" },
	{ TOK_DIR_LINE,           L"TOK_DIR_LINE", L"#line" },
	{ TOK_DIR_ERROR,          L"TOK_DIR_ERROR", L"#error" },
	{ TOK_DIR_PRAGMA,         L"TOK_DIR_PRAGMA", L"#pragma" },
	{ TOK_DIR_IMPORT,         L"TOK_DIR_IMPORT", L"#import" },
	{ TOK_DIR_USING,          L"TOK_DIR_USING", L"#using" },
	{ TOK_DIR_MESSAGE,        L"TOK_DIR_MESSAGE", L"#message" },
	{ TOK_DIR_EXEC,           L"TOK_DIR_EXEC", L"#exec" },
	{ TOK_NEW_LINE,           L"TOK_NEW_LINE", L"\\n" },
	{ TOK_SPACE,              L"TOK_SPACE", L"Any white space character." },
	{ TOK_STRING,             L"TOK_STRING", L"\" or '" },
	{ TOK_SYS_INCLUDE,        L"TOK_SYS_INCLUDE", L"<.+>" },
	{ TOK_NUMBER,             L"TOK_NUMBER", L"[0x]:digit:+", false, false, 1 },
	{ TOK_IDENTIFIER,         L"TOK_IDENTIFIER", L"(_|:alpha:)+(_|:alnum:)*" },
	{ TOK_OP_COMMA,           L"TOK_OP_COMMA", L",", false, true, 16 },
	{ TOK_OP_ASSIGN,          L"TOK_OP_ASSIGN", L"=", false, false, 15 },
	{ TOK_OP_EQ,              L"TOK_OP_EQ", L"==", false, true, 8 },
	{ TOK_OP_NE,              L"TOK_OP_NE", L"!=", false, true, 8 },
	{ TOK_OP_LT,              L"TOK_OP_LT", L"<", false, true, 7 },
	{ TOK_OP_LE,              L"TOK_OP_LE", L"<=", false, true, 7 },
	{ TOK_OP_LSHIFT,          L"TOK_OP_LSHIFT", L"<<", false, true, 6 },
	{ TOK_OP_GT,              L"TOK_OP_GT", L">", false, true, 7 },
	{ TOK_OP_GE,              L"TOK_OP_GE", L">=", false, true, 7 },
	{ TOK_OP_RSHIFT,          L"TOK_OP_RSHIFT", L">>", false, true, 6 },
	{ TOK_OP_MINUS,           L"TOK_OP_MINUS", L"-", false, true, 5 },
	{ TOK_OP_UNARY_MINUS,     L"TOK_OP_UNARY_MINUS", L"-", true, false, 2 },
	{ TOK_OP_PLUS,            L"TOK_OP_PLUS", L"+", false, true, 5 },
	{ TOK_OP_UNARY_PLUS,      L"TOK_OP_UNARY_PLUS", L"+", true, false, 2 },
	{ TOK_OP_MULTIPLY,        L"TOK_OP_MULTIPLY", L"*", false, true, 4 },
	{ TOK_OP_POWER,           L"TOK_OP_POWER", L"**", false, true, 4 },
	{ TOK_OP_DIVIDE,          L"TOK_OP_DIVIDE", L"/", false, true, 4 },
	{ TOK_OP_MODULUS,         L"TOK_OP_MODULUS", L"%", false, true, 4 },
	{ TOK_OP_LOGICAL_NOT,     L"TOK_OP_LOGICAL_NOT", L"!", true, false, 2 },
	{ TOK_OP_LOGICAL_AND,     L"TOK_OP_LOGICAL_AND", L"&&", false, true, 12 },
	{ TOK_OP_LOGICAL_OR,      L"TOK_OP_LOGICAL_OR", L"||", false, true, 14 },
	{ TOK_OP_LOGICAL_XOR,     L"TOK_OP_LOGICAL_XOR", L"^^", false, true, 13 },
	{ TOK_OP_BIT_AND,         L"TOK_OP_BIT_AND", L"&", false, true, 9 },
	{ TOK_OP_BIT_OR,          L"TOK_OP_BIT_OR", L"|", false, true, 11 },
	{ TOK_OP_BIT_XOR,         L"TOK_OP_BIT_XOR", L"^", false, true, 10 },
	{ TOK_OP_BIT_NOT,         L"TOK_OP_BIT_NOT", L"~", true, false, 2 },
	{ TOK_OP_DEFINED,         L"TOK_DEFINED", L"defined", true, false, 2 },
	{ TOK_LEFT_PARENTHESIS,   L"TOK_LEFT_PARENTHESIS", L"(", false, false, 1 },
	{ TOK_RIGHT_PARENTHESIS,  L"TOK_RIGHT_PARENTHESIS", L")", false, false, 1 },
	{ TOK_ADSALESNG_DIRECTIVE,L"TOK_ADSALESNG_DIRECTIVE", L"--[identifier]" },
	{ TOK_OTHER,              L"TOK_OTHER", L"Any stuff." },
	{ TOK_END_OF_FILE,        L"TOK_END_OF_FILE", L"End of file / input stream." }
};


/**
** @brief Get reflection / debug information about the given token.
*/
const TokenInfo& TokenInfo::getTokenInfo( Token token )
{
	int count = sizeof( m_tokenInfo ) / sizeof( TokenInfo );
	for ( int i = 0; i < count; ++i ) {
		const TokenInfo& ti = m_tokenInfo[i];
		if ( ti.token == token ) {
			return ti;
		}
	}
	throw RuntimeError( "Token info not found." );
	//return 	m_tokenInfo[0];

}



// --------------------------------------------------------------------
// TokenExpression
// --------------------------------------------------------------------

/**
** @brief Default constructor. Initializes everything as undefined / empty.
*/
TokenExpression::TokenExpression()
: token( TOK_UNDEFINED )
, tokenId( 0 )
, context( CTX_UNDEFINED )
{
}

/**
** @brief Copy constructor.
*/
TokenExpression::TokenExpression( const TokenExpression& that )
: tokenId( that.tokenId )
, token( that.token )
, context( that.context )
, text( that.text )
, identifier( that.identifier )
{
}

/**
** @brief Assignment operation
*/
TokenExpression& TokenExpression::operator=( const TokenExpression& that )
{
	this->tokenId = that.tokenId;
	this->token = that.token;
	this->context = that.context;
	this->text = that.text;
	this->identifier = that.identifier;
	return *this;
}

/**
** @brief Initializing constructor.
*/
TokenExpression::TokenExpression( Token token, Context context, const wstring& text )
: tokenId( 0 )
, token( token )
, context( context )
, text( text )
, identifier( text )
{
}

/**
** @brief Initializing constructor.
*/
TokenExpression::TokenExpression( Token token, Context context, const wstring& text, const wstring& identifier )
: tokenId( 0 )
, token( token )
, context( context )
, text( text )
, identifier( identifier )
{
}


/**
** @brief Reset everything to be empty / undefined.
*/
void TokenExpression::clear()
{
	this->token   = TOK_UNDEFINED;
	this->context = CTX_UNDEFINED;
	this->tokenId = 0;
	this->text.clear();
	this->identifier.clear();
}


// --------------------------------------------------------------------
// TokenExpressions
// --------------------------------------------------------------------

/**
** @brief Constructor.
*/
TokenExpressions::TokenExpressions()
{
	reserve(16);
}


/**
** @brief Remove leading and trailing space and (optionally) comments.
*/
void TokenExpressions::trim( const bool bRemoveLineFeeds, const bool bRemoveBlockComments, const bool bRemoveLineComments, const bool bRemoveSqlComments  )
{
	for ( reverse_iterator it = this->rbegin(); it != rend(); it = this->rbegin() ) {
		const TokenExpression& token = *it;
		if ( token.token == TOK_SPACE ) {
			pop_back();
		} else if ( bRemoveLineFeeds && token.token == TOK_NEW_LINE ) {
			pop_back();
		} else if ( bRemoveBlockComments && token.token == TOK_BLOCK_COMMENT ) {
			pop_back();
		} else if ( bRemoveLineComments && token.token == TOK_LINE_COMMENT ) {
			pop_back();
		} else if ( bRemoveSqlComments && token.token == TOK_SQL_LINE_COMMENT ) {
			pop_back();
		} else {
			break;
		}
	}

	for ( iterator it = this->begin(); it != end(); it = this->begin() ) {
		const TokenExpression& token = *it;
		if ( token.token == TOK_SPACE ) {
			this->erase( it );
		} else if ( bRemoveLineFeeds && token.token == TOK_NEW_LINE ) {
			this->erase( it );
		} else if ( bRemoveBlockComments && token.token == TOK_BLOCK_COMMENT ) {
			this->erase( it );
		} else if ( bRemoveLineComments && token.token == TOK_LINE_COMMENT ) {
			this->erase( it );
		} else if ( bRemoveSqlComments && token.token == TOK_SQL_LINE_COMMENT ) {
			this->erase( it );
		} else {
			break;
		}
	}
}

/**
** @brief Stringize tokens (for \# and \#@ macro operators.
**
** @param delimiter The character to be used to delimit the string.
** @param escape    The character to be inserted when the delimiter
**        is encountered somewhere in the middle of the expression.
*/
const wstring TokenExpressions::stringize( const wchar_t delimiter, const wchar_t escape ) const
{
	wstringstream buffer;

	buffer << delimiter;

	for ( const_iterator it = this->begin(); it != this->end(); ++it ) {
		const TokenExpression& tokenExpression = *it;
		const wstring&         sTokenText      = tokenExpression.getText();

		for ( size_t i = 0; i < sTokenText.length(); ++i ) {
			wchar_t ch = sTokenText[i];
			if ( ch == delimiter ) {
				buffer << escape;
			}
			buffer << ch;
		}
	}
	buffer << delimiter;

	wstring result = buffer.str();
	return result;
}

} // namespace

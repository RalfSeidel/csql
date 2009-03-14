/**
** @file
** @author Ralf Seidel
** @brief Enumeration of Tokens returned by the scanner.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_TOKEN_H
#define SQTPP_TOKEN_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

enum Context;
class Options;
class Scanner;

/**
** @brief Tokens returned by the scanner.
*/
enum Token 
{
	TOK_UNDEFINED,             //!< Undefined / Unknown
	TOK_LINE_COMMENT,          //!< "//.*$" or "--.*$" (SQL)
	TOK_BLOCK_COMMENT,         //!< "/*"
	TOK_SQL_LINE_COMMENT,      //!< "--.*"
	TOK_ORACLE_HINT,           //!< "/*+"
	TOK_SHARP,                 //!< "#"
	TOK_SHARP_SHARP,           //!< "##"
	TOK_SHARP_AT,              //!< "#@"
	TOK_HELLIP,                //!< "..."
	TOK_DIRECTIVE,             //!< "^:space:*#:space:*:alnum:+:space:*"
	TOK_DIR_DEFINE,            //!< "#define"
	TOK_DIR_UNDEF,             //!< "#undef"
	TOK_DIR_UNDEFALL,          //!< "#undefall"
	TOK_DIR_IF,                //!< "#if"
	TOK_DIR_IFDEF,             //!< "#ifdef"
	TOK_DIR_IFNDEF,            //!< "#ifndef"
	TOK_DIR_ELIF,              //!< "#elif"
	TOK_DIR_ELSE,              //!< "#else"
	TOK_DIR_ENDIF,             //!< "#endif"
	TOK_DIR_INCLUDE,           //!< "#include"
	TOK_DIR_LINE,              //!< "#line"
	TOK_DIR_ERROR,             //!< "#error"
	TOK_DIR_PRAGMA,            //!< "#pragma"
	TOK_DIR_IMPORT,            //!< "#import"
	TOK_DIR_USING,             //!< "#using"
	TOK_DIR_MESSAGE,           //!< "#message"
	TOK_DIR_EXEC,              //!< "#exec"
	TOK_NEW_LINE,              //!< "\n"
	TOK_EOL_BACKSLASH,         //!< "\$"
	TOK_SPACE,                 //!< White space characters except new lines.
	TOK_STRING,                //!< """ or "'"
	TOK_SYS_INCLUDE,           //!< "<.*>". Only returned by the scanner in the #CTX_INCLUDE_DIRECTIVE context.
	TOK_NUMBER,                //!< Sequence of digits.
	TOK_IDENTIFIER,            //!< (_|:alpha:)+(_|:alnum:)*
	TOK_OP_COMMA,              //!< ","
	TOK_OP_ASSIGN,             //!< "="
	TOK_OP_EQ,                 //!< "=="
	TOK_OP_NE,                 //!< "!="
	TOK_OP_LT,                 //!< "<"
	TOK_OP_LE,                 //!< "<="
	TOK_OP_LSHIFT,             //!< "<<"
	TOK_OP_GT,                 //!< ">"
	TOK_OP_GE,                 //!< ">="
	TOK_OP_RSHIFT,             //!< ">>"
	TOK_OP_MINUS,              //!< "-"
	TOK_OP_UNARY_MINUS,        //!< "-" The scanner will never deliver this token - the expression parser 
	                           //!< will replace TOK_OP_MINUS with TOK_OP_UNARY_MINUS if it doesn't expect 
							   //!< a binary operator.
	TOK_OP_PLUS,               //!< "+"
	TOK_OP_UNARY_PLUS,         //!< "+" The scanner will never deliver this token - the expression parser 
	                           //!< will replace TOK_OP_PLUS with TOK_OP_UNARY_PLUS if it doesn't expect 
							   //!< a binary operator.
	TOK_OP_MULTIPLY,           //!< "*"
	TOK_OP_POWER,              //!< "*"
	TOK_OP_DIVIDE,             //!< "/"
	TOK_OP_MODULUS,            //!< "%"
	TOK_OP_LOGICAL_NOT,        //!< "!"
	TOK_OP_LOGICAL_AND,        //!< "&&"
	TOK_OP_LOGICAL_OR,         //!< "||"
	TOK_OP_LOGICAL_XOR,        //!< "^^"
	TOK_OP_BIT_AND,            //!< "&"
	TOK_OP_BIT_OR,             //!< "|"
	TOK_OP_BIT_XOR,            //!< "^"
	TOK_OP_BIT_NOT,            //!< "~"
	TOK_OP_DEFINED,            //!< "defined"
	TOK_LEFT_PARENTHESIS,      //!< "("
	TOK_RIGHT_PARENTHESIS,     //!< ")"
	TOK_ADSALESNG_DIRECTIVE,   //!< Very special token retured if tag of AdSales NG
	                           //!< (a product of one of our customers) is detected.
							   //!< --[identifier]
	TOK_OTHER,                 //!< Any other stuff.

	// Last defined token. Insert any new tokens before this one.
	// TokenTest assumes that this token is the last one.
	TOK_END_OF_FILE            //!< End of file / input stream.
};

/**
** @brief Token and token expression as found by the scanner.
*/
class TokenExpression
{
public:
	/// The token.
	Token   token;

	/// The index of the token in the token input stream.
	size_t  tokenId;

	/// The context of the scanner in which this token was retrieved.
	Context context;

	/// The text scanned.
	wstring text;

	/// The expression of the scanner.
	/// This field is somehow redundant. It only exists to make scanning and processing
	/// of preprocessor directives a bit easier.
	/// When the scanner encounters a preprocessor directive the token expression text 
	/// will hold the whole string (like '\#    ifdef'). The identifier will contain
	/// the directive identifier only (e.g 'ifdef').
	wstring identifier;


	// Default constructor.
	TokenExpression();

	// Initializing constructor.
	TokenExpression( Token token, Context context, const wstring& text );

	// Initializing constructor.
	TokenExpression( Token token, Context context, const wstring& text, const wstring& identifier );

	// Destructor
	~TokenExpression() {}

	// Reset everything to be empty / undefined.
	void clear();

	Token getToken() const throw() { return this->token; }
	Context getContext() const throw() { return this->context; }
	const wstring& getText() const throw() { return this->text; }
	const wstring& getIdentifier() const throw() { return this->identifier; }

};


/**
** @brief Collection of token expressions.
*/
class TokenExpressions : public std::vector<TokenExpression> 
{
public:
	// Remove leading and trailing space and (optionally) comments.
	void trim( const bool bRemoveLineFeeds, const bool bRemoveBlockComments, const bool bRemoveLineComments, const bool bRemoveSqlComments );

	// Stringize tokens (for \# and \#@ macro operators.
	const wstring stringize( const wchar_t delimiter, const wchar_t escape ) const;
};


/**
** @brief Debugging informations about the defined tokens.
*/
class TokenInfo 
{
private:
	static const TokenInfo m_tokenInfo[];

public:
	/// The token id.
	Token          token;
	/// The enumeration symbol.
	const wchar_t* pwcSymbol;
	/// A descriptiv text / name.
	const wchar_t* pwcDescription;
	/// If this token a unary operator (like ~a)?
	bool           m_isUnaryOperator; 
	/// Is this token a binary operator (like a + b)?
	bool           m_isBinaryOperator; 
	/// Operator precedence / priority.
	char           m_nPrecedence; 

	// Initialising constructor.
	//TokenInfo( Token token, const wchar_t* const pwcSymbol, const wchar_t* const pwcDescription );

	// Get reflection / debug information about the given token.
	static const TokenInfo& getTokenInfo( Token token );

	//! Check if the token is an operator token.
	bool isOperator() const throw()        { return this->m_isUnaryOperator || this->m_isBinaryOperator; }
	//! Check if the token is an unary operator token (+, -, !, ~)
	bool isUnaryOperator() const throw()   { return this->m_isUnaryOperator; }
	//! Check if the token is an binary operator token (e.g. +, -, *, /)
	bool isBinaryOperator() const throw()  { return this->m_isBinaryOperator; }

	char getOperatorPrecedence() const throw() { return this->m_nPrecedence; };

private:
	// Assignment operator (not implemented).
	TokenInfo& operator= ( const TokenInfo& that );
};

/**
** @brief Declaration of an abstract token stream.
**
** In general the lexical scanner is the token stream for the processor. 
** However for cases the scanner will be replaced with a token stream
** based on a collection of tokens which are the result of a macro
** expansion.
*/
class ITokenStream
{
public:
	/// Check if macro identifiers found should be expanded.
	/// Identifiers found by the scanner will be expanded. So the scanner
	/// will return true. If the token stream is an expanded macro the overriden
	/// implementation of this function will return false because recursive 
	/// macro expansion is not supported.
	virtual bool  expandMacros() const throw() = 0;

	/// Get next token from this stream.
	virtual Token getNextToken( wistream&, TokenExpression& tokenExpression ) = 0;
};

/**
** @brief a stack of token streams.
*/
class TokenStreamStack : public std::stack<ITokenStream*>
{
};



} // namespace

#endif // SQTPP_TOKEN_H

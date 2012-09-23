/**
** @file
** @author Ralf Seidel
** @brief Declaration of the lexical scanner (#sqtpp::Scanner).
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_SCANNER_H
#define SQTPP_SCANNER_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Token.h"

namespace sqtpp {
enum Context;
class Options;
class TokenInfo;
class TokenExpression;

/**
** @brief The lexical scanner - converts input charcters and strings into enumerated tokens.
**
** The lexical scanner reads wide character / unicode stl streams and translates 
** the found text into tokens.
*/
class Scanner : public ITokenStream
{
private:

	/// The start of a multi line comment
	static const wchar_t* const m_szBlockCommentStart;

	/// The start of a multi line comment
	static const wchar_t* const m_szBlockCommentEnd;

	/// All defined tokens.
	static const TokenInfo m_tokenInfo[];

	/// An empty string
	static const wstring m_emptyString;

	/// The current scanner context.
	Context                    m_context;

	/// If switch the scanner context the current context will be pushed onto this stack.
	std::stack<Context>        m_contextStack;

	/// The global options passed to the scanner constructor.
	const sqtpp::Options&      m_options;

	/// The first no space character in current scanning line.
	/// This variable is used to determine if the '#' symbol introduces
	/// a preprocessor directed i.e. if it is the first character in 
	/// the input line which is not a space.
	wchar_t                    m_wcFirstNonSpaceChar;

	/// The last non spaced character in current scanning line.
	/// This variable is used to determine if a pre processor directive
	/// exceeds one line i.e. if the last character in the previous
	/// line was a backslash.
	wchar_t                    m_wcLastNonSpaceChar;

	/// Token read by the previous call (for look back)
	TokenExpression&           m_lastToken;

	/// The identifier assoziated with the current token e.g. for #define its "define"
	std::wstring               m_tokenIdentifier;

protected:

	/// The read buffer.
	std::wstringstream         m_tokenBuffer;

private:
	// Not implement copy c'tor (to prevent copy).
	Scanner( const Scanner& that );
	// Not implement assignment operator (to prevent copy).
	Scanner& operator=( const Scanner& that );

public:
	// Default c'tor
	Scanner( const sqtpp::Options& options );
	// d'tor.
	virtual ~Scanner();

	// Static c'tor.
	static Scanner* createScanner( const Options& options );

	// Expand all macros found
	bool  expandMacros() const throw() { return true; }

	// Read the next characters from the input stream and translate them into scanner tokens.
	Token getNextToken( std::wistream& input, TokenExpression& tokenExpression );

	// Get the current scanner context.
	Context getContext() const throw() { return m_context; }

	// Change current context.
	void pushContext( Context newContext );

	// Restore previous context.
	void popContext( Context context );

	// Get the last token scanned.
	Token getLastToken() const throw();

	// Get the last character read.
	const wstring& getLastTokenText() const throw();

	// Get the last identifier read.
	const wstring& getLastTokenIdentifier() const throw();

	bool isSpace( wchar_t ch ) const;
	bool isNewLine( wchar_t ch ) const;
protected:

	// Implementation of getNextToken()
	virtual Token getNextTokenCore( std::wistream& input, size_t& nCharCountRead );

	// Determine token which is introduced with a dot ('.').
	Token getDotToken( std::wistream& input );
	// Determine token which is introduced with a forward slash ('/').
	Token getSlashToken( std::wistream& input );
	// Determine token which is introduced with a dash ('-').
	Token getDashToken( std::wistream& input );
	// Determine token which is introduced with a sharp symbol ('#').
	Token getSharpToken( std::wistream& input );
	// Determine if the backslash ('\\') is the last symbol in the current line.
	Token getBackSlashToken( std::wistream& input );
	// Determine the token introduced with a less than ('<') character.
	Token getLtToken( std::wistream& input );
	// Determine the token introduced with a greater than ('>') character.
	Token getGtToken( std::wistream& input );
	// Determine the token introduced with a equal ('=') character.
	Token getEqToken( std::wistream& input );
	// Determine the token introduced with a equal ('!') character.
	Token getNotToken( std::wistream& input );
	// Determine the token introduced with a equal ('&') character.
	Token getAndToken( std::wistream& input );
	// Determine the token introduced with a equal ('|') character.
	Token getOrToken( std::wistream& input );
	// Determine the token introduced with a caret ('^') character.
	Token getXorToken( std::wistream& input );
	// Scan next token in default mode.
	Token continueDefault( std::wistream& input );
	// Continue scanning a line comment.
	Token continueLineComment( std::wistream& input, Token lineCommentToken, bool bFollowup );
	// Continue scanning a block comment.
	Token continueBlockComment( std::wistream& input, const wchar_t* pszCommentEnd, bool bFollowup );
	// Continue conditional block.
	Token continueConditional( std::wistream& input );
	// Continue reading a (multiline) string.
	Token continueString( std::wistream& input, wchar_t delimiter );


	// Get a new line token from the input stream.
	void readNewLine( std::wistream& input, wchar_t wcCurrent );

	// Read all white space characters.
	void readSpace( std::wistream& input );


	void readIdentifier( std::wistream& input, std::wostream& output  );
	void readIdentifier( std::wistream& input );
	// Check for keywords.
	Token translateIdentifier( const wstring& identifier ) const;

	// Read a number.
	void readNumber( std::wistream& input, wchar_t wcCurrent );

	// Token scanned event handler.
	void onTokenScanned();
private:
	const std::locale& getLocale() const;
	bool isIdentifierBegin( wchar_t ch ) const;
	bool isIdentifierContinued( wchar_t ch ) const;

	bool    readIfEqual( std::wistream& input, wchar_t next );
	wchar_t readIfOneOf( std::wistream& input, const wchar_t* next );


};

} // namespace sqtpp

#endif // SQTPP_SCANNER_H

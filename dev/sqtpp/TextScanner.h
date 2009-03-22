/**
** @file
** @author Ralf Seidel
** @brief Declaration of the text and html scanner.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_TEXT_SCANNER_H
#define SQTPP_TEXT_SCANNER_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Scanner.h"

namespace sqtpp {

class Options;

/**
** @brief The lexical scanner for simple text processing.
**
** When processing text files (source Language == LNG_TEXT) the only options
** supported by sqtpp is to eliminate empty lines, leading and trailing blanks
** and to convert the code page. For example to convert Windows text files (line
** ending (CR & LF) to unix you can call sqtpp with the following command line 
** arguments:
** @code
** sqtpp /Lt  
** @endcode
**
** @todo Document command line arguments for text processing.
*/
class TextScanner : public Scanner
{
private:
	typedef sqtpp::Scanner base;

public:
	// The constructor.
	TextScanner( const Options& options ) : base( options ) {}

	// The destructor.
	~TextScanner() throw() {}

	// Simpler version of getNextToken
	virtual Token getNextTokenCore( std::wistream& input );
};


/**
** @brief The lexical scanner for XML/HTML text processing.
**
** HMTL and XML processing is quiet similar to text procssing. 
** The only additional options sqtpp supports is the removal
** of comments (<!-- Comment --> ).
**
** @todo Document command line arguments for HTML processing.
*/
class HtmlScanner : public Scanner
{
private:
	typedef sqtpp::Scanner base;

	// The start of a multi line comment
	static const wchar_t* const m_szBlockCommentStart;
	// The start of a multi line comment
	static const wchar_t* const m_szBlockCommentEnd;

public:
	// The constructor.
	HtmlScanner( const Options& options ) : base( options ) {}

	// The destructor.
	~HtmlScanner() throw() {}


	// Simpler version of getNextToken
	virtual Token getNextTokenCore( std::wistream& input );

private:
	// Check token introduced with less operator ('<').
	Token getLeToken( std::wistream& input  );
};



} // namespace

#endif // SQTPP_TEXT_SCANNER_H

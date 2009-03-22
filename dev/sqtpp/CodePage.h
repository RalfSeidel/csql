/**
** @file
** @author Ralf Seidel
** @brief Declaration of the input/output code pages.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_CODEPAGE_H
#define SQTPP_CODEPAGE_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

/**
** @brief Enumeratiom of supported code pages.
**
** @todo Define (and support) more code pages.
*/
enum CodePage {
	/// Undefined CodePage / initial value.
	CP_UNDEFINED           = 0,
	// OEM (US)
	CP_OEM_437             = 437,
	// OEM (Latin I)
	CP_OEM_850             = 850,
	/// Unicode/UTF16
	CP_UTF16               = 1200,
	/// Unicode/UTF16 Big Endian
	CP_UTF16BE             = 1201,
	/// Windows 1252
	CP_WINDOWS1252         = 1252,
	// Unicode UTF-7
	CP_UTF7                = 65000,
	// Unicode UTF-8 
	CP_UTF8                = 65001,
	/// UTF32 (not supported yet).
	CP_UTF32               = 65005,
	/// UTF32 Bug Endian (not supported yet).
	CP_UTF32BE             = 65006
};

/**
** @brief Debugging / Reflection infos about a code page.
*/
class CodePageInfo
{
private:
	/// All CodePages.
	static const CodePageInfo* m_codePages[];

	static const CodePageInfo Undefined;
	static const CodePageInfo OEM437;
	static const CodePageInfo OEM850;
	static const CodePageInfo UTF7;
	static const CodePageInfo UTF8;
	static const CodePageInfo UTF16;
	static const CodePageInfo UTF16BE;
	static const CodePageInfo Windows1252;

	/// The id of the CodePage.
	CodePage      m_codePage;

	/// The CodePage identifier.
	const wstring m_identifier;


private:
	// Copy c'tor (not implemented)
	CodePageInfo( const CodePageInfo& that );
	// Assignment operator (not implemented)
	CodePageInfo& operator=( const CodePageInfo& that );
public:
	// Initializing constructor.
	CodePageInfo( CodePage CodePage, const wchar_t* text );
	// Destructor.
	~CodePageInfo() throw();

	// Get all supported code pages.
	static const CodePageInfo** getCodePages() throw();

	// Get CodePage of given identifier.
	static const CodePageInfo* findCodePageInfo( CodePage cp ) throw();

	// Get CodePage of given identifier.
	static const CodePageInfo& getCodePageInfo( CodePage cp ) throw();

	// Get the CodePage id.
	CodePage getCodePage() const throw() { return m_codePage; }

	// Get the CodePage identifier.
	const wstring& getIdentifier() const throw() { return m_identifier; }
};


} // namespace

#endif // SQTPP_CODEPAGE_H

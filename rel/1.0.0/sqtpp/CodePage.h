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
enum CodePageId {
	/// Undefined code page / initial value.
	CPID_UNDEFINED           = 0,
	// OEM (US)
	CPID_OEM_437             = 437,
	// OEM (Latin I)
	CPID_OEM_850             = 850,
	/// Unicode/UTF16
	CPID_UTF16               = 1200,
	/// Unicode/UTF16 Big Endian
	CPID_UTF16BE             = 1201,
	/// Windows 1252
	CPID_WINDOWS_1252        = 1252,
	// Unicode UTF-7
	CPID_UTF7                = 65000,
	// Unicode UTF-8 
	CPID_UTF8                = 65001,
	/// UTF32 (not supported yet).
	CPID_UTF32               = 65005,
	/// UTF32 Big Endian (not supported yet).
	CPID_UTF32BE             = 65006
};

/**
** @brief Debugging / Reflection infos about a code page.
*/
class CodePageInfo
{
private:
	/// All CodePages.
	static const CodePageInfo* m_codePages[];

	static const CodePageInfo& Undefined;
	static const CodePageInfo& OEM437;
	static const CodePageInfo& OEM850;
	static const CodePageInfo& Windows1252;
	static const CodePageInfo& UTF7;
	static const CodePageInfo& UTF8;
	static const CodePageInfo& UTF16;
	static const CodePageInfo& UTF16BE;

	/// @brief The id of the code page.
	CodePageId m_codePageId;

	/// @brief The CodePageId identifier.
	const wstring m_identifier;

private:
	// Copy c'tor (not implemented)
	CodePageInfo( const CodePageInfo& that );
	// Assignment operator (not implemented)
	CodePageInfo& operator=( const CodePageInfo& that );
public:
	/// @brief Initializing constructor.
	CodePageInfo( CodePageId codePageId, const wchar_t* name );

	/// @brief Destructor.
	virtual ~CodePageInfo() throw();

	/// @brief Get all supported code pages.
	static const CodePageInfo** getCodePages() throw();

	/// Get the code page informations of the given identifier.
	static const CodePageInfo* findCodePageInfo( CodePageId cp ) throw();

	/// Get the code page informations of the given identifier.
	static const CodePageInfo& getCodePageInfo( CodePageId cp ) throw();

	/// Get the default code page id.
	static CodePageId getDefaultCodePageId() throw() { return CPID_WINDOWS_1252; }

	/// Get the code page id.
	CodePageId getCodePageId() const throw() { return m_codePageId; }

	/// Get the code page identifier.
	const wstring& getIdentifier() const throw() { return m_identifier; }

	/// Get the byte order mask character sequence that is used to identify the code page
	/// at the beginning of a file.
	/// @return <code>NULL</code> if no BOM is defined for the code page. The character
	/// sequence otherwise.
	virtual const char* getFileBom() const throw() = NULL;

	/// @brief Get the locale used to convert the characters set of the codepage from and to 
	/// the c++ unicode strings.
	virtual const locale& getLocale() const throw() = NULL;
};


} // namespace

#endif // SQTPP_CODEPAGE_H

#include "stdafx.h"
#include "CodePage.h"
#include "CodePageConverter.h"

namespace sqtpp {

namespace {

/**
** @brief Base class for informations of single byte encoding code pages
*/
class SbcsInfoBase : public CodePageInfo
{
private:
	const std::locale m_locale;

public:
	SbcsInfoBase( CodePageId codePageId, const wchar_t* text )
	: CodePageInfo( codePageId, text )
	, m_locale( getLocaleName( codePageId ).c_str() )
	{
	}

	/**
	** @return <code>NULL</code>
	*/
	const char* getFileBom() const throw()
	{
		return NULL;
	}

	virtual const locale& getLocale() const
	{
		return m_locale;
	}
private:
	static string getLocaleName( CodePageId codePageId )
	{
		if ( codePageId == CPID_UNDEFINED ) {
			return locale::classic().name();
		} else {
			stringstream cpBuffer;
			cpBuffer << "English_US." << int(codePageId);
			string locName = cpBuffer.str();
			return locName;
		}
	}
};

/**
** @brief Base class for the various unicode encodings.
*/
class UtfInfoBase : public CodePageInfo
{
private:
	/// @brief The byte order mask which identifies the code page at the
	/// beginning of a file.
	const char* const m_pFileBom;

	const std::locale m_locale;
public:
	UtfInfoBase( CodePageId codePageId, const wchar_t* text, const char* fileBom, CodePageConverter* pCodePageConverter )
	: CodePageInfo( codePageId, text )
	, m_pFileBom( fileBom )
	, m_locale( locale::classic(), pCodePageConverter )
	{
	}

	virtual const char* getFileBom() const throw()
	{
		return m_pFileBom;
	}

	virtual const locale& getLocale() const throw()
	{
		return m_locale;
	}
};

class UTF7Info : public UtfInfoBase
{

public:
	UTF7Info() 
	: UtfInfoBase( CPID_UTF7, L"UTF-7", "\x2B\x2F\x76", new Utf7Converter() )
	{
	}


};

class UTF8Info : public UtfInfoBase
{
public:
	UTF8Info() 
	: UtfInfoBase( CPID_UTF8, L"UTF-8", "\xEF\xBB\xBF", new Utf8Converter() )
	{
	}
};

class UTF16Info : public UtfInfoBase
{
private:
	const std::locale m_locale;

public:
	UTF16Info() 
	: UtfInfoBase( CPID_UTF16, L"UTF-16", "\xFF\xFE", new Utf16Converter() )
	{
	}
};


class UTF16BEInfo : public UtfInfoBase
{
public:
	UTF16BEInfo() 
	: UtfInfoBase( CPID_UTF16BE, L"UTF-16", "\xFE\xFF", new Utf16BeConverter() )
	{
	}
};
}



static const SbcsInfoBase cpUndefinedInfoInstance( CPID_UNDEFINED,  L"" );
static const SbcsInfoBase cpOem437InfoInstance( CPID_OEM_437,  L"OEM-437" );
static const SbcsInfoBase cpOem850InfoInstance( CPID_OEM_850,  L"OEM-850" );
static const SbcsInfoBase cpWindows1252InfoInstance( CPID_WINDOWS_1252,  L"WINDOWS-1252" );
static const UTF7Info cpUTF7InfoInstance;
static const UTF8Info cpUTF8InfoInstance;
static const UTF16Info cpUTF16InfoInstance;
static const UTF16BEInfo cpUTF16BEInfoInstance;


const CodePageInfo& CodePageInfo::Undefined( cpUndefinedInfoInstance );
const CodePageInfo& CodePageInfo::OEM437( cpOem437InfoInstance );
const CodePageInfo& CodePageInfo::OEM850( cpOem850InfoInstance );
const CodePageInfo& CodePageInfo::Windows1252 = cpWindows1252InfoInstance;
const CodePageInfo& CodePageInfo::UTF7 = cpUTF7InfoInstance;
const CodePageInfo& CodePageInfo::UTF8 = cpUTF8InfoInstance;
const CodePageInfo& CodePageInfo::UTF16 = cpUTF16InfoInstance;
const CodePageInfo& CodePageInfo::UTF16BE = cpUTF16BEInfoInstance;

/**
** @brief Array of all supported code pages.
*/
const CodePageInfo* CodePageInfo::m_codePages[] = {
	&CodePageInfo::Undefined,
	&CodePageInfo::OEM437,
	&CodePageInfo::OEM850,
	&CodePageInfo::Windows1252,
	&CodePageInfo::UTF16,
	&CodePageInfo::UTF16BE,
	&CodePageInfo::UTF7,
	&CodePageInfo::UTF8,
	NULL
};


/**
** @brief Initializing constructor.
** @param codePageId the code page id.
** @param text The name of the code page.
** @param fileBom For unicode formats: the lead in bytes in files used to define the encoding.
*/
CodePageInfo::CodePageInfo( CodePageId codePageId, const wchar_t* text )
: m_codePageId( codePageId )
, m_identifier( text )
{
}

CodePageInfo::~CodePageInfo() throw()
{
}


/**
** @brief Get all supported code pages.
**
** @return Array of all supported code pages. The last element 
** in the pointer array will be null.
*/
const CodePageInfo** CodePageInfo::getCodePages() throw()
{
	return m_codePages;
}

/**
** @brief Find information about given code page.
**
** @return Codepage information if cp was found. NULL if not.
*/
const CodePageInfo* CodePageInfo::findCodePageInfo( CodePageId cp ) throw()
{
	const CodePageInfo* const* ppInfo = m_codePages;

	while ( *ppInfo != NULL ) {
		const CodePageInfo& info = **ppInfo;
		if ( info.getCodePageId() == cp ) {
			return *ppInfo;
		}
		++ppInfo;
	}

	return NULL;
}

/**
** @brief Get information about given code page.
**
** @return Codepage information if cp was found. CodePageInfo::Undefined if not.
*/
const CodePageInfo& CodePageInfo::getCodePageInfo( CodePageId cp ) throw()
{
	const CodePageInfo* pInfo = findCodePageInfo( cp );
	if ( pInfo == NULL ) {
		return CodePageInfo::Undefined;
	} else {
		return *pInfo;
	}
}



} // namespace

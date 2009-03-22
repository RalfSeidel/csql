#include "stdafx.h"
#include "CodePage.h"

namespace sqtpp {

const CodePageInfo CodePageInfo::Undefined( CP_UNDEFINED,  L"" );
const CodePageInfo CodePageInfo::OEM437( CP_OEM_437, L"OEM-437" );
const CodePageInfo CodePageInfo::OEM850( CP_OEM_850, L"OEM-850" );
const CodePageInfo CodePageInfo::UTF7( CP_UTF7, L"UTF-7" );
const CodePageInfo CodePageInfo::UTF8( CP_UTF8, L"UTF-8" );
const CodePageInfo CodePageInfo::UTF16( CP_UTF16, L"UTF-16" );
const CodePageInfo CodePageInfo::UTF16BE( CP_UTF16BE, L"UTF-16FEFF" );
const CodePageInfo CodePageInfo::Windows1252( CP_WINDOWS1252,  L"WINDOWS-1252" );

/**
** @brief Array of all supported code pages.
*/
const CodePageInfo* CodePageInfo::m_codePages[] = {
	&CodePageInfo::Undefined,
	&CodePageInfo::OEM437,
	&CodePageInfo::OEM850,
	&CodePageInfo::UTF16,
	&CodePageInfo::UTF16BE,
	&CodePageInfo::Windows1252,
	NULL
};


/**
** Initializing constructor.
*/
CodePageInfo::CodePageInfo( CodePage codePage, const wchar_t* text )
: m_codePage( codePage )
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
const CodePageInfo* CodePageInfo::findCodePageInfo( CodePage cp ) throw()
{
	const CodePageInfo* const* ppInfo = m_codePages;

	while ( *ppInfo != NULL ) {
		const CodePageInfo& info = **ppInfo;
		if ( info.getCodePage() == cp ) {
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
const CodePageInfo& CodePageInfo::getCodePageInfo( CodePage cp ) throw()
{
	const CodePageInfo* pInfo = findCodePageInfo( cp );
	if ( pInfo == NULL ) {
		return CodePageInfo::Undefined;
	} else {
		return *pInfo;
	}
}



} // namespace

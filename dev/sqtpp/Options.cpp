#include "stdafx.h"
#include <cassert>
#include "Exceptions.h"
#include "Options.h"

namespace sqtpp {

Options::LanguageInfo::LanguageInfo( Language eLanguage, const wchar_t* pwszSymbol, const wchar_t* pwszMacro )
: language( eLanguage )
, pwszSymbol( pwszSymbol )
, pwszMacro( pwszMacro )
{
}

const wchar_t Options::m_wcDirectorySeperator = L';';

const Options::LanguageInfo Options::m_languageInfo[] = {
	Options::LanguageInfo( Options::LNG_UNDEFINED, L"LNG_UNDEFINED", NULL ),
	Options::LanguageInfo( Options::LNG_TEXT, L"LNG_TEXT", L"__SQTPP_TEXT" ),
	Options::LanguageInfo( Options::LNG_XML, L"LNG_XML", L"__SQTPP_XML"  ),
	Options::LanguageInfo( Options::LNG_C, L"LNG_C", L"__SQTPP_C" ),
	Options::LanguageInfo( Options::LNG_CPP, L"LNG_CPP", L"__cplusplus" ),
	Options::LanguageInfo( Options::LNG_ASM, L"LNG_ASM", L"__SQTPP_ASM"  ),
	Options::LanguageInfo( Options::LNG_RC, L"LNG_RC", L"__SQTPP_RC"  ),
	Options::LanguageInfo( Options::LNG_SQL, L"LNG_SQL", L"__SQTPP_SQL"  ),
};

/*
** @brief The constructor.
*/
Options::Options()
{
	m_eLanguage                = Options::LNG_SQL;
	m_eQuoting                 = Options::QUOT_ESCAPE;
	m_eStringDelimiter         = Options::STRD_DOUBLE;
	m_eNewLineOutput           = Options::NLO_OS_DEFAULT;
	m_bEmitLine                = true;
	m_bKeepBlockComments       = false;
	m_bKeepLineComments        = false;
	m_bKeepSqlComments         = false;
	m_bMultiLineMacroExpansion = false;
	m_bMultiLineStringLiterals = false;
	m_bExpandMacroArguments    = false;
	m_bTrimLeadingBlanks       = false;
	m_bTrimTrailingBlanks      = false;
	m_bEliminateEmptyLines     = false;
	m_bIgnoreCWD               = false;
	m_bUndefAllBuildin         = false;
	m_bSupportAdSalesNG        = true;
	m_bVerbose                 = false;
	m_nInputCodePage           = 1252;
	m_nOutputCodePage          = 1252;

	setLanguageDefaults();
}

/*
** @brief The destructor.
*/
Options::~Options() throw()
{
	const wchar_t* pwzsEnvironment = _wgetenv( L"INCLUDE" );

	if ( pwzsEnvironment != NULL ) {
		addIncludeDirectories( pwzsEnvironment );
	}
}

/**
** @brief Get some (debug) information about the given source code language.
**
** @returns Language infos.
*/
const Options::LanguageInfo& Options::getLanguageInfo( sqtpp::Options::Language language )
{
	const int count = sizeof( m_languageInfo ) / sizeof( LanguageInfo );
	for ( int i = 0; i < count; ++i ) {
		const Options::LanguageInfo& info = m_languageInfo[i];
		if ( info.language == language ) {
			return info;
		}
	}
	// Language not found
	assert( false );
	return m_languageInfo[0];
}

/**
** @brief Get the date format string.
**
** @returns Format string for wcsftime.
*/
const std::wstring Options::getDateFormat() const
{
	wstring sDateFormat = m_sDateFormat;
	
	if ( sDateFormat.empty() ) {
		wstring delimiter;
		switch ( getLanguage() ) {
			case LNG_SQL:
				sDateFormat = L"%Y%m%d";
				break;
			case LNG_XML:
				sDateFormat = L"%Y-%m-%d";
				break;
			default:
				sDateFormat = L"%b %d %Y";
				break;
		}
		switch ( getStringDelimiter() ) {
			case STRD_DOUBLE:
				delimiter = L"\"";
				break;
			case STRD_SINGLE:
				delimiter = L"\'";
				break;
			default:
				throw UnexpectedSwitchError();
		}
		sDateFormat = delimiter + sDateFormat + delimiter;
	}
	return sDateFormat;
}

/**
** @brief Get the time format string.
**
** @returns Format string for wcsftime.
*/
const std::wstring Options::getTimeFormat() const
{ 
	wstring sTimeFormat = m_sTimeFormat;
	
	if ( sTimeFormat.empty() ) {
		wstring delimiter;
		switch ( getLanguage() ) {
			case LNG_SQL:
				sTimeFormat = L"%H:%M:%S";;
				break;
			default:
				sTimeFormat = L"%H:%M:%S";;
				break;
		}
		switch ( getStringDelimiter() ) {
			case STRD_DOUBLE:
				delimiter = L"\"";
				break;
			case STRD_SINGLE:
				delimiter = L"\'";
				break;
			default:
				throw UnexpectedSwitchError();
		}
		sTimeFormat = delimiter + sTimeFormat + delimiter;
	}
	return sTimeFormat;
}


/**
** @brief Get the time format string.
**
** @returns Format string for wcsftime.
*/
const std::wstring Options::getTimestampFormat() const
{ 
	wstring sTimestampFormat = m_sTimestampFormat;
	
	if ( sTimestampFormat.empty() ) {
		wstring delimiter;
		switch ( getLanguage() ) {
			case LNG_SQL:
				sTimestampFormat = L"%Y%m%d %H:%M:%S";;
				break;
			case LNG_XML:
				sTimestampFormat = L"%Y%m%dT%H:%M:%S";;
				break;
			default:
				sTimestampFormat = L"%H:%M:%S";;
				break;
		}
		switch ( getStringDelimiter() ) {
			case STRD_DOUBLE:
				delimiter = L"\"";
				break;
			case STRD_SINGLE:
				delimiter = L"\'";
				break;
			default:
				throw UnexpectedSwitchError();
		}
		sTimestampFormat = delimiter + sTimestampFormat + delimiter;
	}
	return sTimestampFormat;
}

/*
** @brief Switch all comment options on or off.
*/
void Options::keepComments( bool bKeep ) throw()
{
	keepBlockComments( bKeep );
	keepLineComments( bKeep );
	keepSqlComments( bKeep );
}

/*
** @brief Add one or more included directories.
*/
void Options::addIncludeDirectories( const wchar_t* pwszIncludeDirectories )
{
	if ( pwszIncludeDirectories != NULL ) {
		const wchar_t* pchStart = pwszIncludeDirectories;
		const wchar_t* pchScan  = pchStart;

		for (;;) {
			if ( *pchScan == Options::m_wcDirectorySeperator || *pchScan == '\0' ) {
				while ( pchStart < pchScan && std::isspace( *pchStart, locale::classic() ) ) 
					++pchStart;
				const wchar_t* pchEnd = pchScan - 1;
				while ( pchEnd >= pchStart && std::isspace( *pchEnd, locale::classic() ) ) 
					--pchEnd;

				if ( pchStart < pchEnd ) {
					wstring sIncludeDirectory( pchStart, pchEnd - pchStart + 1 );
					m_includeDirectories.push_back( sIncludeDirectory );
				}
				if ( *pchScan == L'\0' )
					break;
			}
			++pchScan;
		} 
	}
}

/*
** @brief Get default options.
*/
const Options& Options::getDefaultOptions() throw()
{
	static const Options defaultOptions;
	return defaultOptions;
}



/*
** @brief Set default options for the defined input language.
*/
void Options::setLanguageDefaults()
{
	switch ( m_eLanguage ) {
		case Options::LNG_TEXT:
		case Options::LNG_XML:
			m_eQuoting                 = Options::QUOT_UNDEFINED;
			m_eStringDelimiter         = Options::STRD_UNDEFINED;
			m_bEmitLine                = false;
			m_bKeepBlockComments       = true;
			m_bKeepLineComments        = true;
			m_bKeepSqlComments         = true;
			m_bMultiLineMacroExpansion = false;
			m_bMultiLineStringLiterals = true;
			m_bExpandMacroArguments    = false;
			m_bTrimLeadingBlanks       = false;
			m_bTrimTrailingBlanks      = false;
			m_bEliminateEmptyLines     = false;
			break;
			//break;
		case Options::LNG_C:
		case Options::LNG_CPP:
			m_eQuoting                 = Options::QUOT_ESCAPE;
			m_eStringDelimiter         = Options::STRD_DOUBLE;
			m_bEmitLine                = true;
			m_bKeepBlockComments       = false;
			m_bKeepLineComments        = false;
			m_bKeepSqlComments         = true;
			m_bMultiLineMacroExpansion = false;
			m_bMultiLineStringLiterals = false;
			m_bExpandMacroArguments    = false;
			m_bTrimLeadingBlanks       = false;
			m_bTrimTrailingBlanks      = false;
			m_bEliminateEmptyLines     = false;
			break;
		case Options::LNG_RC:
			throw UnexpectedSwitchError();
			//break;
		case Options::LNG_ASM:
			throw UnexpectedSwitchError();
			//break;
		case Options::LNG_SQL:
			m_eQuoting                 = Options::QUOT_DOUBLE;
			m_eStringDelimiter         = Options::STRD_SINGLE;
			m_bEmitLine                = false;
			m_bKeepBlockComments       = false;
			m_bKeepLineComments        = false;
			m_bKeepSqlComments         = true;
			m_bMultiLineMacroExpansion = true;
			m_bMultiLineStringLiterals = false;
			m_bExpandMacroArguments    = false;
			m_bTrimLeadingBlanks       = false;
			m_bTrimTrailingBlanks      = false;
			m_bEliminateEmptyLines     = false;
			break;
			//break;
		default:
			throw UnexpectedSwitchError();
	}
}


} // namespace

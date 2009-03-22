#include "StdAfx.h"
#include "Location.h"
#include "File.h"
#include "Error.h"

/**
** @namespace sqtpp::error
** 
** @brief Errors generated while processing the input.
*/

namespace sqtpp {
namespace error {

/**
** @brief Get the string expression of the given error severity.
**
** 
*/
const wchar_t* Error::getSeverityString( Error::Severity severity ) throw()
{
	switch ( severity ) {
		case SEV_UNDEFINED:
			return L"undefined";
		case SEV_DEBUG_VERBOSE:
			return L"debug_verbose";
		case SEV_DEBUG:
			return L"debug";
		case SEV_INFO_VERBOSE:
			return L"info_verbose";
		case SEV_INFO:
			return L"info";
		case SEV_WARNING_L9:
			return L"warning_l9";
		case SEV_WARNING_L8:
			return L"warning_l8";
		case SEV_WARNING_L7:
			return L"warning_l7";
		case SEV_WARNING_L6:
			return L"warning_l6";
		case SEV_WARNING_L5:
			return L"warning_l5";
		case SEV_WARNING_L4:
			return L"warning_l4";
		case SEV_WARNING_L3:
			return L"warning_l3";
		case SEV_WARNING_L2:
			return L"warning_l2";
		case SEV_WARNING_L1:
			return L"warning_l1";
		case SEV_WARNING_L0:
			return L"warning_l0";
		case SEV_ERROR:
			return L"error";
		case SEV_FATAL:
			return L"fatal";
		default:
			return L"unknown";
	}
}


/**
** @brief Implementation of std::exception::what().
** 
*/
const char* Error::what() const
{
	return "Preprocessor error";
}

/**
** @brief Set information about current file in the error object.
**
** @param file The current file of the processor. 
*/
void Error::setFileInfo( const File& file )
{
	wstring path = file.getPath();

	if ( path.empty() ) {
		wstringstream stringBuilder;
		stringBuilder << L"::stream:" << file.getInstanceId();
		path = stringBuilder.str();
	} 

	setFilePath( path );
	setLine( file.getLine() );
	setColumn( file.getColumn() );
}


/**
** @brief Expand internal message with parameter.
**
** @param sParameter1 The substitution for {1}
** @param sParameter2 The substitution for {2}
*/
void Error::formatMessage( const wstring& sParameter1, const wstring& sParameter2 )
{
	size_t  parampos = m_sText.find( L"{1}" );
	size_t  prevpos  = 0;
	if ( parampos != wstring::npos ) {
		wstringstream sb;
		do {
			sb << m_sText.substr( prevpos, parampos );
			sb << sParameter1;

			prevpos  = parampos + wcslen( L"{1}" );
			parampos = m_sText.find( L"{1}", prevpos );
		} while ( parampos != wstring::npos );
		sb << m_sText.substr( prevpos );
		m_sText = sb.str();
	}

	parampos = m_sText.find( L"{2}" );
	prevpos  = 0;
	if ( parampos != wstring::npos ) {
		wstringstream sb;
		do {
			sb << m_sText.substr( prevpos, parampos );
			sb << sParameter2;

			prevpos  = parampos + wcslen( L"{2}" );
			parampos = m_sText.find( L"{2}", prevpos );
		} while ( parampos != wstring::npos );
		sb << m_sText.substr( prevpos );
		m_sText = sb.str();
	}
}



/**
** @brief Message severity stream out.
*/
C1070::C1070( const Location* pLocation ) : FatalError( L"C1070", L"mismatched #if/#endif pair in file {1}({2})" )
{
	if ( pLocation != NULL ) {
		 wstringstream linoNoBuilder;
		 linoNoBuilder << pLocation->getLine();
		 wstring lineNo = linoNoBuilder.str();
		 formatMessage( pLocation->getFile(), lineNo );
	}
}




} // namespace error
} // namespace sqtpp


/**
** @brief Message severity stream out.
*/
std::wostream& operator<<(std::wostream& os, const sqtpp::error::Error::Severity& severity )
{
	os << sqtpp::error::Error::getSeverityString( severity );
	return os;
}


/**
** @brief Message stream out operator.
*/
std::wostream& operator<<( std::wostream& os, const sqtpp::error::Error& error )
{
	os << error.getFilePath() << L'(' << error.getLine() << L"): " 
	   << error.getSeverity() << L' ' << error.getCode() << L": "
	   << error.getText() << endl;
	return os;
}

#include "stdafx.h"
#include "File.h"
#include "Location.h"


namespace sqtpp {

/**
** @brief Copy location information from given file.
*/
Location::Location( const File& file )
: m_nLine( file.getLine() )
, m_sFile( file.getPath() )
{
	if ( m_sFile.empty() ) {
		wstringstream stringBuilder;
		stringBuilder << L"::stream:" << file.getInstanceId();
		m_sFile = stringBuilder.str();
	} 
}



} // namespace

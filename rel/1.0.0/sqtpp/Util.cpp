#include "stdafx.h"
#include "Exceptions.h"
#include "Util.h"

namespace sqtpp {


// --------------------------------------------------------------------
// Convert
// --------------------------------------------------------------------


/**
** @brief Convert a single unicode charater into a 8 Bit character
*/
char Convert::wc2ch( const wchar_t wc, const std::locale& loc )
{
	const wchar_t*  pwszInput     = &wc;
	const wchar_t*  pwszInputNext = NULL;
	size_t          nInputLen     = 1;
	mbstate_t       state         = 0;
	char            chResult      = L'\0';
	char*           pszAnsi       = &chResult;
	char*           pszAnsiNext   = NULL;

	const std::codecvt<wchar_t, char, mbstate_t>& decoder = use_facet<codecvt<wchar_t, char, mbstate_t> >(loc);

	int result = decoder.out( state
				            , pwszInput, &pwszInput[nInputLen], pwszInputNext
				            , pszAnsi,   &pszAnsi[nInputLen],   pszAnsiNext );

	if ( result == codecvt_base::error ) {
		throw RuntimeError( "Error converting character" );
	}

	return chResult;
}


/**
** @brief Convert 8 bit string to a unicode string.
*/
const std::wstring Convert::str2wcs( const char* str )
{
	wstringstream buffer;
	buffer << str;
	wstring result = buffer.str();
	return result;
}


/**
** @brief Convert unicode string to an 8 bit string.
**
** @todo Better error handling.
*/
const std::string Convert::wcs2str( const wchar_t* str )
{
	const wchar_t* pwszInput    = str;
	const wchar_t* pwszInputNext = NULL;
	const size_t   nInputLen    = wcslen( str );
	const size_t   nOutputLen   = nInputLen + 1;
	mbstate_t      state        = 0;
	char*          pszOutput     = new char[nOutputLen+1];
	char*          pszOutputNext = NULL;
	const std::codecvt<wchar_t, char, mbstate_t>& codec = use_facet<codecvt<wchar_t, char, mbstate_t> >( locale() );

	int result = codec.out( state
				          , pwszInput, &pwszInput[nInputLen], pwszInputNext
				          , pszOutput, &pszOutput[nInputLen], pszOutputNext );
	pszOutput[nInputLen] = 0;

	string s( pszOutput );
	delete[] pszOutput;

	switch ( result ) {
		case codecvt_base::ok:
			break;
		case codecvt_base::noconv:
			throw RuntimeError( "Error converting UTF to 8 Bit character string." );
			break;
		case codecvt_base::error:
			//throw RuntimeError( "Error converting UTF to 8 Bit character string." );
			s.clear();
			break;
		case codecvt_base::partial:
			//throw RuntimeError( "Error converting UTF to 8 Bit character string." );
			s.clear();
			break;
		default:
			assert( false );
			throw std::runtime_error( string("Unexpected case value: %d", errno ) );
	}



	return s;
}

// --------------------------------------------------------------------
// Util
// --------------------------------------------------------------------

/**
** @brief Get current local date/time.
*/
void Util::getLocalTime( tm& localTime )
{
	time_t currentTime;
	// Get time as long integer.
	time( &currentTime );
	// Convert to local time
#	if _MSC_VER >= 1400
		errno_t  error = _localtime64_s( &localTime, &currentTime ); 
		if ( error != 0 ) {
			throw RuntimeError( "Error converting date/time." );
		}
#	else 
		const tm* localTimePtr;
		localTimePtr = localtime( &currentTime ); 
		localTime    = *localTimePtr;
#	endif
}


/**
** @brief Remove leading and trailing blanks from a string.
** 
** @param str The string to trim.
*/
const wstring Util::trim( const wstring& str )
{
	wstring::size_type pos1 = str.find_first_not_of( L' ' );
	wstring::size_type pos2 = str.find_last_not_of( L' ' );

	if ( pos1 == wstring::npos )
		pos1 = 0;
	if ( pos2 == wstring::npos ) 
		pos2 = str.length() - 1;

	wstring trimed = str.substr( pos1, pos2 - pos1 + 1 );
	return trimed;
}

/**
** @brief Remove leading and trailing blanks from a string.
**
** @param str The string to trim.
** @param c The character to remove.
*/
const wstring Util::trim( const wstring& str, wchar_t c )
{
	wstring::size_type pos1 = str.find_first_not_of( c );
	wstring::size_type pos2 = str.find_last_not_of( c );

	if ( pos1 == wstring::npos )
		pos1 = 0;
	if ( pos2 == wstring::npos ) 
		pos2 = str.length() - 1;

	wstring trimed = str.substr( pos1, pos2 - pos1 + 1 );
	return trimed;
}

} // namespace sqtpp

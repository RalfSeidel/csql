/**
** @file
** @author Ralf Seidel
** @brief Implementate of the the custom code page converter used 
**        for converting wide character strings in the input and output files.
**
**
** © 2009 by Heinrich und Seidel GbR Wuppertal.
*/
#include "stdafx.h"
#include <Windows.h>
#include "CodePageConverter.h"
#include "Exceptions.h"

namespace sqtpp {

// --------------------------------------------------------------------
// SbcsConverter
// --------------------------------------------------------------------

/**
** @brief Constructor.
*/
SbcsConverter::SbcsConverter( unsigned int codePageId, size_t refCount )
: base( refCount ) 
, m_codePageId( codePageId )
{
}

/**
** @brief Destructor.
*/
SbcsConverter::~SbcsConverter()
{
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/in/">c++ documentation</a> for details.
*/
std::codecvt_base::result SbcsConverter::do_in( mbstate_t& state
, const char* pFrom , const char* pFromMax , const char*& pFromNext
, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );
	assert( pTo != NULL );
	assert( pToMax != NULL );
	assert( pToMax > pTo );

	pFromNext = pFrom;
	pToNext = pTo;

	const size_t toLength = pToMax - pTo;
	if ( toLength == 0 )
		return noconv;

	const size_t fromLength = do_length( state, pFrom, pFromMax, toLength );

	if ( fromLength == 0 ) {
		return pFrom == pFromMax ? noconv : partial;
	}

	// Set dwFlags (Windows Vista and later): 
	// The function does not drop illegal code points if the application does not set this flag.
	const DWORD dwFlags = MB_ERR_INVALID_CHARS;
	const unsigned int codePageId = getCodePageId();
	const int charCount = MultiByteToWideChar( codePageId, dwFlags, pFrom, fromLength, pTo, toLength ) ; 
	if ( charCount > 0 ) {
		pFromNext = pFrom + fromLength;
		pToNext   = pTo   + charCount;
		return pFromNext == pFromMax ? ok : partial;
	} else {
		return error;
	}
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/out/">c++ documentation</a> for details.
*/
std::codecvt_base::result SbcsConverter::do_out( mbstate_t& /* state */
 , const wchar_t* pFrom , const wchar_t* pFromMax , const wchar_t*& pFromNext 
 , char* pTo, char* pToMax, char*& pToNext ) const
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );
	assert( pTo != NULL );
	assert( pToMax != NULL );
	assert( pToMax > pTo );

	pFromNext = pFrom;
	pToNext = pTo;

	const size_t fromLength = pFromMax - pFrom;
	if ( fromLength == 0 ) {
		return noconv;
	}

	const size_t toLength = pToMax - pTo;
	if ( toLength == 0 ) {
		return partial;
	}

	// Set dwFlags (Windows Vista and later): 
	// dwFlags must be set to either 0 or WC_ERR_INVALID_CHARS. 
	const DWORD dwFlags = 0; // WC_ERR_INVALID_CHARS;
	const unsigned int codePageId = getCodePageId();
	const int charCount = WideCharToMultiByte( codePageId, dwFlags, pFrom, fromLength, pTo, toLength, NULL, NULL ) ; 
	if ( charCount > 0 ) {
		pFromNext = pFrom + fromLength;
		pToNext = pTo + charCount;
		return ok;
	} else {
		DWORD dwErrorCode = ::GetLastError();
		return dwErrorCode == ERROR_INSUFFICIENT_BUFFER ? partial : error;
	}
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/unshift/">c++ documentation</a> for details.
*/
std::codecvt_base::result SbcsConverter::do_unshift( mbstate_t& /* state */, char* pTo, char* pToMax, char*& pToNext ) const
{
    return noconv;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/length/">c++ documentation</a> for details.
*/
int SbcsConverter::do_length( const mbstate_t& /* state */, const char* pFrom, const char* pFromMax, size_t toLength ) const throw() 
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );

	const size_t fromCount = pFromMax - pFrom;
	return fromCount < toLength ? fromCount : toLength;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/always_noconv/">c++ documentation</a> for details.
**
** @return <code>false</code>. 
*/
bool SbcsConverter::do_always_noconv() const throw()
{
    return false;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/max_length/">c++ documentation</a> for details.
**
** @return <code>4</code>. 
*/
int SbcsConverter::do_max_length() const throw()
{
    return 1;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/encoding/">c++ documentation</a> for details.
**
** @return <code>1</code> because single byte characters sets have a fixed width.
*/
int SbcsConverter::do_encoding() const throw()
{
    return 1;
}



// --------------------------------------------------------------------
// Utf7Converter
// --------------------------------------------------------------------

/**
** @brief Constructor.
*/
Utf7Converter::Utf7Converter( size_t refCount /* = 0 */ )
: base( refCount ) 
{
}

/**
** @brief Destructor.
*/
Utf7Converter::~Utf7Converter()
{
}

/**
** @brief Check if the given character is the start byte of an UTF-7 
** character sequence.
*/
bool Utf7Converter::is_start_byte( char byte )
{
	bool result = byte == '+';
	return result;
}


/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/in/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf7Converter::do_in( mbstate_t& state
, const char* pFrom , const char* pFromMax , const char*& pFromNext
, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );
	assert( pTo != NULL );
	assert( pToMax != NULL );
	assert( pToMax > pTo );

	pFromNext = pFrom;
	pToNext = pTo;

	const size_t toLength = pToMax - pTo;
	if ( toLength == 0 )
		return noconv;

	const size_t fromLength = do_length( state, pFrom, pFromMax, toLength );

	if ( fromLength == 0 ) {
		return pFrom == pFromMax ? noconv : partial;
	}

	// Set dwFlags (Windows Vista and later): 
	// The function does not drop illegal code points if the application does not set this flag.
	const DWORD dwFlags = MB_ERR_INVALID_CHARS;
	const int charCount = MultiByteToWideChar( CP_UTF7, dwFlags, pFrom, fromLength, pTo, toLength ) ; 
	if ( charCount > 0 ) {
		pFromNext = pFrom + fromLength;
		pToNext   = pTo   + charCount;
		return pFromNext == pFromMax ? ok : partial;
	} else {
		return error;
	}
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/out/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf7Converter::do_out( mbstate_t& /* state */
 , const wchar_t* pFrom , const wchar_t* pFromMax , const wchar_t*& pFromNext 
 , char* pTo, char* pToMax, char*& pToNext ) const
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );
	assert( pTo != NULL );
	assert( pToMax != NULL );
	assert( pToMax > pTo );

	pFromNext = pFrom;
	pToNext = pTo;

	const size_t fromLength = pFromMax - pFrom;
	if ( fromLength == 0 ) {
		return noconv;
	}

	const size_t toLength = pToMax - pTo;
	if ( toLength == 0 ) {
		return partial;
	}

	// Set dwFlags (Windows Vista and later): 
	// dwFlags must be set to either 0 or WC_ERR_INVALID_CHARS. 
	const DWORD dwFlags = 0; // WC_ERR_INVALID_CHARS;
	const int charCount = WideCharToMultiByte( CP_UTF7, dwFlags, pFrom, fromLength, pTo, toLength, NULL, NULL ) ; 
	if ( charCount > 0 ) {
		pFromNext = pFrom + fromLength;
		pToNext = pTo + charCount;
		return ok;
	} else {
		DWORD dwErrorCode = ::GetLastError();
		return dwErrorCode == ERROR_INSUFFICIENT_BUFFER ? partial : error;
	}
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/unshift/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf7Converter::do_unshift( mbstate_t& /* state */, char* pTo, char* pToMax, char*& pToNext ) const
{
    return noconv;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/length/">c++ documentation</a> for details.
*/
int Utf7Converter::do_length( const mbstate_t& /* state */, const char* pFrom, const char* pFromMax, size_t toLength ) const throw()
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );

	const size_t fromLength = pFromMax - pFrom;
	const DWORD dwFlags = 0; // WC_ERR_INVALID_CHARS;
	const int charCount = MultiByteToWideChar( CP_UTF7, dwFlags, pFrom, fromLength, NULL, 0 ) ; 
	return charCount;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/always_noconv/">c++ documentation</a> for details.
**
** @return <code>false</code>. 
*/
bool Utf7Converter::do_always_noconv() const throw()
{
    return false;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/max_length/">c++ documentation</a> for details.
**
** @return <code>4</code>. 
*/
int Utf7Converter::do_max_length() const throw()
{
    return 4;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/encoding/">c++ documentation</a> for details.
**
** @return <code>0</code> because UTF7 characters have a variable width.
*/
int Utf7Converter::do_encoding() const throw()
{
    return 0;
}





// --------------------------------------------------------------------
// Utf8Converter
// --------------------------------------------------------------------

/**
** @brief Constructor.
*/
Utf8Converter::Utf8Converter( size_t refCount /* = 0 */ )
: base( refCount ) 
{
}

/**
** @brief Destructor.
*/
Utf8Converter::~Utf8Converter()
{
}

/**
** @brief Check if the given character is the start byte of an UTF-8 
** character sequence.
*/
bool Utf8Converter::is_start_byte( char byte )
{
	char mask = '\xC0'; // binary 11000000
	char test = byte & mask;
	bool result = test == mask;
	return result;
}

/**
** @brief Check if the given character is the start byte of an UTF-8 
** character sequence.
** @param start the first byte of an UTF-8 character sequence.
** @return The length of the whole sequence.
*/
int Utf8Converter::sequence_length( char start )
{
	char mask = '\xC0'; // binary 11000000
	char test = start & mask;
	if ( (test & mask) != mask )
		return 1;

	int result = 2;
	
	for ( mask = '\x20'; mask != 0 && (start & mask) != 0; mask>>= 1 ) {
		++result;
	}
	// Maximal length of an UTF-8 character sequence is 4. Maybe handle this case.
	if ( result > 4 ) {

	}
	return result;
}



/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/in/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf8Converter::do_in( mbstate_t& state
, const char* pFrom , const char* pFromMax , const char*& pFromNext
, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );
	assert( pTo != NULL );
	assert( pToMax != NULL );
	assert( pToMax > pTo );

	pFromNext = pFrom;
	pToNext = pTo;

	const size_t toLength = pToMax - pTo;
	if ( toLength == 0 )
		return noconv;

	const size_t fromLength = do_length( state, pFrom, pFromMax, toLength );

	if ( fromLength == 0 ) {
		return pFrom == pFromMax ? noconv : partial;
	}

	// Set dwFlags (Windows Vista and later): 
	// The function does not drop illegal code points if the application does not set this flag.
	const DWORD dwFlags = MB_ERR_INVALID_CHARS;
	const int charCount = MultiByteToWideChar( CP_UTF8, dwFlags, pFrom, fromLength, pTo, toLength ) ; 
	if ( charCount > 0 ) {
		pFromNext = pFrom + fromLength;
		pToNext   = pTo   + charCount;
		return pFromNext == pFromMax ? ok : partial;
	} else {
		return error;
	}
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/out/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf8Converter::do_out( mbstate_t& state
 , const wchar_t* pFrom , const wchar_t* pFromMax , const wchar_t*& pFromNext 
 , char* pTo, char* pToMax, char*& pToNext ) const
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );
	assert( pTo != NULL );
	assert( pToMax != NULL );
	assert( pToMax > pTo );

	pFromNext = pFrom;
	pToNext = pTo;

	const size_t fromLength = pFromMax - pFrom;
	if ( fromLength == 0 ) {
		return noconv;
	}

	const size_t toLength = pToMax - pTo;
	if ( toLength == 0 ) {
		return partial;
	}

	// Set dwFlags (Windows Vista and later): 
	// dwFlags must be set to either 0 or WC_ERR_INVALID_CHARS. 
	const DWORD dwFlags = 0; // WC_ERR_INVALID_CHARS;
	const int charCount = WideCharToMultiByte( CP_UTF8, dwFlags, pFrom, fromLength, pTo, toLength, NULL, NULL ) ; 
	if ( charCount > 0 ) {
		pFromNext = pFrom + fromLength;
		pToNext = pTo + charCount;
		return ok;
	} else {
		DWORD dwErrorCode = ::GetLastError();
		return dwErrorCode == ERROR_INSUFFICIENT_BUFFER ? partial : error;
	}
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/unshift/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf8Converter::do_unshift( mbstate_t& state, char* pTo, char* pToMax, char*& pToNext ) const
{
    return noconv;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/length/">c++ documentation</a> for details.
*/
int Utf8Converter::do_length( const mbstate_t& state , const char* pFrom, const char* pFromMax , size_t toLength ) const throw() 
{
	assert( pFrom != NULL );
	assert( pFromMax != NULL );
	assert( pFromMax > pFrom );

	const char* pFromNext = pFrom;
	int fromCount = 0;
	size_t toCount   = 0;
	while( pFromNext < pFromMax && toCount < toLength  ) {
		int sequenceLength = sequence_length( *pFromNext );
		pFromNext+= sequenceLength;
		if ( pFromNext > pFromMax ) 
			break;
		fromCount+= sequenceLength;
		++toCount;
	}
	return fromCount;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/always_noconv/">c++ documentation</a> for details.
**
** @return <code>false</code>. 
*/
bool Utf8Converter::do_always_noconv() const throw()
{
    return false;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/max_length/">c++ documentation</a> for details.
**
** @return <code>4</code>. 
*/
int Utf8Converter::do_max_length() const throw()
{
    return 4;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/encoding/">c++ documentation</a> for details.
**
** @return <code>0</code> because UTF8 characters have a variable width.
*/
int Utf8Converter::do_encoding() const throw()
{
    return 0;
}

// --------------------------------------------------------------------
// Utf16Converter
// --------------------------------------------------------------------

/**
** @brief Constructor.
*/
Utf16Converter::Utf16Converter( size_t refCount /* = 0 */ )
: base( refCount ) 
{
}

/**
** @brief Destructor.
*/
Utf16Converter::~Utf16Converter()
{
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/in/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf16Converter::do_in( mbstate_t& state
, const char* pFrom , const char* pFromMax , const char*& pFromNext
, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const
{
    return noconv;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/out/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf16Converter::do_out( mbstate_t& state
 , const wchar_t* pFrom, const wchar_t* pFromMax, const wchar_t*& pFromNext 
 , char* pTo, char* pToMax, char*& pToNext ) const
{
    return noconv;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/unshift/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf16Converter::do_unshift( mbstate_t& state, char* pTo , char* pToMax, char*& pToNext ) const
{
    return noconv;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/length/">c++ documentation</a> for details.
*/
int Utf16Converter::do_length( const mbstate_t& state , const char* pFrom, const char* pFromMax , size_t toLength ) const throw()
{
    return (toLength < (size_t)(pFromMax - pFrom)) ? toLength : pFromMax - pFrom ;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/always_noconv/">c++ documentation</a> for details.
**
** @return <code>true</code>
*/
bool Utf16Converter::do_always_noconv() const throw()
{
    return true;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/max_length/">c++ documentation</a> for details.
**
** @return <code>2</code> because this class only supports 16 bit unicode characters.
*/
int Utf16Converter::do_max_length() const throw()
{
    return 2;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/encoding/">c++ documentation</a> for details.
**
** @return <code>2</code> because this class only supports 16 bit unicode characters.
*/
int Utf16Converter::do_encoding() const throw()
{
    return 2;
}


// --------------------------------------------------------------------
// Utf16BeConverter
// --------------------------------------------------------------------

/**
** @brief Constructor.
*/
Utf16BeConverter::Utf16BeConverter( size_t refCount /* = 0 */ )
: base( refCount ) 
{
}

/**
** @brief Destructor.
*/
Utf16BeConverter::~Utf16BeConverter()
{
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/in/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf16BeConverter::do_in( mbstate_t& state
, const char* pFrom , const char* pFromMax , const char*& pFromNext
, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const
{
	for ( pFromNext = pFrom, pToNext = pTo; pFromNext + 1 < pFromMax && pToNext < pToMax; pFromNext+=2, ++pToNext ) {
		unsigned char loByte = unsigned char(pFromNext[1]);
		unsigned char hiByte = unsigned char(pFromNext[0]);
		*pToNext = wchar_t( hiByte << 8 | loByte );
	}

	return pFromNext == pFromMax ? ok : partial;;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/out/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf16BeConverter::do_out( mbstate_t& state
 , const wchar_t* pFrom, const wchar_t* pFromMax, const wchar_t*& pFromNext 
 , char* pTo, char* pToMax, char*& pToNext ) const
{
	for ( pFromNext = pFrom, pToNext = pTo; pFromNext < pFromMax && pToNext + 1 < pToMax; ++pFromNext, pToNext+= 2 ) {
		wchar_t c = *pFrom;
		unsigned char loByte = unsigned char(c & 0x00FF);
		unsigned char hiByte = unsigned char(c & 0xFF00 >> 8);
		pToNext[0] = hiByte;
		pToNext[1] = loByte;
	}

	return pFromNext == pFromMax ? ok : partial;;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/unshift/">c++ documentation</a> for details.
*/
std::codecvt_base::result Utf16BeConverter::do_unshift( mbstate_t& state, char* pTo , char* pToMax, char*& pToNext ) const
{
    return noconv;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/length/">c++ documentation</a> for details.
*/
int Utf16BeConverter::do_length( const mbstate_t& state , const char* pFrom, const char* pFromMax , size_t toLength ) const throw()
{
    return (toLength < (size_t)(pFromMax - pFrom)) ? toLength : pFromMax - pFrom ;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/always_noconv/">c++ documentation</a> for details.
**
** @return <code>true</code>
*/
bool Utf16BeConverter::do_always_noconv() const throw()
{
    return false;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/max_length/">c++ documentation</a> for details.
**
** @return <code>2</code> because this class only supports 16Be bit unicode characters.
*/
int Utf16BeConverter::do_max_length() const throw()
{
    return 2;
}

/**
** @brief See <a href="http://www.cplusplus.com/reference/std/locale/codecvt/encoding/">c++ documentation</a> for details.
**
** @return <code>2</code> because this class only supports 16Be bit unicode characters.
*/
int Utf16BeConverter::do_encoding() const throw()
{
    return 2;
}




} // namespace

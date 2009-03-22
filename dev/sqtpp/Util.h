#ifndef SQTPP_UTIL_H
#define SQTPP_UTIL_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

/**
** @brief Conversion cast.
**
** Helper to cast anything from and to string using C conventions.\p
** Usage:
** @code 
** wstring s = lexical_cast<wstring>(1);
** int     i = lexical_cast<int>(s);
** int     j = lexical_cast<int>("42");
** @endcode
*/
template<class TOUT, class TIN> TOUT lexical_cast( const TIN& input )
{
	TOUT result;
	std::wstringstream buffer;
	buffer.rdbuf()->pubimbue( std::locale::classic() );
	buffer.exceptions( ios::failbit | ios::badbit );
	buffer << input;
	buffer >> result;
	return result;
}

/**
** @brief Converting cast with local other than std::locale::classic().
*/
template<class TOUT, class TIN> TOUT lexical_cast( const TIN& input, const std::locale& loc )
{
	TOUT result;
	const std::locale& oldloc = locale::global( loc );
	try {
		result = lexical_cast<TOUT>(input);
		locale::global( oldloc );
	}
	catch ( ... ) {
		locale::global( oldloc );
		throw;
	}
	return result;
}

/**
** @brief conversion helpers.
*/
class Convert
{
private:
	/// Default constructor (not implemented).
	Convert();
public:
	// Convert a single unicode charater into a 8 Bit character.
	static char wc2ch( const wchar_t wc, const std::locale& loc );

	// Convert 8 bit string to a unicode string.
	static const std::wstring str2wcs( const char* str );

	// Convert unicode string to an 8 bit string .
	static const std::string wcs2str( const wchar_t* str );
};


/**
** @brief Some helper functions.
*/
class Util
{
private:
	/// Default constructor (not implemented).
	Util();
public:
	// Get current local date/time.
	static void getLocalTime( tm& localTime );

	// Remove leading and trailing blanks from a string
	static const wstring trim( const wstring& str );

};

} // namespace


#endif // SQTPP_UTIL_H

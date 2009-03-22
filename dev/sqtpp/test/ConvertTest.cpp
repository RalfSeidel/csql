#include "stdafx.h"
#include <strstream>
#include <cassert>
#include "convertTest.h"

namespace sqtpp {
namespace test {

ConvertTest::ConvertTest()
{
}

void ConvertTest::run()
{
	ConvertTest test;

	test.localeTest();
	test.cToUtf16Test();
	test.utf16ToCTest();
}

/**
** @brief Test of the STL locale class.
*/
void ConvertTest::localeTest()
{
	using std::locale;
	using std::endl;

	locale locDefault;
	locale locUS1252( "English_US.1252" );
	locale locGermany( "German_germany" );
	locale locBritain( "English_Britain" );

	assert( locDefault.name() == "C" );
	assert( locUS1252.name() == "English_United States.1252" );
	assert( locGermany.name() == "German_Germany.1252" );
	assert( locBritain.name() == "English_United Kingdom.1252" );

	locBritain = locale( "English_Britain.850" );
	assert( locBritain.name() == "English_United Kingdom.850" );
}

void ConvertTest::cToUtf16Test()
{
	//locale       loc("C"); //English_Britain");//German_Germany
	locale         loc1252("English_US.1252"); //English_Britain");//German_Germany
	locale         loc850("English_US.850");  //English_Britain");//German_Germany
	const char*    pszInput        =  "abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@€";
	const wchar_t* pwszExpectation = L"abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@\x20AC";
	const char*    pszInputNext = NULL;
	size_t         nInputLen    = strlen( pszInput );
	const size_t   nOutputLen   = 127;
	mbstate_t      state        = 0;
	wchar_t        pwszOutput[nOutputLen+1];
	wchar_t*       pwszOutputNext = NULL;
	char           pszBack[nOutputLen+1];
	char*          pszBackNext = NULL;

	memset( pwszOutput, 0, sizeof( pwszOutput ) );

	const std::codecvt<wchar_t, char, mbstate_t>& codec = use_facet<codecvt<wchar_t, char, mbstate_t> >( loc1252 );


	// Convert Windows 1252 --> UTF16

	int result = codec.in( state
				         , pszInput,    &pszInput[nInputLen],    pszInputNext
				         , pwszOutput, &pwszOutput[nInputLen], pwszOutputNext );
	pwszOutput[nInputLen] = 0;

	switch ( result ) {
		case codecvt_base::ok:
		case codecvt_base::noconv:
			break;
		case codecvt_base::error:
			break;
		case codecvt_base::partial:
			break;
		default:
			assert( false );
			throw std::runtime_error( string("Unexpected case value: %d", errno ) );
	}

	assert( result != codecvt_base::error );

	if ( result != codecvt_base::error ) {
		assert( wcscmp( pwszOutput, pwszExpectation ) == 0 );
	}

	// back

	const wchar_t* pwzsNext;

	result = codec.out( state
			          , pwszOutput, &pwszOutput[nInputLen], pwzsNext /* pwszOutputNext doesn't work - why? */
			          , pszBack,    &pszBack[nInputLen],    pszBackNext );
	pszBack[nInputLen] = 0;

	assert( result != codecvt_base::error );

	switch ( result ) {
		case codecvt_base::ok:
			break;
		case codecvt_base::noconv:
			break;
		case codecvt_base::error:
			break;
		case codecvt_base::partial:
			break;
		default:
			assert( false );
			throw std::runtime_error( string("Unexpected case value: %d", errno ) );
	}

	if ( result != codecvt_base::error ) {
		assert( strcmp( pszBack, pszInput ) == 0 );
	}
}

void ConvertTest::utf16ToCTest()
{
	//locale       loc("C"); //English_Britain");//German_Germany
	locale         loc1252("English_US.1252"); //English_Britain");//German_Germany
	locale         loc850("English_US.850");  //English_Britain");//German_Germany
	const wchar_t* pwszInput      = L"abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@\x20AC";
	const char*    pszExpectation =  "abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@€";
	const wchar_t* pwszInputNext = NULL;
	size_t         nInputLen    = wcslen( pwszInput );
	const size_t   nOutputLen   = 127;
	mbstate_t      state        = 0;
	char           pszOutput[nOutputLen+1];
	char*          pszOutputNext = NULL;
	wchar_t        pwszBack[nOutputLen+1];
	wchar_t*       pwszBackNext = NULL;

	memset( pszOutput, 0, sizeof( pszOutput ) );

//	const std::codecvt<char, wchar_t, mbstate_t>& codec = use_facet<codecvt<char, wchar_t, mbstate_t> >( loc1252 );
	const std::codecvt<wchar_t, char, mbstate_t>& codec = use_facet<codecvt<wchar_t, char, mbstate_t> >( loc1252 );


	// Convert Windows 1252 --> UTF16

	int result = codec.out( state
				          , pwszInput, &pwszInput[nInputLen], pwszInputNext
				          , pszOutput, &pszOutput[nInputLen], pszOutputNext );
	pszOutput[nInputLen] = 0;


	switch ( result ) {
		case codecvt_base::ok:
			break;
		case codecvt_base::noconv:
			break;
		case codecvt_base::error:
			break;
		case codecvt_base::partial:
			break;
		default:
			assert( false );
			throw std::runtime_error( string("Unexpected case value: %d", errno ) );
	}

	assert( result != codecvt_base::error );

	switch ( result ) {
		case codecvt_base::ok:
			break;
		case codecvt_base::noconv:
			break;
		case codecvt_base::error:
			break;
		case codecvt_base::partial:
			break;
		default:
			assert( false );
			throw std::runtime_error( string("Unexpected case value: %d", errno ) );
	}

	if ( result != codecvt_base::error ) {
		assert( strcmp( pszOutput, pszExpectation ) == 0 );
	}

	// back

	const char* pwzsNext;

	result = codec.in( state
			         , pszOutput, &pszOutput[nInputLen], pwzsNext /* pszOutputNext doesn't work - why? */
			         , pwszBack,  &pwszBack[nInputLen],  pwszBackNext);
	pwszBack[nInputLen] = 0;

	switch ( result ) {
		case codecvt_base::ok:
			break;
		case codecvt_base::noconv:
			break;
		case codecvt_base::error:
			break;
		case codecvt_base::partial:
			break;
		default:
			assert( false );
			throw std::runtime_error( string("Unexpected case value: %d", errno ) );
	}

	assert( result != codecvt_base::error );

	if ( result != codecvt_base::error ) {
		assert( wcscmp( pwszBack, pwszInput ) == 0 );
	}
}


} // namespace test
} // namespace sqtpp

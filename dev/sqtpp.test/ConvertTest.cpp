#include "stdafx.h"
#include <strstream>
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class ConvertTest : TestBase
{
public:

	/**
	** @brief Test of the STL locale class.
	*/
	[TestMethod]
	void localeTest()
	{
		using std::locale;
		using std::endl;

		locale locDefault;
		locale locUS1252( "English_US.1252" );
		locale locGermany( "German_germany" );
		locale locBritain( "English_Britain" );

		Assert::IsTrue( locDefault.name() == "C" );
		Assert::IsTrue( locUS1252.name() == "English_United States.1252" );
		Assert::IsTrue( locGermany.name() == "German_Germany.1252" );
		Assert::IsTrue( locBritain.name() == "English_United Kingdom.1252" );

		locBritain = locale( "English_Britain.850" );
		Assert::IsTrue( locBritain.name() == "English_United Kingdom.850" );
	}

	[TestMethod]
	void cToUtf16Test()
	{
		//locale       loc("C"); //English_Britain");//German_Germany
		locale         loc1252("English_US.1252"); //English_Britain");//German_Germany
		locale         loc850("English_US.850");  //English_Britain");//German_Germany
		const char*    pszInput        =  "abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@€";
		const wchar_t* pwszExpectation = L"abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@\x20AC";
		const char*    pszInputNext = NULL;
		size_t         nInputLen    = strlen( pszInput );
		const size_t   nOutputLen   = 127;
		wchar_t        pwszOutput[nOutputLen+1];
		wchar_t*       pwszOutputNext = NULL;
		char           pszBack[nOutputLen+1];
		char*          pszBackNext = NULL;
		mbstate_t      state;

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
				Assert::IsTrue( false );
				throw std::runtime_error( string("Unexpected case value: %d", errno ) );
		}

		Assert::IsTrue( result != codecvt_base::error );

		if ( result != codecvt_base::error ) {
			Assert::IsTrue( wcscmp( pwszOutput, pwszExpectation ) == 0 );
		}

		// back

		const wchar_t* pwzsNext;

		result = codec.out( state
						  , pwszOutput, &pwszOutput[nInputLen], pwzsNext /* pwszOutputNext doesn't work - why? */
						  , pszBack,    &pszBack[nInputLen],    pszBackNext );
		pszBack[nInputLen] = 0;

		Assert::IsTrue( result != codecvt_base::error );

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
				Assert::IsTrue( false );
				throw std::runtime_error( string("Unexpected case value: %d", errno ) );
		}

		if ( result != codecvt_base::error ) {
			Assert::IsTrue( strcmp( pszBack, pszInput ) == 0 );
		}
	}

	[TestMethod]
	void utf16ToCTest()
	{
		//locale       loc("C"); //English_Britain");//German_Germany
		locale         loc1252("English_US.1252"); //English_Britain");//German_Germany
		locale         loc850("English_US.850");  //English_Britain");//German_Germany
		const wchar_t* pwszInput      = L"abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@\x20AC";
		const char*    pszExpectation =  "abcdefghijklmnopqrstuvwxyzäöüß!\"§$%&/()=?+*~#'-_.:,;µ@€";
		const wchar_t* pwszInputNext = NULL;
		size_t         nInputLen    = wcslen( pwszInput );
		const size_t   nOutputLen   = 127;
		char           pszOutput[nOutputLen+1];
		char*          pszOutputNext = NULL;
		wchar_t        pwszBack[nOutputLen+1];
		wchar_t*       pwszBackNext = NULL;
		mbstate_t      state;

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
				Assert::IsTrue( false );
				throw std::runtime_error( string("Unexpected case value: %d", errno ) );
		}

		Assert::IsTrue( result != codecvt_base::error );

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
				Assert::IsTrue( false );
				throw std::runtime_error( string("Unexpected case value: %d", errno ) );
		}

		if ( result != codecvt_base::error ) {
			Assert::IsTrue( strcmp( pszOutput, pszExpectation ) == 0 );
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
				Assert::IsTrue( false );
				throw std::runtime_error( string("Unexpected case value: %d", errno ) );
		}

		Assert::IsTrue( result != codecvt_base::error );

		if ( result != codecvt_base::error ) {
			Assert::IsTrue( wcscmp( pwszBack, pwszInput ) == 0 );
		}
	}

}; // class

} // namespace test
} // namespace sqtpp

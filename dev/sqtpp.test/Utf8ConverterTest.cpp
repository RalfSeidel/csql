#include "stdafx.h"
#include "CodePageConverter.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class Utf8ConverterTest : public TestBase
{
public:
	/**
	** @brief Test for is_start_byte method
	*/
	[TestMethod]
	void isStartByteTest()
	{
		bool is_start_byte;
		char c;
		for ( c = '\0'; c != char('\127'); ++c ) {
			is_start_byte = Utf8Converter::is_start_byte( c );
			Assert::IsTrue( !is_start_byte );
		}

		c = '\xC0';
		is_start_byte = Utf8Converter::is_start_byte( c );
		Assert::IsTrue( is_start_byte );

		c = '\xE0';
		is_start_byte = Utf8Converter::is_start_byte( c );
		Assert::IsTrue( is_start_byte );

		c = '\xF0';
		is_start_byte = Utf8Converter::is_start_byte( c );
		Assert::IsTrue( is_start_byte );

	}

	/**
	** @brief Test for sequence_length method
	*/
	[TestMethod]
	void sequenceLengthTest()
	{
		int length;
		char c;
		for ( c = '\0'; c != char('\127'); ++c ) {
			length = Utf8Converter::sequence_length( c );
			Assert::IsTrue( length == 1 );
		}

		c = '\xC0';
		length = Utf8Converter::sequence_length( c );
		Assert::IsTrue( length == 2 );

		c = '\xE0';
		length = Utf8Converter::sequence_length( c );
		Assert::IsTrue( length == 3 );

		c = '\xF0';
		length = Utf8Converter::sequence_length( c );
		Assert::IsTrue( length == 4 );

		c = '\xF8';
		length = Utf8Converter::sequence_length( c );
		Assert::IsTrue( length == 5 );

		c = '\xFC';
		length = Utf8Converter::sequence_length( c );
		Assert::IsTrue( length == 6 );

		c = '\xFE';
		length = Utf8Converter::sequence_length( c );
		Assert::IsTrue( length == 7 );

		c = '\xFF';
		length = Utf8Converter::sequence_length( c );
		Assert::IsTrue( length == 8 );
	}

	/**
	** @brief Test for Utf7ConverterTest::length method
	*/
	[TestMethod]
	void lengthTest()
	{
		Utf8Converter converter(0);

		mbstate_t state;
		char inAbc[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		// "ä"
		char inUmlaut[] = "\xC3\xA4";
		// "€"
		char inEuro[] = "\xE2\x82\xAC";
		const char* pIn;
		const char* pInMax;
		size_t inCount;
		int result;

		pIn = inAbc;
		inCount = strlen( pIn );
		pInMax = pIn + inCount;
		result = converter.length( state, pIn, pInMax, 100 );
		Assert::IsTrue( size_t(result) == inCount );

		pIn = inUmlaut;
		inCount = strlen( pIn );
		pInMax = pIn + inCount;
		result = converter.length( state, pIn, pInMax, 100 );
		Assert::IsTrue( size_t(result) == inCount );

		pIn = inEuro;
		inCount = strlen( pIn );
		pInMax = pIn + inCount;
		result = converter.length( state, pIn, pInMax, 100 );
		Assert::IsTrue( size_t(result) == inCount );
	}


	/**
	** @brief Test for in method
	*/
	[TestMethod]
	void inTest()
	{
		Utf8Converter converter(0);

		mbstate_t state;
		char inAbc[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		// "ä"
		//char inUmlaut[] = "\xC3\xA4";
		// "€"
		//char inEuro[] = "\xE2\x82\xAC";
		const char* pIn;
		const char* pInMax;
		const char* pInNext;
		size_t inCount;

		wchar_t  outBuffer[128];
		wchar_t* pOut;
		wchar_t* pOutMax;
		wchar_t* pOutNext;

		int result;

		pIn = inAbc;
		inCount = strlen( pIn );
		pInMax = pIn + inCount;

		pOut = outBuffer;
		pOutMax = pOut + inCount;
		result = converter.in( state, pIn, pInMax, pInNext, pOut, pOutMax, pOutNext );
		Assert::IsTrue( result == std::codecvt_base::ok );
		Assert::IsTrue( pInNext == pInMax );
		Assert::IsTrue( pOutNext == pOutMax );

		pIn = inAbc;
		inCount = strlen( pIn );
		pInMax = pIn + inCount;
		pOut = outBuffer;
		pOutMax = pOut + 1;
		result = converter.in( state, pIn, pInMax, pInNext, pOut, pOutMax, pOutNext );
		Assert::IsTrue( result == std::codecvt_base::partial );
		Assert::IsTrue( pInNext == pIn + 1 );
		Assert::IsTrue( pOutNext == pOut + 1 );
	}

	/**
	** @brief Test for in method
	*/
	[TestMethod]
	void outTest()
	{
		Utf8Converter converter(0);

		int result;
		mbstate_t state;
		wchar_t inAbc[] = L"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		//wchar_t inUmlaut[] = L"ä";
		//wchar_t inEuro[] = L"€";
		const wchar_t* pIn;
		const wchar_t* pInMax;
		const wchar_t* pInNext;
		int inCount;

		char  outBuffer[128];
		char* pOut;
		char* pOutMax;
		char* pOutNext;

		pIn = inAbc;
		inCount = wcslen( pIn );
		pInMax = pIn + inCount;
		pOut = outBuffer;
		pOutMax = pOut + inCount;
		result = converter.out( state, pIn, pInMax, pInNext, pOut, pOutMax, pOutNext );
		Assert::IsTrue( result == std::codecvt_base::ok );
		Assert::IsTrue( pInNext == pIn + inCount );
		Assert::IsTrue( pOutNext == pOut + inCount );
	}
}; // class

} // namespace test
} // namespace sqtpp

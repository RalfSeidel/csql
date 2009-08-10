#include "stdafx.h"
#include <cassert>
#include "../CodePageConverter.h"
#include "Utf8ConverterTest.h"

namespace sqtpp {
namespace test {

Utf8ConverterTest::Utf8ConverterTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void Utf8ConverterTest::run()
{
	Utf8ConverterTest test;

	test.isStartByteTest();
	test.sequenceLengthTest();
	test.lengthTest();
	test.inTest();
	test.outTest();
}

/**
** @brief Test for Utf8ConverterTest::is_start_byte method
*/
void Utf8ConverterTest::isStartByteTest()
{
	bool is_start_byte;
	char c;
	for ( c = '\0'; c != char('\128'); ++c ) {
		is_start_byte = Utf8Converter::is_start_byte( c );
		assert( !is_start_byte );
	}

	c = '\xC0';
	is_start_byte = Utf8Converter::is_start_byte( c );
	assert( is_start_byte );

	c = '\xE0';
	is_start_byte = Utf8Converter::is_start_byte( c );
	assert( is_start_byte );

	c = '\xF0';
	is_start_byte = Utf8Converter::is_start_byte( c );
	assert( is_start_byte );

}

/**
** @brief Test for Utf8ConverterTest::sequence_length method
*/
void Utf8ConverterTest::sequenceLengthTest()
{
	int length;
	char c;
	for ( c = '\0'; c != char('\128'); ++c ) {
		length = Utf8Converter::sequence_length( c );
		assert( length == 1 );
	}

	c = '\xC0';
	length = Utf8Converter::sequence_length( c );
	assert( length == 2 );

	c = '\xE0';
	length = Utf8Converter::sequence_length( c );
	assert( length == 3 );

	c = '\xF0';
	length = Utf8Converter::sequence_length( c );
	assert( length == 4 );

	c = '\xF8';
	length = Utf8Converter::sequence_length( c );
	assert( length == 5 );

	c = '\xFC';
	length = Utf8Converter::sequence_length( c );
	assert( length == 6 );

	c = '\xFE';
	length = Utf8Converter::sequence_length( c );
	assert( length == 7 );

	c = '\xFF';
	length = Utf8Converter::sequence_length( c );
	assert( length == 8 );
}

/**
** @brief Test for Utf7ConverterTest::length method
*/
void Utf8ConverterTest::lengthTest()
{
	Utf8Converter converter(0);

	mbstate_t state = 0;
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
	assert( size_t(result) == inCount );

	pIn = inUmlaut;
	inCount = strlen( pIn );
	pInMax = pIn + inCount;
	result = converter.length( state, pIn, pInMax, 100 );
	assert( size_t(result) == inCount );

	pIn = inEuro;
	inCount = strlen( pIn );
	pInMax = pIn + inCount;
	result = converter.length( state, pIn, pInMax, 100 );
	assert( size_t(result) == inCount );
}




/**
** @brief Test for Utf8ConverterTest::in method
*/
void Utf8ConverterTest::inTest()
{
	Utf8Converter converter(0);

	mbstate_t state = 0;
	char inAbc[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	// "ä"
	char inUmlaut[] = "\xC3\xA4";
	// "€"
	char inEuro[] = "\xE2\x82\xAC";
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
	assert( result == std::codecvt_base::ok );
	assert( pInNext == pInMax );
	assert( pOutNext == pOutMax );

	pIn = inAbc;
	inCount = strlen( pIn );
	pInMax = pIn + inCount;
	pOut = outBuffer;
	pOutMax = pOut + 1;
	result = converter.in( state, pIn, pInMax, pInNext, pOut, pOutMax, pOutNext );
	assert( result == std::codecvt_base::partial );
	assert( pInNext == pIn + 1 );
	assert( pOutNext == pOut + 1 );
}

/**
** @brief Test for Utf8ConverterTest::in method
*/
void Utf8ConverterTest::outTest()
{
	Utf8Converter converter(0);

	int result;
	mbstate_t state = 0;
	wchar_t inAbc[] = L"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	wchar_t inUmlaut[] = L"ä";
	wchar_t inEuro[] = L"€";
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
	assert( result == std::codecvt_base::ok );
	assert( pInNext == pIn + inCount );
	assert( pOutNext == pOut + inCount );
}

} // namespace test
} // namespace sqtpp

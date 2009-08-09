#include "stdafx.h"
#include <cassert>
#include "../CodePageConverter.h"
#include "Utf7ConverterTest.h"

namespace sqtpp {
namespace test {

Utf7ConverterTest::Utf7ConverterTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void Utf7ConverterTest::run()
{
	Utf7ConverterTest test;

	test.isStartByteTest();
	test.lengthTest();
	test.inTest();
	test.outTest();
}

/**
** @brief Test for Utf7ConverterTest::is_start_byte method
*/
void Utf7ConverterTest::isStartByteTest()
{
	bool is_start_byte;
	char c;

	c = '+';
	is_start_byte = Utf7Converter::is_start_byte( c );
	assert( is_start_byte );
}

/**
** @brief Test for Utf7ConverterTest::length method
*/
void Utf7ConverterTest::lengthTest()
{
}


/**
** @brief Test for Utf7ConverterTest::in method
*/
void Utf7ConverterTest::inTest()
{
}

/**
** @brief Test for Utf7ConverterTest::in method
*/
void Utf7ConverterTest::outTest()
{
}

} // namespace test
} // namespace sqtpp

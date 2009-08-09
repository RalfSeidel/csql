#ifndef SQTPP_UTF8CONVERTER_TEST_H
#define SQTPP_UTF8CONVERTER_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.Utf8Converter
*/
class Utf8ConverterTest : public Test
{
public:
	Utf8ConverterTest();
	~Utf8ConverterTest() throw() {}

	/// Execute all tests.
	static void run();

	/// Test for is_start_byte
	void isStartByteTest();

	/// Test for sequence_length
	void sequenceLengthTest();

	/// Test for length
	void lengthTest();

	/// Test for in
	void inTest();

	/// Test for in
	void outTest();
};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_UTF8CONVERTER_TEST_H

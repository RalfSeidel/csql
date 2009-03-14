#ifndef SQTPP_NSTRINGSTREAM_TEST_H
#define SQTPP_NSTRINGSTREAM_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for NStringStream.
*/
class NStringStreamTest : public Test
{
public:
	static void run();


	void setStringTest();
	void getStringTest();
};


} // namespace test
} // namespace sqtpp

#endif // SQTPP_NSTRINGSTREAM_TEST_H

#ifndef SQTPP_CONVERT_TEST_H
#define SQTPP_CONVERT_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Character/string conversion tests.
*/
class ConvertTest : public Test
{
public:
	ConvertTest();
	~ConvertTest() throw() {}

	/// Execute all defined tests.
	static void run();

	void localeTest();
	void cToUtf16Test();
	void utf16ToCTest();
};



} // namespace test
} // namespace sqtpp




#endif // SQTPP_CONVERT_TEST_H

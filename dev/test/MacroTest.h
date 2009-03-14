#ifndef SQTPP_MACRO_TEST_H
#define SQTPP_MACRO_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.Macro
**
** @todo Currently no test is defined.
*/
class MacroTest : public Test
{
public:
	MacroTest();
	~MacroTest() throw() {}

	/// Execute all tests.
	static void run();


};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_MACRO_TEST_H

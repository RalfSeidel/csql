#ifndef SQTPP_BUILDIN_TEST_H
#define SQTPP_BUILDIN_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.Buildin.
**
** @todo Define tests for all buildin macros.
*/
class BuildinTest : public Test
{
public:
	BuildinTest();
	~BuildinTest() throw() {}

	/// Execute all tests.
	static void run();


	/// Test __EVAL
	void evalTest();
	void eval2Test();

	/// Test __QUOTE
	void quoteTest();


};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_BUILDIN_TEST_H

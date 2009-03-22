#ifndef SQTPP_ERROR_TEST_H
#define SQTPP_ERROR_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class #sqtpp::error::Error
**
** @todo Implement tests.
*/
class ErrorTest : public Test
{
public:
	ErrorTest();
	~ErrorTest() throw() {}

	/// Execute all tests.
	static void run();


};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_ERROR_TEST_H

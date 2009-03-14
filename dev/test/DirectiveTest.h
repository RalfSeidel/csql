#ifndef SQTPP_DIRECTIVE_TEST_H
#define SQTPP_DIRECTIVE_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.Directive
*/
class DirectiveTest : public Test
{
public:

	DirectiveTest();
	~DirectiveTest() throw() {}

	/// Execute all tests.
	static void run();

	/// Test the getDirective methode.
	void getDirectiveTest();

	/// Test the findDirectiveInfo methode.
	void findDirectiveInfoTest();
};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_DIRECTIVE_TEST_H

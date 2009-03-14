#ifndef SQTPP_TOKEN_TEST_H
#define SQTPP_TOKEN_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.Token
*/
class TokenTest : public Test
{
public:

	TokenTest();
	~TokenTest() throw() {}

	/// Execute all tests.
	static void run();

	/// Test the getToken methode.
	void getTokenTest();

	/// Test the getTokenInfo methode.
	void getTokenInfoTest();
};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_TOKEN_TEST_H

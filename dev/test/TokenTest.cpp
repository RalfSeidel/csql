#include "stdafx.h"
#include <cassert>
#include "../Token.h"
#include "TokenTest.h"

namespace sqtpp {
namespace test {

TokenTest::TokenTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void TokenTest::run()
{
	TokenTest test;

	test.getTokenTest();
	test.getTokenInfoTest();
}

/**
** @brief Test the TokenInfo::getToken methode.
*/
void TokenTest::getTokenTest()
{
}

/**
** @brief Test the TokenInfo::getTokenInfo methode.
*/
void TokenTest::getTokenInfoTest()
{
}

} // namespace test
} // namespace sqtpp

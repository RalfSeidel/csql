#include "stdafx.h"
#include "Token.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class TokenTest : public TestBase
{
public:
	/**
	** @brief Test the TokenInfo::getToken methode.
	*/
	[TestMethod]
	void getTokenTest()
	{
	}

	/**
	** @brief Test the TokenInfo::getTokenInfo methode.
	*/
	[TestMethod]
	void getUndefinedTokenInfoTest()
	{
		const TokenInfo& tokenInfo = TokenInfo::getTokenInfo( TOK_UNDEFINED );
		Assert::IsTrue( tokenInfo.token == TOK_UNDEFINED );
		Assert::IsTrue( gcnew String(tokenInfo.pwcSymbol) == "TOK_UNDEFINED" );

	}
}; // class

} // namespace test
} // namespace sqtpp

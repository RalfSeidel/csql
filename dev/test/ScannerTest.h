#ifndef SQTPP_SCANNER_TEST_H
#define SQTPP_SCANNER_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
class Options;
class TokenExpression;
class TokenExpressions;
};

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp::Scanner
*/
class ScannerTest : public Test
{
private:
	Options& m_options;

	ScannerTest& operator=( const ScannerTest& );
public:
	ScannerTest();
	~ScannerTest() throw();

	/// Execute all tests.
	static void run();

	/// Test scanning text without any preprocessor directives.
	void scanTextTest();

	/// Test scanning identifier.
	void scanIdentifierTest();

	/// Test scanning identifier.
	void scanNumberTest();

	/// Test scanning identifier.
	void scanStringTest();

	/// Test scanning comments.
	void scanComment1Test();

	/// Test scanning comments.
	void scanComment2Test();

	/// Test scanning a directive
	void scanDirectiveTest();

	/// Test scanning a directive
	void scanDirective2Test();

	/// Test scanning a directive not starting a the beginning of the line.
	void scanNonDirectiveTest();

	/// Test a simple macro define a directive.
	void scanDefineTest();

	/// Test a simple macro define a directive.
	void scanDefine2Test();

	/// Test scanning new line characters.
	void scanNewLineTest();

	/// Test scanning operators.
	void scanOperatorTest();

private:

	/// Test sequence of tokens
	void testTokenSequence( const wstring& scannerInput, const TokenExpressions& tokens );

};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_SCANNER_TEST_H

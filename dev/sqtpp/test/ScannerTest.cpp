#include "stdafx.h"
#include <cassert>
#include "../Options.h"
#include "../Token.h"
#include "../Context.h"
#include "../Scanner.h"
#include "ScannerTest.h"

namespace sqtpp {
namespace test {

ScannerTest::ScannerTest()
: m_options( *new Options() )
{
	m_options.setNewLineOutput( Options::NLO_AS_IS );
	m_options.keepComments( false );
}

ScannerTest::~ScannerTest()
{
	delete &m_options;
}

/**
** @brief Run all defined tests for this class.
*/
void ScannerTest::run()
{
	ScannerTest test;

	test.scanTextTest();
	test.scanIdentifierTest();
	test.scanNumberTest();
	test.scanStringTest();
	test.scanComment1Test();
	test.scanComment2Test();
	test.scanNonDirectiveTest();
	test.scanNonDirectiveTest2();
	test.scanDirectiveTest();
	test.scanDefineTest();
	test.scanDefine2Test();
	test.scanNewLineTest();
	test.scanOperatorTest();
}


/**
** @brief Test scanning text without any preprocessor directives.
*/
void ScannerTest::scanTextTest()
{
	wstring          inputText = L"A\nB\nC";
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"B" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"C" ) );
	tokensExpected.push_back( TokenExpression( TOK_END_OF_FILE, CTX_DEFAULT, L"" ) );

	testTokenSequence( inputText, tokensExpected );
}



/**
** @brief Test scanning identifier.
*/
void ScannerTest::scanIdentifierTest()
{
	wstring          inputText = L"(A B C)";
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"B" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"C" ) );
	tokensExpected.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
	tokensExpected.push_back( TokenExpression( TOK_END_OF_FILE, CTX_DEFAULT, L"" ) );

	testTokenSequence( inputText, tokensExpected );
}

/**
** @brief Test scanning numbers.
*/
void ScannerTest::scanNumberTest()
{
	wstring          inputText = L"(1+010+0x10)";
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"010" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0x10" ) );
	tokensExpected.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
	tokensExpected.push_back( TokenExpression( TOK_END_OF_FILE, CTX_DEFAULT, L"" ) );

	testTokenSequence( inputText, tokensExpected );
}

/**
** @brief Test scanning strings.
*/
void ScannerTest::scanStringTest()
{
	wstring          inputText;;
	TokenExpressions tokensExpected;
	bool bMultiLineString = m_options.multiLineStringLiterals();
	Options::Quoting  quoting      = m_options.getStringQuoting();

	inputText = L"\"\"";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\"\"" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"''";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"''" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"\"A\"";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\"A\"" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"\'A\'";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\'A\'" ) );
	testTokenSequence( inputText, tokensExpected );


	inputText = L"\'A\'\n";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\'A\'" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\n" ) );
	testTokenSequence( inputText, tokensExpected );

	m_options.setStringQuoting( Options::QUOT_ESCAPE );

	inputText = L"\"A \\\"string\\\"\"";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\"A \\\"string\\\"\"" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"\'A \\\'string\\\'\'";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\'A \\\'string\\\'\'" ) );
	testTokenSequence( inputText, tokensExpected );

	m_options.setStringQuoting( Options::QUOT_DOUBLE );

	inputText = L"\"A \"\"string\"\"\"";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\"A \"\"string\"\"\"" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"\'A \'\'string\'\'\'";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"\'A \'\'string\'\'\'" ) );
	testTokenSequence( inputText, tokensExpected );

	m_options.setStringQuoting( quoting );

	m_options.multiLineStringLiterals( true );

	inputText = L"\"A\nmulti\nline\nstring\"";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DQUOTE_STRING, L"\"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DQUOTE_STRING, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DQUOTE_STRING, L"multi" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DQUOTE_STRING, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DQUOTE_STRING, L"line" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DQUOTE_STRING, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_STRING, CTX_DEFAULT, L"string\"" ) );
	testTokenSequence( inputText, tokensExpected );

	m_options.multiLineStringLiterals( bMultiLineString );
}


/**
** @brief Test scanning comments.
*/
void ScannerTest::scanComment1Test()
{
	wstring          inputText;;
	TokenExpressions tokensExpected;

	inputText = L"\r\n"
		        L"-- \"A COMMENT\"\r\n"
		        L"// 'B COMMENT'\r\n";

	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_SQL_LINE_COMMENT, CTX_DEFAULT, L"-- \"A COMMENT\"") );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_LINE_COMMENT, CTX_DEFAULT, L"// 'B COMMENT'" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r\n" ) );
	
	testTokenSequence( inputText, tokensExpected );
}


/**
** @brief Test scanning comments.
**
** This test fixes an error encountered with block comments ending with an even number of stars.
*/
void ScannerTest::scanComment2Test()
{
	wstring          inputText;;
	TokenExpressions tokensExpected;

	inputText = L"/* comment */";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_DEFAULT, inputText ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"/** comment **/";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_DEFAULT, inputText ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"/*** comment ***/";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_DEFAULT, inputText ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"/**** comment ****/";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_DEFAULT, inputText ) );
	testTokenSequence( inputText, tokensExpected );
}


/**
** @brief Test scanning preprocessor directives.
*/
void ScannerTest::scanDirectiveTest()
{
	wstring          inputText( L"#directive MACRO A" );
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_DIRECTIVE, CTX_DEFAULT, L"directive" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"MACRO" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_END_OF_FILE, CTX_DEFAULT, L"" ) );

	testTokenSequence( inputText, tokensExpected );
}



/**
** @brief Test scanning a directive not starting a the beginning of the line.
**
** Test scanning a #define directive which is not placed at the beginning of the 
** line i.e. which is not a preprocess directive.
*/
void ScannerTest::scanNonDirectiveTest()
{
	wstring          inputText( L".#define MACRO A" );
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_OTHER, CTX_DEFAULT, L"." ) );
	tokensExpected.push_back( TokenExpression( TOK_SHARP, CTX_DEFAULT, L"#" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"define" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"MACRO" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_END_OF_FILE, CTX_DEFAULT, L"" ) );

	testTokenSequence( inputText, tokensExpected );
}


/**
** @brief Test scanning a script with a temporary table name.
**
** Repro for a problem found by Melanie. The following 
** line i.e. which is not a preprocess directive::
** @code
**	insert into 
**		#temp
** values ...
** @endcode
*/
void ScannerTest::scanNonDirectiveTest2()
{
	wstring          inputText( L"insert into\r\n\t#temp" );
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"insert" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"into" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L"\t" ) );
	tokensExpected.push_back( TokenExpression( TOK_DIRECTIVE, CTX_DEFAULT, L"temp" ) );

	testTokenSequence( inputText, tokensExpected );
}



/**
** @brief Test a simple macro define a directive.
*/
void ScannerTest::scanDefineTest()
{
	wstring          inputText( L"#define MACRO A" );
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_DIR_DEFINE, CTX_DEFAULT, L"define" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"MACRO" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_END_OF_FILE, CTX_DEFAULT, L"" ) );

	testTokenSequence( inputText, tokensExpected );
}


/**
** @brief Test scanning preprocessor directives.
*/
void ScannerTest::scanDefine2Test()
{
	wstring          inputText( L"#define A B\n#define B A" );
	TokenExpressions tokensExpected;

	tokensExpected.push_back( TokenExpression( TOK_DIR_DEFINE, CTX_DEFAULT, L"define" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"B" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\n" ) );

	tokensExpected.push_back( TokenExpression( TOK_DIR_DEFINE, CTX_DEFAULT, L"define" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"B" ) );
	tokensExpected.push_back( TokenExpression( TOK_SPACE, CTX_DEFAULT, L" " ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );

	testTokenSequence( inputText, tokensExpected );
}


/**
** @brief Test scanning preprocessor directives.
*/
void ScannerTest::scanNewLineTest()
{
	wstring          inputText;
	TokenExpressions tokensExpected;

	// Unix
	inputText = L"\n";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\n" ) );
	testTokenSequence( inputText, tokensExpected );

	// DOS
	inputText = L"\r\n";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r\n" ) );
	testTokenSequence( inputText, tokensExpected );

	// MAC
	inputText = L"\r";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r" ) );
	testTokenSequence( inputText, tokensExpected );

	// Mixed (Unix, DOS, MAC)
	inputText = L"\n\r\n\r";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r" ) );
	testTokenSequence( inputText, tokensExpected );

	// Now some text inside...

	// Unix
	inputText = L"A\nB";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"B" ) );
	testTokenSequence( inputText, tokensExpected );

	// DOS
	inputText = L"A\r\nB";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"B" ) );
	testTokenSequence( inputText, tokensExpected );

	// Mac
	inputText = L"A\rB";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_DEFAULT, L"\r" ) );
	tokensExpected.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"B" ) );
	testTokenSequence( inputText, tokensExpected );


	// With comment

	// Unix
	inputText = L"/*A\n\nB*/";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_BLOCK_COMMENT, L"/*A" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_BLOCK_COMMENT, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_BLOCK_COMMENT, L"\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_DEFAULT, L"B*/" ) );
	testTokenSequence( inputText, tokensExpected );

	// DOS
	inputText = L"/*\r\n\r\nB*/";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_BLOCK_COMMENT, L"/*" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_BLOCK_COMMENT, L"\r\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_BLOCK_COMMENT, L"\r\n" ) );
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_DEFAULT, L"B*/" ) );
	testTokenSequence( inputText, tokensExpected );

	// Mac
	inputText = L"/*\rA\rB*/";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_BLOCK_COMMENT, L"/*" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_BLOCK_COMMENT, L"\r" ) );
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_BLOCK_COMMENT, L"A" ) );
	tokensExpected.push_back( TokenExpression( TOK_NEW_LINE, CTX_BLOCK_COMMENT, L"\r" ) );
	tokensExpected.push_back( TokenExpression( TOK_BLOCK_COMMENT, CTX_DEFAULT, L"B*/" ) );
	testTokenSequence( inputText, tokensExpected );
}


/**
** @brief Test scanning operators.
*/
void ScannerTest::scanOperatorTest()
{
	wstring          inputText;
	TokenExpressions tokensExpected;

	inputText = L"1==0";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"1!=0";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_NE, CTX_DEFAULT, L"!=" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"1&0";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_BIT_AND, CTX_DEFAULT, L"&" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"1&&0";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_LOGICAL_AND, CTX_DEFAULT, L"&&" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"1^0";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_BIT_XOR, CTX_DEFAULT, L"^" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
	testTokenSequence( inputText, tokensExpected );

	inputText = L"1^^0";
	tokensExpected.clear();
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
	tokensExpected.push_back( TokenExpression( TOK_OP_LOGICAL_XOR, CTX_DEFAULT, L"^^" ) );
	tokensExpected.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
	testTokenSequence( inputText, tokensExpected );
}


/**
** @brief Test sequence of tokens.
*/
void ScannerTest::testTokenSequence( const wstring& inputText, const TokenExpressions& tokens )
{
	Scanner          scanner( m_options );
	wstringstream    input( inputText );


	for ( size_t i = 0; i < tokens.size(); ++i ) {
		const TokenExpression& expectedExpr = tokens[i];
		TokenExpression  foundExpr;
		scanner.getNextToken( input, foundExpr );

		assert( foundExpr.getToken() == expectedExpr.getToken() );
		assert( foundExpr.getIdentifier() == expectedExpr.getIdentifier() );
	}
}


} // namespace test
} // namespace sqtpp

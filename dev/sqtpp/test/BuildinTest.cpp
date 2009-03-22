#include "stdafx.h"
#include "../Buildin.h"
#include "../Options.h"
#include "../Processor.h"
#include "BuildinTest.h"

namespace sqtpp {
namespace test {

BuildinTest::BuildinTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void BuildinTest::run()
{
	wclog << L"BuildinTest" << endl;
	BuildinTest test;

	test.evalTest();
	test.eval2Test();
	test.quoteTest();
}


/**
** @brief Test the evaluation buildin macro (__EVAL).
*/
void BuildinTest::evalTest()
{
	Options       options;
	Processor     processor( options );
	wstring       inputExpr;
	wstring       inputText;
	wstringstream input;
	wstringstream output;
	wstring       outputText;

	options.emitLine( false );

	processor.setOutStream( output );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();

	inputExpr = L"1";
	inputText = L"__EVAL(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"1" );

	inputExpr = L"1 + 2";
	inputText = L"__EVAL(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"3" );

	inputExpr = L"1 << 2";
	inputText = L"__EVAL(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"4" );

	inputExpr = L"(1 + 2 * 2) << 1";
	inputText = L"__EVAL(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"10" );


	inputExpr = L"defined MACRO";
	inputText = L"__EVAL(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"0" );
}


/**
** @brief Further testing of the evaluation buildin macro (__EVAL).
*/
void BuildinTest::eval2Test()
{
	Options       options;
	Processor     processor( options );
	wstring       inputText;
	wstringstream input;
	wstringstream output;
	wstring       outputText;

	options.emitLine( false );
	options.eliminateEmptyLines( true );

	processor.setOutStream( output );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();

	inputText = L"#define BIT0 1\n"
	            L"#define BIT1 __EVAL( BIT0 << 1 )\n"
	            L"#define BIT2 __EVAL( BIT0 << 2 )\n"
	            L"#define BIT3 __EVAL( BIT2 << 1 )\n";

	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"" );

	inputText = L"BIT0";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"1" );

	inputText = L"BIT1";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"2" );

	inputText = L"BIT2";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"4" );

	inputText = L"BIT3";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"8" );
}



/**
** @brief Test the __QUOTE macro
*/
void BuildinTest::quoteTest()
{
	Options       options;
	Processor     processor( options );
	wstring       inputExpr;
	wstring       inputText;
	wstringstream input;
	wstringstream output;
	wstring       outputText;

	options.emitLine( false );
	options.eliminateEmptyLines( true );
	options.setStringDelimiter( Options::STRD_SINGLE );
	options.setStringQuoting( Options::QUOT_DOUBLE );

	processor.setOutStream( output );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();

	inputExpr = L"A";
	inputText = L"__QUOTE(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"'A'" );

	inputExpr = L"'A'";
	inputText = L"__QUOTE(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"'''A'''" );

	inputExpr = L"A";
	inputText = L"#define A B\n";
	inputText+= L"__QUOTE(" + inputExpr + L")";
	input.clear();
	input.str( inputText );
	output.str( wstring() );
	processor.processStream( input );
	outputText = output.str();
	assert( outputText == L"'B'" );
}


} // namespace test
} // namespace sqtpp

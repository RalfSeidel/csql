#include "stdafx.h"
#include "Buildin.h"
#include "Options.h"
#include "Processor.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class BuildinTest : public TestBase
{
public:

	/**
	** @brief Test the evaluation buildin macro (__EVAL).
	*/
	[TestMethod]
	void evalTest()
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
		Assert::IsTrue( outputText == L"1" );

		inputExpr = L"1 + 2";
		inputText = L"__EVAL(" + inputExpr + L")";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"3" );

		inputExpr = L"1 << 2";
		inputText = L"__EVAL(" + inputExpr + L")";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"4" );

		inputExpr = L"(1 + 2 * 2) << 1";
		inputText = L"__EVAL(" + inputExpr + L")";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"10" );


		inputExpr = L"defined MACRO";
		inputText = L"__EVAL(" + inputExpr + L")";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"0" );
	}


	/**
	** @brief Further testing of the evaluation buildin macro (__EVAL).
	*/
	[TestMethod]
	void eval2Test()
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
		Assert::IsTrue( outputText == L"" );

		inputText = L"BIT0";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"1" );

		inputText = L"BIT1";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"2" );

		inputText = L"BIT2";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"4" );

		inputText = L"BIT3";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"8" );
	}



	/**
	** @brief Test the __QUOTE macro
	*/
	[TestMethod]
	void quoteTest()
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
		Assert::IsTrue( outputText == L"'A'" );

		inputExpr = L"'A'";
		inputText = L"__QUOTE(" + inputExpr + L")";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"'''A'''" );

		inputExpr = L"A";
		inputText = L"#define A B\n";
		inputText+= L"__QUOTE(" + inputExpr + L")";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"'B'" );
	}


	/**
	** @brief Test the __DATE__ macro
	*/
	[TestMethod]
	void dateTest()
	{
		Options       options;
		Processor     processor( options );
		wstring       inputExpr;
		wstring       inputText;
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		tm           testTime;

		memset( &testTime, 0, sizeof( testTime ) );
		testTime.tm_year = 101; // 2001
		testTime.tm_mon  = 8;   // Sep
		testTime.tm_mday = 30;
		testTime.tm_hour = 16;
		testTime.tm_min = 45;
		testTime.tm_sec = 10;
		processor.setTimestamp( &testTime );

		options.setLanguage( Options::LNG_SQL );
		options.emitLine( false );
		options.eliminateEmptyLines( true );

		processor.setOutStream( output );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();

		inputText = L"__DATE__";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"'20010930'" );
	}

	/**
	** @brief Test the __TIME__ macro
	*/
	[TestMethod]
	void timeTest()
	{
		Options       options;
		Processor     processor( options );
		wstring       inputExpr;
		wstring       inputText;
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		tm           testTime;

		memset( &testTime, 0, sizeof( testTime ) );
		testTime.tm_year = 101; // 2001
		testTime.tm_mon  = 8;   // Sep
		testTime.tm_mday = 30;
		testTime.tm_hour = 16;
		testTime.tm_min = 45;
		testTime.tm_sec = 10;
		processor.setTimestamp( &testTime );

		options.setLanguage( Options::LNG_SQL );
		options.emitLine( false );
		options.eliminateEmptyLines( true );

		processor.setOutStream( output );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();

		inputText = L"__TIME__";
		input.clear();
		input.str( inputText );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"'16:45:10'" );
	}
}; // class


} // namespace test
} // namespace sqtpp

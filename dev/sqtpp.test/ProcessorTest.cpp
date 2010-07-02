#include "stdafx.h"
#include "Options.h"
#include "Scanner.h"
#include "Processor.h"
#include "Error.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class ProcessorTest : public TestBase
{
public:
	/**
	** @brief Test processing macro definitions.
	*/
	[TestMethod]
	void processTextTest()
	{
		Options       options;
		Processor     processor( options );
		wstring       inputText;
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );

		inputText = L"A\nB\nC";
		input.str( inputText );

		processor.setOutStream( output );
		processor.processStream( input );

		outputText = output.str();

		Assert::IsTrue( inputText == outputText );
	}



	/**
	** @brief Test processing macro definitions (macro without an argument).
	**
	** @code
	** #define MACRO A
	** MACRO
	** MACRO()
	** @endcode
	*/
	[TestMethod]
	void processDefine1Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define MACRO A" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );

		processor.setOutStream( output );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText.empty() );

		input.str( L"MACRO" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		input.str( L"MACRO()" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A()" );
	}


	/**
	** @brief  Test processing macro definitions (macro with an empty argument list).
	**
	** @code
	** #define MACRO() A
	** MACRO
	** MACRO()
	** MACRO ()
	** MACRO <comment> ()
	** @endcode
	*/ 
	[TestMethod]
	void processDefine2Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define MACRO() A" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );

		processor.setOutStream( output );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();

		Assert::IsTrue( outputText.empty() );

		// Macro defined with parameter but used without arguements --> Don't expand.
		input.str( L"MACRO" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"MACRO" );

		// Macro used without arguements --> expand.
		input.str( L"MACRO()" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		// Macro used without arguements --> expand.
		input.str( L"MACRO ()" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		// Macro used with comment between name and arguements --> expand.
		input.str( L"MACRO /* comment */ ()" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		// Macro used with comments in argument list --> expand.
		input.str( L"MACRO ( /* comment */ )" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		// Macro used with new line in argument list --> expand.
		input.str( L"MACRO (\r\n)" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		// Macro used with multi line comments in argument list --> expand.
		input.str( L"MACRO (\r\n/*\r\n comment \r\n*/\r\n)" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );
	}
	/*
	** Test processing macro definitions (macro with one argument).
	** @code
	** #define MACRO(X) A
	** MACRO(X)
	** @endcode
	*/
	[TestMethod]
	void processDefine3Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define MACRO(X) A" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );

		processor.setOutStream( output );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText.empty() );

		output.str(wstring());
		input.str( L"MACRO(X)" );
		input.clear();
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		// clear the output buffer.
		output.str(wstring());
	}

	/**
	** @brief Test processing macro definitions (macro with several arguments).
	**
	** @code
	** #define MACRO(A,B,C) C B A
	** MACRO
	** MACRO(1,2,3)
	** MACRO ( 1 <comment>, 2, 3 )
	** MACRO <comment> ( 1, 2, 3 )
	** @endcode
	*/ 
	[TestMethod]
	void processDefine4Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define MACRO(A, B, C) C B A" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );

		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText.empty() );

		input.str( L"MACRO" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"MACRO" );

		input.str( L"MACRO(1,2,3)" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"3 2 1" );

		input.str( L"MACRO( 1 /* comment */ , 2, 3 )" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"3 2 1" );

		input.str( L"MACRO /* comment */ ( 1 , 2 , 3 )" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"3 2 1" );

		// clear the output buffer.
		output.str(wstring());
	}


	/**
	** @brief Define and use Macro (recursive definition).
	**
	** @code
	** #define A B
	** #define B A
	** A
	** B
	** @endcode
	*/ 
	[TestMethod]
	void processDefine5Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define A B\n#define B A" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );

		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"\n" );

		input.str( L"A" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A" );

		input.str( L"B" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"B" );

		input.str( L"A\nB" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A\nB" );

	}


	/**
	** @brief Define macro with trailing noise.
	**
	** @code
	** #define A // noise
	** #define B /\* noise *\/
	** #define C [blank]
	** #define D [blank] /\* noise *\/ [blank] 
	** A
	** B
	** C
	** D
	** @endcode
	*/ 
	[TestMethod]
	void processDefine6Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstring       inputText;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );


		inputText = L"#define A // noise\n"
					L"#define B /* noise */\n"
					L"#define C \n"
					L"#define D  /* noise */\t \t /* even more noise */ \t \n"
					L"A\n"
					L"B\n"
					L"C\n"
					L"D\n";

		input.str( inputText );
		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"" );
	}


	/**
	** @brief Define and use Macro defined with parenthesis but not as an argument list.
	*/ 
	[TestMethod]
	void processDefine7Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstring       inputText;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );


		inputText = L"#define A 1 | 2\n"
		         L"#define B (A) << 1\n"
					L"B";

		input.str( inputText );
		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"(1 | 2) << 1" );
	}



	/**
	** @brief  Test processing macro definitions (macro with an empty argument list).
	**
	** @code
	** #define MACRO() A
	** MACRO
	** MACRO()
	** @endcode
	*/ 
	[TestMethod]
	void processDefine8Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define MACRO() A" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );

		processor.setOutStream( output );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();

		Assert::IsTrue( outputText.empty() );

		// Don't expand if first occurence (usage without parentheses)
		// but the second.
		input.str( L"MACRO\nMACRO()" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"MACRO\nA" );
	}

	/**
	** @brief  Test processing macro definitions (macro with an empty argument list).
	**
	** @code
	** #define MACRO( U ) -- Comment1\
	** U
	** MACRO( X )
	** @endcode
	*/ 
	[TestMethod]
	void processDefine9Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define MACRO( U ) -- Comment1\\\nU" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );

		processor.setOutStream( output );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();

		Assert::IsTrue( outputText.empty() );

		// Don't expand if first occurence (usage without parentheses)
		// but the second.
		input.str( L"MACRO(X)" );
		input.clear();
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"X" );
	}


	/**
	** @brief Define and undefine some macros then check that the macros are undefined.
	**
	** @code
	** #define M1 A
	** #define M2 B
	** #define M3 C
	** #undefall M
	** #if defined( M1 ) || defined( M2 ) || defined( M3 ) 
	** ERROR
	** #else
	** OK
	** #endif
	** @endcode
	*/ 
	[TestMethod]
	void processUndefall1Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstring       inputText;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );


		inputText = L"#define M1 A\n"
					L"#define M2 B\n"
					L"#define M3 C\n"
					L"#undefall M\n"
					L"#if defined( M1 ) || defined( M2 ) || defined( M3 )\n"
					L"ERROR\n"
					L"#else\n"
					L"OK\n"
					L"#endif\n";

		input.str( inputText );
		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );
	}

	[TestMethod]
	void processUndefall2Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstring       inputText;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );


		inputText = L"#define MA1 A1\n"
					L"#define MA2 A2\n"
					L"#define MA3 A3\n"
					L"#define MB1 B1\n"
					L"#define MB2 B2\n"
					L"#define MB3 B3\n"
					L"#undefall MA\n"
					L"#if defined( MA1 ) || defined( MA2 ) || defined( MA3 ) \\\n"
					L" || !defined( MB1 ) || !defined( MB2 ) || !defined( MB3 )\n"
					L"ERROR\n"
					L"#else\n"
					L"OK\n"
					L"#endif\n";

		input.str( inputText );
		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );
	}


	/**
	** @brief Define and use Macro (recursive definition).
	**
	** @code
	** #define M A\
	** B\
	**
	** M
	** @endcode
	*/ 
	[TestMethod]
	void processMultiLineDefineTest()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input( L"#define M A\\\nB\\\n\nM" );
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );

		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A\nB\n" );
	}

	/**
	** @brief Check \#ifdef MACRO.
	**
	** @code
	** #ifdef MACRO
	** #error MACRO not defined.
	** #else 
	** OK
	** #endif
	** #define MACRO
	** #ifdef MACRO
	** OK
	** #else 
	** #error MACRO is defined.
	** #endif
	** @endcode
	**
	** @example ProcessorProcessIfdef1Test.h
	*/ 
	[TestMethod]
	void processIfdef1Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );

		input << L"#ifdef MACRO\n"
			  << L"MACRO is not defined.\n"
			  << L"#else\n"
			  << L"OK\n"
			  << L"#endif\n"
			  << L"OK";


		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\nOK" );
	}

	/**
	** @brief Check \#ifdef MACRO.
	**
	** @code
	** #ifdef MACRO
	** #error MACRO not defined.
	** #else 
	** OK
	** #endif
	** #define MACRO
	** #ifdef MACRO
	** OK
	** #else 
	** #error MACRO is defined.
	** #endif
	** @endcode
	**
	** @example ProcessorProcessIfndef1Test.h
	*/ 
	[TestMethod]
	void processIfndef1Test()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );

		input << L"#ifdef MACRO\n"
			  << L"MACRO is not defined.\n"
			  << L"#else\n"
			  << L"OK\n"
			  << L"#endif\n"
			  << L"OK";


		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\nOK" );
	}



	/**
	** @brief Check conditional evaluation.
	**
	*/
	[TestMethod]
	void processIfTest()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );
		options.expandMacroArguments( false );
		processor.setOutStream( output );

		input << L"#if 1\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

		input.str( wstring() );
		input.clear();
		input << L"#if 1 /* comment */\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

		input.str( wstring() );
		input.clear();
		input << L"#if 1 == 1\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

		input.str( wstring() );
		input.clear();
		input << L"#if 1 == 0\n"
			  << L"ERROR\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"" );

		input.str( wstring() );
		input.clear();
		input << L"#if 0\n"
			  << L"ERROR\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"" );

		input.str( wstring() );
		input.clear();
		input << L"#define MACRO\n"
			  << L"#if defined( MACRO )\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

		input.str( wstring() );
		input.clear();
		input << L"#if defined MACRO\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );


		input.str( wstring() );
		input.clear();
		input << L"#if defined FUCK\n"
			  << L"ERROR\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"" );


		input.str( wstring() );
		input.clear();
		input << L"#define A 1\n"
			  << L"#define B 2\n"
			  << L"#define X B\n"
			  << L"#if X == A\n"
			  << L"ERROR\n"
			  << L"#endif\n"
			  << L"#if X == B\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

	}

	/**
	** @brief Check conditional evaluation.
	**
	*/
	[TestMethod]
	void processElifTest()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( true );
		options.eliminateEmptyLines( true );
		options.expandMacroArguments( false );
		processor.setOutStream( output );

		input.str( wstring() );
		input.clear();
		input << L"#if 0\n"
			  << L"ERROR\n"
			  << L"#elif 1\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );


		input.str( wstring() );
		input.clear();
		input << L"#if 0\n"
			  << L"ERROR 0\n"
			  << L"#elif 1\n"
			  << L"OK\n"
			  << L"#elif 2\n"
			  << L"ERROR 2\n"
			  << L"#else\n"
			  << L"ERROR ELSE\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

		input.str( wstring() );
		input.clear();
		input << L"#if 0\n"
			  << L"ERROR\n"
			  << L"#elif 0\n"
			  << L"ERROR\n"
			  << L"#elif 1\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );


		input.str( wstring() );
		input.clear();
		input << L"#define A 1\n"
			  << L"#define B 2\n"
			  << L"#define C 3\n"
			  << L"#define X B\n"
			  << L"#if X == A\n"
			  << L"ERROR\n"
			  << L"#elif X == B\n"
			  << L"OK\n"
			  << L"#elif X == C\n"
			  << L"ERROR\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

		input.str( wstring() );
		input.clear();
		input << L"#undef X\n"
			  << L"#define X C /* now X is C */\n"
			  << L"#if X == A\n"
			  << L"ERROR\n"
			  << L"#elif X == B\n"
			  << L"ERROR\n"
			  << L"#elif X == C\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

		input.str( wstring() );
		input.clear();
		input << L"#undef C\n"
			  << L"#define C 3 /* X is still C */\n"
			  << L"#if X == A\n"
			  << L"ERROR\n"
			  << L"#elif X == B\n"
			  << L"ERROR\n"
			  << L"#elif X == C\n"
			  << L"OK\n"
			  << L"#endif";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"OK\n" );

	}




	/*
	** @brief Check usage of the concate operator (\#\#).
	**
	** The check is done without expanding macros found in a macro argument list.
	**
	** @code
	** #define CONCATE A ## B
	** #define CONCATE_ARGS ( DO, CONCATE ) DO ## CONCATE
	**
	** CONCATE
	** CONCATE_ARGS( A, B )
	** CONCATE_ARGS( CONCATE, C )
	** CONCATE_ARGS( CONCATE_ARGS, C )
	** CONCATE_ARGS( CONCATE_ARGS( A, B ), C )
	** @endcode
	**
	** @exampe input/concate.h
	**
	*/
	[TestMethod]
	void concateOperatorTest()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );
		options.expandMacroArguments( false );

		input << L"#define CONCATE A ## B\n"
			  << L"#define CONCATE_ARGS( DO, CONCATE ) DO ## CONCATE\n"
			  << L"CONCATE";

		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"AB" );

		input.str( L"CONCATE_ARGS( A, B )" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"AB" );

		input.str( L"CONCATE_ARGS( CONCATE, C )" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"ABC" );

		input.str( L"CONCATE_ARGS( CONCATE_ARGS, C )" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"CONCATE_ARGSC" );

		input.str( L"CONCATE_ARGS( CONCATE_ARGS( A, B ), C )" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"ABC" );
	}

	/**
	** @brief Check usage of the concate operator (\#\@).
	*/
	[TestMethod]
	void charizeOperatorTest()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );
		options.expandMacroArguments( false );
		processor.setOutStream( output );

		input << L"#define CHARIZE(A) #@A\n"
			  << L"CHARIZE( X )";

		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"'X'" );

		input.clear();
		input << L"#define CHARIZE_2(A) print 'Hello' + #@A\n"
			  << L"CHARIZE_2( World )";
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"print 'Hello' + 'World'" );
	}

	/*
	** @brief Check usage of the concate operator (\#\#).
	**
	** The check is done without expanding macros found in a macro argument list.
	**
	** @code
	** #define MACRO A \
	**               B
	** MACRO
	** @endcode
	**
	*/
	[TestMethod]
	void multiLineMacroTest()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );
		options.expandMacroArguments( false );
		options.multiLineMacroExpansion( true );

		input << L"#define MACRO A\\\n"
			  << L"              B\n"
			  << L"MACRO";
		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A\n              B" );

		options.multiLineMacroExpansion( false );
		input.str( L"MACRO" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"A B" );
	}

	/*
	** @brief Check usage of the concate operator (\#\#).
	**
	** The check is done without expanding macros found in a macro argument list.
	**
	** @code
	** #define MACRO1( A, B  ) A #A B
	** #define MACRO2( C ) MACRO1( X, C )
	** 
	** @endcode
	**
	*/
	[TestMethod]
	void nestedMacroTest()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );
		options.expandMacroArguments( false );
		options.multiLineMacroExpansion( true );

		input << L"#define MACRO1( A, B  ) A B\n"
			  << L"#define MACRO2( A ) MACRO1( A, X )\n";

		processor.setOutStream( output );
		output.str(wstring());
		processor.processStream( input );

		input.str( L"MACRO2( U )" );
		input.clear();
		output.str(wstring());
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"U X" );
	}


	/*
	** @brief Check Support of range option.
	**
	** Check that only for the the selected input output is emitted.
	*/
	[TestMethod]
	void rangeTest1()
	{
		Options       options;
		Processor     processor( options );
		wstringstream input;
		wstringstream output;
		wstring       outputText;

		options.emitLine( false );
		options.keepComments( false );
		options.eliminateEmptyLines( true );
		options.expandMacroArguments( false );
		options.multiLineMacroExpansion( true );

		input << L"ABC DEF GHI" << ends;
		processor.setOutStream( output );

		options.setOutputRange( Range(0,3) );
		output.str( wstring() );
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"ABC" );

		options.setOutputRange( Range(3,4) );
		output.str( wstring() );
		input.clear();
		input.seekg(0);
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L" " );

		options.setOutputRange( Range(4,7) );
		output.str( wstring() );
		input.clear();
		input.seekg(0);
		processor.processStream( input );
		outputText = output.str();
		Assert::IsTrue( outputText == L"DEF" );

	}

}; // class

} // namespace test
} // namespace sqtpp

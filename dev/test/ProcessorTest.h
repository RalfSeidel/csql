#ifndef SQTPP_PROCESSOR_TEST_H
#define SQTPP_PROCESSOR_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.Processor
*/
class ProcessorTest : public Test
{
public:
	ProcessorTest();
	~ProcessorTest() throw() {}

	/// Execute all tests.
	static void run();


	/// Process text without any preprocessor stuff.
	void processTextTest();

	/// Test processing macro definitions (macro without an argument).
	void processDefine1Test();

	/// Test processing macro definitions (macro with an empty argument list).
	void processDefine2Test();

	/// Test processing macro definitions (macro with one argument).
	void processDefine3Test();

	/// Test processing macro definitions (macro with several arguments).
	void processDefine4Test();

	/// Define and use Macro (recursive definition).
	void processDefine5Test();

	/// Define and use Macro (recursive definition).
	void processDefine6Test();

	/// Define and use Macro defined with parenthesis but not as an argument list.
	void processDefine7Test();

	/// Repro for a bug. Macro without arguments did not expand...
	void processDefine8Test();

	/// Repro for a bug. Macro without arguments did not expand...
	void processDefine9Test();

	/// Check processing of the #undefall directive.
	void processUndefall1Test();

	/// Check processing of the #undefall directive.
	void processUndefall2Test();

	/// Define and use Macro exceeding one line.
	void processMultiLineDefineTest();

	/// Check macro definition.
	void processIfdef1Test();

	/// Check macro definition.
	void processIfndef1Test();

	/// Check conditional evalualtion
	void processIfTest();

	/// Check conditional evalualtion
	void processElifTest();

	/// Check conditional evalualtion 
	void processUnmatchedConditional();

	/// Check usage of the concate operator (\#\#).
	void concateOperatorTest();

	/// Check usage of the concate operator (\#\@).
	void charizeOperatorTest();

	/// Check definition of macros extceeding one line.
	void multiLineMacroTest();

	/// Check tested macro usage.
	void nestedMacroTest();
};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_PROCESSOR_TEST_H

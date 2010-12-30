#include "stdafx.h"
#include "Macro.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class MacroTest : public TestBase
{
public:
	/**
	** @brief Run all defined tests for this class.
	*/
	[TestMethod]
	void MacroTest::run()
	{
		wclog << L"MacroTest" << endl;
		MacroTest test;
	}

}; // class

} // namespace test
} // namespace sqtpp

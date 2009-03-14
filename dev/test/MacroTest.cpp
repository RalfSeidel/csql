#include "stdafx.h"
#include "../Macro.h"
#include "MacroTest.h"

namespace sqtpp {
namespace test {

MacroTest::MacroTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void MacroTest::run()
{
	wclog << L"MacroTest" << endl;
	MacroTest test;
}

} // namespace test
} // namespace sqtpp

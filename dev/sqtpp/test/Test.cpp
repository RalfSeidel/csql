#include "stdafx.h"
#include "CmdArgsTest.h"
#include "BuildinTest.h"
#include "ExpressionTest.h"
#include "ConvertTest.h"
#include "DirectiveTest.h"
#include "ErrorTest.h"
#include "FileTest.h"
#include "MacroTest.h"
#include "ProcessorTest.h"
#include "ScannerTest.h"
#include "StreamsTest.h"
#include "TokenTest.h"
#include "NStringStreamTest.h"
#include "Test.h"


namespace sqtpp {
namespace test {

void Test::run()
{
	CmdArgsTest::run();
	ProcessorTest::run();
	ScannerTest::run();
	BuildinTest::run();
	ExpressionTest::run();
	AnsiFileBufferTest::run();
	UtfFileBufferTest::run();
	AnsiFileStreamTest::run();
	UtfFileStreamTest::run();
	ConvertTest::run();
	DirectiveTest::run();
	ErrorTest::run();
	FileTest::run();
	MacroTest::run();
	TokenTest::run();
#if defined( _MANAGED )
	NStringStreamTest::run();
#endif
}

} // namespace test
} // namespace sqtpp

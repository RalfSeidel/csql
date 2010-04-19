#include "stdafx.h"
#include <strstream>
#include "Options.h"
#include "CmdArgs.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

public ref class CmdArgsTest : TestBase 
{
public:

	/**
	** @brief Test the evaluation of the -c argument
	*/
	[TestMethod]
	void commentsOptionTest()
	{
		const int argc = 2;
		const wchar_t* argv[] = { L"sqtpp.exe", L"/c" };
		Options options;
		CmdArgs cmdArgs( argc, argv );
		// For the test purposes let the command line parser
		// ignore that we don't specify any input file.
		cmdArgs.ignoreMissingArgs( true );

		cmdArgs.parse( options );
		Assert::IsTrue( options.keepBlockComments() );
		Assert::IsTrue( options.keepLineComments() );
		Assert::IsTrue( options.keepSqlComments() );

		argv[1] = L"/c-b+";
		cmdArgs.parse( options );
		Assert::IsTrue( options.keepBlockComments() );
		Assert::IsTrue( !options.keepLineComments() );
		Assert::IsTrue( !options.keepSqlComments() );

		argv[1] = L"/cbl-s+";
		cmdArgs.parse( options );
		Assert::IsTrue( !options.keepBlockComments() );
		Assert::IsTrue( !options.keepLineComments() );
		Assert::IsTrue( options.keepSqlComments() );
	}

}; // class

} // namespace test
} // namespace sqtpp

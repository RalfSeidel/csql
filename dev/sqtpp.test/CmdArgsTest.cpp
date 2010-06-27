#include "stdafx.h"
#include <strstream>
#include "Options.h"
#include "CmdArgs.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
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

	[TestMethod]
	void norangeOptionTest()
	{
		const int argc = 1;
		const wchar_t* argv[] = { L"sqtpp.exe" };
		Options options;
		CmdArgs cmdArgs( argc, argv );

		cmdArgs.ignoreMissingArgs( true );
		cmdArgs.parse( options );

		const Range& outputRange = options.getOutputRange();
		Assert::IsTrue( outputRange.isEmpty() );
	}


	[TestMethod]
	void rangeOptionTest()
	{
		const int argc = 2;
		const wchar_t* argv[] = { L"sqtpp.exe", L"/r10-20" };
		Options options;
		CmdArgs cmdArgs( argc, argv );

		cmdArgs.ignoreMissingArgs( true );
		cmdArgs.parse( options );

		const Range& outputRange = options.getOutputRange();
		Assert::IsTrue( outputRange.getStartIndex() == 10 );
		Assert::IsTrue( outputRange.getEndIndex() == 20 );
	}

}; // class

} // namespace test
} // namespace sqtpp

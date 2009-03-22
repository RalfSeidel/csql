#include "stdafx.h"
#include <strstream>
#include "../Options.h"
#include "../CmdArgs.h"
#include "CmdArgsTest.h"

namespace sqtpp {
namespace test {

CmdArgsTest::CmdArgsTest()
{
}

CmdArgsTest::~CmdArgsTest() throw()
{
}


/**
** @brief Run all defined tests for this class.
*/
void CmdArgsTest::run()
{
	CmdArgsTest test;

	test.commentsOptionTest();
}

/**
** @brief Test the evaluation of the -c argument
*/
void CmdArgsTest::commentsOptionTest()
{
	const int argc = 2;
	const wchar_t* argv[] = { L"sqtpp.exe", L"/c" };
	Options options;
	CmdArgs cmdArgs( argc, argv );
	// For the test purposes let the command line parser
	// ignore that we don't specify any input file.
	cmdArgs.ignoreMissingArgs( true );

	cmdArgs.parse( options );
	assert( options.keepBlockComments() );
	assert( options.keepLineComments() );
	assert( options.keepSqlComments() );


	argv[1] = L"/c-b+";
	cmdArgs.parse( options );
	assert( options.keepBlockComments() );
	assert( !options.keepLineComments() );
	assert( !options.keepSqlComments() );

	argv[1] = L"/cbl-s+";
	cmdArgs.parse( options );
	assert( !options.keepBlockComments() );
	assert( !options.keepLineComments() );
	assert( options.keepSqlComments() );
}


} // namespace test
} // namespace sqtpp

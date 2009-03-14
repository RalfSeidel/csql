#ifndef SQTPP_CMD_ARGS_TEST_H
#define SQTPP_CMD_ARGS_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.CmdArgs
** 
** @todo Currently no test is implemented.
*/
class CmdArgsTest : public Test
{
public:

	CmdArgsTest();
	~CmdArgsTest() throw();

	/// Execute all tests.
	static void run();

	/// Test the evaluation of the -c argument
	void commentsOptionTest();

};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_CMD_ARGS_TEST_H

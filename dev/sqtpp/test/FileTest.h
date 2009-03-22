#ifndef SQTPP_FILE_TEST_H
#define SQTPP_FILE_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Test routines for the class sqtpp::File.
*/
class FileTest : public Test
{
public:
	FileTest() {}
	~FileTest() throw() {}

	/// Execute all tests.
	static void run();

	/// Execute all tests.
	void setUp();
	/// Execute all tests.
	void tearDown();

	/// Test getLine
	void getLineTest();

	/// Test static methode isFile()
	void isFileTest();

	/// Test findFile
	void findFileTest();

};

} // namespace test
} // namespace sqtpp


#endif // SQTPP_FILE_TEST_H

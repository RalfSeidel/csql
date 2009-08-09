#ifndef SQTPP_STREAMS_TEST_H
#define SQTPP_STREAMS_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {


/**
** @brief Tests for the class sqtpp.Streams
*/
class AnsiFileBufferTest : public Test
{
public:
	/// Constructor
	AnsiFileBufferTest();
	/// Destructor
	~AnsiFileBufferTest() throw() {}

	/// Execute all tests.
	static void run();

	/// Test openeing a stream.
	void openTest();
	/// Test character fetching.
	void sbumpcTest();
	/// Test character fetching.
	void uflowTest();
	/// Test absolute positioning.
	void seekposTest();
	/// Test relative positioning.
	void seekoffTest();
	/// Test putting back a character.
	void sputbackTest();
	/// Test writing a string with sputn.
	void writeStringTest();

private:

};

/**
** @brief Tests for the class sqtpp.Streams
*/
class AnsiFileStreamTest : public Test
{
public:
	/// Constructor
	AnsiFileStreamTest();
	/// Destructor
	~AnsiFileStreamTest() throw() {}

	/// Execute all tests.
	static void run();

	/// Test peek test
	void peekTest();
};


/**
** @brief Tests for the class sqtpp::UtfFileBuffer
*/
class UtfFileBufferTest : public Test
{
public:
	/// Constructor
	UtfFileBufferTest();
	/// Destructor
	~UtfFileBufferTest() throw() {}

	/// Execute all tests.
	static void run();

	/// Test openeing a stream.
	void openTest();
	/// Test character fetching.
	void sbumpcTest();
	/// Test character fetching.
	void uflowTest();
	/// Test absolute positioning.
	void seekposTest();
	/// Test relative positioning.
	void seekoffTest();
	/// Test putting back a character.
	void sputbackTest();
	/// Read a file with non ansi file name and content.
	void readRussiaTest();

	/// Test writing a string with sputn.
	void writeStringTest();

private:

};


} // namespace test
} // namespace sqtpp



#endif // SQTPP_STREAMS_TEST_H

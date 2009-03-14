#ifndef SQTPP__TEST_H
#define SQTPP__TEST_H
#if _MSC_VER > 10
#pragma once
#endif


namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.
*/
class Test
{
public:
	Test() {}
	~Test() throw() {}

	static void run();


	virtual void setUp() {}
	virtual void tearDown() {}

};



} // namespace test
} // namespace sqtpp



#endif // SQTPP__TEST_H

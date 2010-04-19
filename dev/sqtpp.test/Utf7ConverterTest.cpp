#include "stdafx.h"
#include "CodePageConverter.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class Utf7ConverterTest : public TestBase
{
public:

	/**
	** @brief Test for is_start_byte method
	*/
	[TestMethod]
	void isStartByteTest()
	{
		bool is_start_byte;
		char c;

		c = '+';
		is_start_byte = Utf7Converter::is_start_byte( c );
		Assert::IsTrue( is_start_byte );
	}

	/**
	** @brief Test for length method
	*/
	[TestMethod]
	void lengthTest()
	{
	}


	/**
	** @brief Test for in method
	*/
	[TestMethod]
	void inTest()
	{
	}

	/**
	** @brief Test for in method
	*/
	[TestMethod]
	void outTest()
	{
	}

}; // class

} // namespace test
} // namespace sqtpp

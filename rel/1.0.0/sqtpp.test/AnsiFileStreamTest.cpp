#include "stdafx.h"
#include "File.h"
#include "Streams.h"
#include "TestBase.h"

#undef GetCurrentDirectory

namespace sqtpp {
namespace test {

typedef basic_fstream<char> AnsiFileStream;


[TestClass]
public ref class AnsiFileStreamTest : public TestBase
{
public:
	[TestMethod]
	void AnsiFileStreamTest::peekTest()
	{
		AnsiFileStream stream;
		wstring directory = TestBase::TestFileDirectory::get();
		wstring filePath  = directory + L"ansiabc.txt";
		stream.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( stream.good() );
		AnsiFileStream::int_type chExpected = 'A';
		AnsiFileStream::int_type chFound;

		for ( int i = 0; i < 26; ++i ) {
			chFound = stream.peek();
			Assert::IsTrue( chFound == chExpected );
			chFound = stream.get();
			Assert::IsTrue( chFound == chExpected );
			++chExpected;
		}
		chFound = stream.peek();
		Assert::IsTrue( chFound == AnsiFileStream::traits_type::eof() );
		Assert::IsTrue( stream.eof() );
	}
}; // class


} // namespace test
} // namespace sqtpp

#include "StdAfx.h"
#include "File.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class FileTest : public TestBase
{
public:
	[TestMethod]
	void getLineTest()
	{
		wstringstream input( L"Line 1\nLine 2\nLine 3\n" );
		File          file;

		Assert::IsTrue( file.getLine() == 0 );
		Assert::IsTrue( file.getColumn() == 0 );

		file.attach( input );
		Assert::IsTrue( file.getLine() == 1 );
		Assert::IsTrue( file.getColumn() == 1 );

		file.incLine();
		Assert::IsTrue( file.getLine() == 2 );
		Assert::IsTrue( file.getColumn() == 1 );

		file.incColumn( 1 );
		Assert::IsTrue( file.getLine() == 2 );
		Assert::IsTrue( file.getColumn() == 2 );

		file.incColumn( 8 );
		Assert::IsTrue( file.getLine() == 2 );
		Assert::IsTrue( file.getColumn() == 10 );

		file.incLine();
		Assert::IsTrue( file.getLine() == 3 );
		Assert::IsTrue( file.getColumn() == 1 );
	}


	[TestMethod]
	void isFileTest()
	{
		wstring directory = TestFileDirectory;
		wstring filePath  = directory + L"buildin.h";
		Assert::IsTrue( File::isFile( filePath.c_str() ) );
	}

}; // class



} // namespace test
} // namespace sqtpp


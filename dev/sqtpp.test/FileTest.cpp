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

	[TestMethod]
	void findFileInCurrentDirectoryTest()
	{
		vector<wstring> includeDirectories;
		File            file;
		wstring         path;

		wstring directory = TestFileDirectory;
		wstring filePath  = directory + L"include1.h";
		file.open( filePath.c_str() );

		// Check that include is not resolved if current directory is not searched.
		path = file.findFile( L"include2.h", includeDirectories, false );
		Assert::IsTrue( path.empty() );
		
		// Check that include is resolved if current directory is not searched.
		path = file.findFile( L"include2.h", includeDirectories, true );
		Assert::IsTrue( !path.empty() );
	}

	/**
	** @brief Check that include is resolved if current directory is not searched but 
	** specified in the include directory collection.
	*/
	[TestMethod]
	void findFileInIncludedDirectoriesTest()
	{
		vector<wstring> includeDirectories;
		File            file;
		wstring         path;

		wstring directory = TestFileDirectory;
		includeDirectories.push_back( directory );
		path = file.findFile( L"include2.h", includeDirectories, false );
		Assert::IsTrue( !path.empty() );
	}

}; // class



} // namespace test
} // namespace sqtpp


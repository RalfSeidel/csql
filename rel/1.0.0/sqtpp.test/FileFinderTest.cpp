#include "stdafx.h"
#include "TestBase.h"
#include "FileFinder.h"

namespace sqtpp {
namespace test {
[TestClass]
public ref class FileFinderTest : public TestBase
{
public: 

	[TestMethod]
	void findFileInCurrentDirectoryTest1()
	{
		vector<wstring> includeDirectories;

		// Check that include is not resolved if current directory is not searched.
		FileFinder finder( includeDirectories );
		wstring path = finder.findFile( L"include2.h" );
		Assert::IsTrue( path.empty() );
	}

	[TestMethod]
	void findFileInCurrentDirectoryTest2()
	{
		vector<wstring> includeDirectories;
		wstring directory = TestFileDirectory;
		wstring filePath  = directory + L"include1.h";

		// Check that include is resolved if current directory is searched.
		FileFinder finder( includeDirectories, filePath  );
		wstring path = finder.findFile( L"include2.h" );
		Assert::IsFalse( path.empty() );
	}

	[TestMethod]
	void findNonExistingFileTest()
	{
		std::vector<std::wstring> includeDirectories;
		FileFinder ff( includeDirectories );
		std::wstring result = ff.findFile( L"Non Existing File" );
		bool isEmpty = result.empty();
		Assert::IsTrue( isEmpty );
	};

	[TestMethod]
	void findFileInIncludeDirectoriesTest()
	{
		std::vector<std::wstring> includeDirectories;
		std::wstring directory = TestFileDirectory;
		includeDirectories.push_back( directory );
		FileFinder ff( includeDirectories );
		std::wstring result = ff.findFile( L"ansiabc.txt" );
		bool isEmpty = result.empty();
		Assert::IsFalse( isEmpty );
	};


	[TestMethod]
	void findFileRelativeToCurrentFileTest1()
	{
		std::vector<std::wstring> includeDirectories;
		std::wstring filePath = TestFileDirectory + L"special/findfile.h";
		FileFinder ff( includeDirectories, filePath );
		std::wstring result = ff.findFile( L"include/findfile_include.h" );
		bool isEmpty = result.empty();
		Assert::IsFalse( isEmpty );
	};

	[TestMethod]
	void findFileRelativeToCurrentFileTest2()
	{
		std::vector<std::wstring> includeDirectories;
		std::wstring filePath = TestFileDirectory + L"special/include/findfile_include.h";
		FileFinder ff( includeDirectories, filePath );
		std::wstring result = ff.findFile( L"../findfile.h" );
		bool isEmpty = result.empty();
		Assert::IsFalse( isEmpty );
	};

	[TestMethod]
	void notExistingFileRelativeToCurrentTest()
	{
		std::vector<std::wstring> includeDirectories;
		std::wstring filePath = TestFileDirectory + L"special/findfile.h";
		FileFinder ff( includeDirectories, filePath );
		std::wstring result = ff.findFile( L"not_existing_include.h" );
		bool isEmpty = result.empty();
		Assert::IsTrue( isEmpty );
	};
};

} // namespace test
} // namespace sqtpp

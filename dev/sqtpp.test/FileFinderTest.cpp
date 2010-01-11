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
		void TestFindNonExistingFile()
		{
			/*
			std::vector<std::wstring> includeDirectories;
			FileFinder ff( includeDirectories );
			std::wstring result = ff.findFile( L"Non Existing File" );
			bool isEmpty = result.empty();
			Assert::IsTrue( isEmpty );
			*/
		};
	};

} // namespace test
} // namespace sqtpp

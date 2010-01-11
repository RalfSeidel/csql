#pragma once
#using <mscorlib.dll>
#include "DllInit.h"
using namespace System;
using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

namespace sqtpp {
namespace test {

[TestClass]
public ref class TestBase
{
private:
	static TestBase();

	TestContext^ m_testContext;

public:
	TestBase();
	virtual ~TestBase();

	/// <summary>
	///Gets or sets the test context which provides
	///information about and functionality for the current test run.
	///</summary>
	property Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ TestContext
	{
		Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ get();
		System::Void set( Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ value );
	};


	[AssemblyInitialize]
	static void AssemblyInitialize( Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ testContext );

	[AssemblyCleanup]
	static void AssemblyCleanup();

	[ClassInitialize]
	static void ClassInitialize( Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ testContext );

	[ClassCleanup]
	static void ClassCleanup();

	void LogCurrentTest();
};

} // namespace UnitTest
} // namespace CDbLib


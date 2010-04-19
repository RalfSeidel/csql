#include "StdAfx.h"
#include "TestBase.h"
#using <mscorlib.dll>

using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

namespace sqtpp {
namespace test {

/**
** \brief Initialize the class
*/
static TestBase::TestBase()
{
	// CrtDllInit::Initialize();
}


/**
** \brief Initialize the instance.
**
*/
TestBase::TestBase()
{
	CrtDllInit::Initialize();
}

/**
** \brief Destroy the instance.
**
*/
TestBase::~TestBase()
{
	CrtDllInit::Cleanup();
}

/**
** \brief Get the current test context.
*/
TestContext^ TestBase::TestContext::get()
{
	return m_testContext;
}
System::Void TestBase::TestContext::set( Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ value )
{
	m_testContext = value;
}

/**
** \brief Get the current test context.
*/
wstring TestBase::TestFileDirectory::get()
{
	return L"..\\..\\..\\IntegrationTest\\files\\sqtpp\\input\\";
}


/**
** \brief Initialize the assembly.
**
*/
void TestBase::AssemblyInitialize( Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ )
{
	CrtDllInit::Initialize();
}

/**
** \brief Cleanup the assembly.
**
*/
void TestBase::AssemblyCleanup()
{
	CrtDllInit::Cleanup();
}

/**
** \brief Initialize the assembly.
**
*/
void TestBase::ClassInitialize( Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ )
{
	CrtDllInit::Initialize();
}

/**
** \brief Cleanup the assembly.
**
*/
void TestBase::ClassCleanup()
{
	CrtDllInit::Cleanup();
}



/**
** \brief Trace the execution of the current test.
*/
void TestBase::LogCurrentTest()
{
	using namespace System::Diagnostics;

    StackTrace^ st = gcnew StackTrace(true);
	if ( st->FrameCount > 1 ) {
        StackFrame^ sf = st->GetFrame(1);
		Console::WriteLine("{0}.{1}", sf->GetMethod()->ReflectedType->Name, sf->GetMethod()->Name );
	}
}

} // namespace test
} // namespace sqtpp

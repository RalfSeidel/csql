#include "stdafx.h"
#include "NStringStream.h"
#include "TestBase.h"
#using <System.dll>
using namespace System;
using namespace System::Diagnostics;

namespace sqtpp {
namespace test {

[TestClass]
[Ignore]
public ref class NStringStreamTest : public TestBase
{
public:
	/**
	** @brief Test NStringStream::setString.
	*/
	[TestMethod]
	void setStringTest()
	{
		NStringStream  ns;
		const wchar_t* pwszInput = L"Hello world";

		ns.setString( gcnew System::String( pwszInput ) );

		size_t index = 0;
		for (;;) {
			wchar_t wc = ns.get();
			if ( ns.eof() )
				break;

			Debug::Assert( true );
			Debug::Assert( wc == pwszInput[index] );
			++index;
		}
		Debug::Assert( index == wcslen( pwszInput ) );


		ns.setString( nullptr );
		ns.clear();
		ns.get();
		Debug::Assert( ns.eof() );

		ns.setString( System::String::Empty );
		ns.clear();
		ns.get();
		Debug::Assert( ns.eof() );
	}

	/**
	** @brief Test NStringStream::getString.
	*/
	[TestMethod]
	void getStringTest()
	{
		NStringStream  ns;
		const wchar_t* pwszInput = L"Hello world";

		ns << pwszInput;

		System::String^ nstring = ns.getString();

		Debug::Assert( nstring == gcnew System::String( pwszInput )  );
	}

}; // class

} // namespace test
} // namespace sqtpp

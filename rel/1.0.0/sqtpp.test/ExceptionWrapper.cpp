/*!\file
** \brief Helper to translate native exception thrown by old fashioned 
**        libraries into managed code exception.
**
** © 2010 by SQL Service GmbH
*/
#include "stdafx.h"
#include "ExceptionWrapper.h"
using namespace System;

namespace sqtpp {
namespace test {
namespace Interop {

StlException::StlException( String^ message )
: System::Exception( message )
{
}

void StlException::Throw( const std::exception& stdException  )
{
	const char* pszMessage = stdException.what();
	String^ message = gcnew String( pszMessage );
	StlException^ managedException = gcnew StlException( message );
	throw managedException;
}



void throwException()
{
	System::Exception^ pEx = gcnew System::Exception( "test" );
	throw pEx;
}


} // namespace Interop
} // namespace test
} // namespace sqtpp

// Entry points into the managed code.
#pragma managed(push, on)

static void throwStlException( const std::exception&  stlException )
{
	sqtpp::test::Interop::StlException::Throw( stlException );
}


#pragma managed(pop)

#pragma managed(push, off)

static const DWORD MS_MAGIC      = 0x19930520;
static const DWORD CPP_EXCEPTION = 0xE06D7363; 

/**
**
**
** See <a href="http://www.ddj.com/windows/184416600">http://www.ddj.com/windows/184416600</a> for 
** more information about this function.
*/
extern "C" void __stdcall _CxxThrowException( void* pObject, _s__ThrowInfo const * pObjectInfo )
{
    const ULONG_PTR args[] = { MS_MAGIC
                             , (ULONG_PTR)pObject
                             , (ULONG_PTR)pObjectInfo };
	try {
		RaiseException(CPP_EXCEPTION, EXCEPTION_NONCONTINUABLE, sizeof(args)/sizeof(args[0]), args);
	}
	catch ( const std::exception& stlException ) {
		throwStlException( stlException );
	}
}


#pragma managed(pop)


	
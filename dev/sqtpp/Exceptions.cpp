#include "stdafx.h"
#include "Exceptions.h"

namespace sqtpp {

// --------------------------------------------------------------------
// Runtime Error
// --------------------------------------------------------------------

RuntimeError::RuntimeError()
: base( "" )
{
}

RuntimeError::RuntimeError( const char* message )
: base( message )
{
}

RuntimeError::~RuntimeError() throw()
{
}

// --------------------------------------------------------------------
// Logic Error
// --------------------------------------------------------------------

LogicError::LogicError()
: base( "" )
{
}

LogicError::LogicError( const char* message )
: base( message )
{
}

// --------------------------------------------------------------------
// Unexpected switch error
// --------------------------------------------------------------------

UnexpectedSwitchError::UnexpectedSwitchError()
: base( "Unexpected switch value." )
{
}

UnexpectedSwitchError::UnexpectedSwitchError( const char* pszMessage )
: base( pszMessage )
{
}

// --------------------------------------------------------------------
// Not supported / implemented error.
// --------------------------------------------------------------------

NotSupportedError::NotSupportedError(void)
: base( "The operation is not supported" )
{
}
} // namespace


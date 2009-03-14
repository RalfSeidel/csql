#include "stdafx.h"
#include "Exceptions.h"
#include "Context.h"

namespace sqtpp {



// --------------------------------------------------------------------
// ContextInfo
// --------------------------------------------------------------------



ContextInfo::ContextInfo( Context context, const wchar_t* const pwszSymbol, const wchar_t* const pwszDescription )
: context( context )
, pwszSymbol( pwszSymbol )
, pwszDescription( pwszDescription )
{
}

const ContextInfo& ContextInfo::getContextInfo( Context /* context */ )
{
	throw NotSupportedError();
}



} // namespace sqtpp

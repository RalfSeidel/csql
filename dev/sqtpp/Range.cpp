#include "stdafx.h"
#include "Range.h"

namespace sqtpp {


Range::Range() throw()
{
	nStartIndex = nEndIndex = size_t(-1);
}

Range::Range( size_t nFromChar, size_t nToChar ) throw()
{
	this->nStartIndex = nFromChar;
	this->nEndIndex = nToChar;
}

bool Range::isOverlapping( const Range& that ) const throw()
{
	if ( this->nEndIndex <= that.nStartIndex )
		return false;

	if ( this->nStartIndex >= that.nEndIndex )
		return false;

	return true;
}

} // namespace

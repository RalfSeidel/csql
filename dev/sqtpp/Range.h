/**
** @file
** @author Ralf Seidel
** @brief Declaration of the Range structure.
**
** © 2004-2010 by SQL Service GmbH, Wuppertal.
*/
#ifndef SQTPP_RANGE_H
#define SQTPP_RANGE_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

	/**
	** @brief Range in the input file to emit output for.
	*/
	struct Range
	{
	private:
		size_t nStartIndex;
		size_t nEndIndex;
	public:

		Range() throw();

		Range( size_t nFromChar, size_t nToChar ) throw();

		size_t getStartIndex() const throw()
		{
			return this->nStartIndex;
		}

		size_t getEndIndex() const throw()
		{
			return this->nEndIndex;
		}

		bool isEmpty() const throw()
		{
			return nStartIndex < 0 || nEndIndex <= nStartIndex;
		}

		bool isWithin( size_t nCharPosition )
		{
			return nStartIndex <= nCharPosition && nCharPosition < nEndIndex;
		}

		/// Check if the this range is overlapping the other range.
		bool isOverlapping( const Range& that ) const throw();
	};
}

#endif // SQTPP_RANGE_H


#ifndef SQTPP_LOCATION_H
#define SQTPP_LOCATION_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

class File;

/** 
** @brief Information about a location.
*/
class Location
{
private:
	size_t  m_nLine;
	wstring m_sFile;
public:
	Location( const File& file );

	size_t getLine() const throw()         { return m_nLine; }
	const wstring& getFile() const throw() { return m_sFile; }

};

class LocationStack : public std::stack<Location>
{
public:
	const container_type& container()
	{
		return c;
	}


};

} // namespace


#endif SQTPP_LOCATION_H

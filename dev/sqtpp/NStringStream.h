#ifndef SQTPP_NSTRINGSTREAM_H
#define SQTPP_NSTRINGSTREAM_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

/**
** @brief A STL stream for .NET strings.
**
** The class simply extends the std::wstringstream with two methods
** for converting the c++ string from and to .NET system string.
*/
class NStringStream : public std::wstringstream
{
private:
	typedef std::wstringstream base;

public:
	explicit NStringStream( ios_base::openmode mode = ios_base::in | ios_base::out );
	explicit NStringStream( gcroot<System::String^> nstring, ios_base::openmode mode = ios_base::in | ios_base::out );

	System::String^ getString() const;
	void setString( System::String^ string );
};


} // namespace sqtpp

#endif // SQTPP_NSTRINGSTREAM_H

#include "StdAfx.h"
#include "NStringStream.h"

namespace sqtpp {

/**
** @brief Default constructor.
** 
** The default construction just goes through to the base implementation.
** 
** @param mode Sets the read / write mode.
*/
NStringStream::NStringStream( ios_base::openmode mode /* = ios_base::in | ios_base::out */ )
: base( mode )
{
}

/**
** @brief Initialising constructor.
** 
** The default construction just goes through to the base implementation.
** 
** @param mode    Sets the read / write mode.
** @param nstring The string to process.
*/
NStringStream::NStringStream( gcroot<System::String^> nstring, ios_base::openmode mode /* = ios_base::in | ios_base::out */ )
: base( mode )
{
	setString( nstring );
}

/**
** @brief Get the string.
*/
System::String^ NStringStream::getString() const
{
	std::wstring    stdstr  = base::str();
	System::String^ nstring;;
	if ( stdstr.empty() ) {
		nstring = System::String::Empty;
	} else {
		nstring = gcnew System::String( stdstr.c_str() );
	}
	return nstring;
}

/**
** @brief Set the string.
*/
void NStringStream::setString( System::String^ nstring )
{
	if ( !nstring || nstring->Length == 0 ) {
		base::str( std::wstring() );
	} else {
		const int       length     = nstring->Length;
		wchar_t*        pwszString = (wchar_t*)alloca( (length + 1) * sizeof( wchar_t ) );
		for ( int i = 0; i < length; ++i ) {
			wchar_t wchar = (*nstring)[i];
			pwszString[i] = wchar;
		}
		pwszString[length] = L'\0';
		base::str( std::wstring(pwszString) );
	}
}


} // namespace sqtpp

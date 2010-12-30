#include "StdAfx.h"
#ifdef _WIN32
#include <Windows.h>
#endif
#include "Exceptions.h"
#include "Util.h"
#include "Streams.h"

namespace sqtpp 
{

	
// --------------------------------------------------------------------
// UtfFileBuffer
// --------------------------------------------------------------------

/**
** @brief Default constructor.
*/
UtfFileBuffer::UtfFileBuffer()
: base()
, m_currentChar( traits_type::eof() )
, m_bBigEndian( false )
{
}


/**
** @brief Open the file.
*/
UtfFileBuffer* UtfFileBuffer::open( const wchar_t* pszFileName, ios_base::openmode openMode, int protFlags /* = (int)ios_base::_Openprot */ )
{
	// Non binary (CRLF translation) is not supported by this stream.
	// assert( (openMode & ios_base::binary) == ios_base::binary );
	openMode|= ios_base::binary;

#	if _MSC_VER >= 1400
	InnerBuffer* pFileBuffer = m_fileBuffer.open( pszFileName, openMode, protFlags );
#	else
	std::string sFileName = Convert::wcs2str( pszFileName );
	InnerBuffer* pFileBuffer = m_fileBuffer.open( sFileName.c_str(), openMode, protFlags );
#	endif
	if ( pFileBuffer == NULL ) {
		return NULL;
	}

	if ( (openMode & ios_base::out) != 0 ) {
		// Write Unicode tag if file is empty.
		InnerBuffer::pos_type curpos = m_fileBuffer.pubseekoff( 0, ios_base::cur, ios_base::out );
		if ( curpos == InnerBuffer::pos_type(0) ) {
			Chars chars = { '\xff', '\xfe' };
			m_fileBuffer.sputn( (const char*)&chars, sizeof( chars ) );
		}
	} else if ( (openMode & ios_base::in) != 0 ) {
		Chars      chars;
		streamsize length = m_fileBuffer.sgetn( (char*)&chars, 2 );

		if ( length != 2 ) {
			m_fileBuffer.close();
			return NULL;
		}

		if ( chars.char1 == '\xff' && chars.char2 == '\xfe' ) {
			m_bBigEndian = false;
		} else if ( chars.char1 == '\xfe' && chars.char2 == '\xff' ) {
			m_bBigEndian = true;
		} else {
			// Not UTF file.
			m_fileBuffer.close();
			string msg;//( convert(wstring(pszFileName)) );
			msg+= "Not a unicode file";
			throw RuntimeError( msg.c_str() );
		}
	} else {
		// Neither in nor out specified.
		assert( false );
		return NULL;
	}

	return this;
}

/**
** @brief Close the file.
*/
UtfFileBuffer* UtfFileBuffer::close()
{
	InnerBuffer* pFileBuffer = m_fileBuffer.close();

	return pFileBuffer != NULL ? this : NULL;
}


/**
** @brief Convert two characters read from the 8 bit stream into a wide character.
**
** @param char1 The first character read from the 8 bit stream.
** @param char2 The second character read from the 8 bit stream.
** @return The unicode character.
*/
wchar_t UtfFileBuffer::c2w( char char1, char char2 )  const throw()
{
	wchar_t wchar;

	if ( m_bBigEndian ) {
		wchar = (wchar_t)((unsigned char)char1 << 8 | (unsigned char)char2);
	} else {
		wchar = (wchar_t)((unsigned char)char2 << 8 | (unsigned char)char1);
	}
	
	return wchar;
}

/**
** @brief Convert a wide character into two 8 bit characters.
**
** @param wchar The unicode character.
** @return Two 8 bit characters to be put into the 8 bit stream:
**         char[0] first, char[1] second.
*/
UtfFileBuffer::Chars UtfFileBuffer::w2c( wchar_t wchar ) const throw()
{
	Chars chars;
	if ( wchar == traits_type::eof() ) {
		chars.char2 = (char)InnerBuffer::traits_type::eof();
		chars.char1 = (char)InnerBuffer::traits_type::eof();
	} else if ( m_bBigEndian ) {
		chars.char2 = char(wchar & 0x00FF);
		chars.char1 = char((wchar & 0xFF00) >> 8);
	} else {
		chars.char1 = char(wchar & 0x00FF);
		chars.char2 = char((wchar & 0xFF00) >> 8);
	}
	return chars;
}


/**
** @brief Synchronize with file content.
** 
** @returns If the function cannot succeed, it returns -1. The default behavior is to return zero.
*/
int UtfFileBuffer::sync()
{
	return m_fileBuffer.pubsync();
}

/**
** @brief Fetch a single character and advance current read position.
** 
** @returns The character fetched.
*/
UtfFileBuffer::int_type UtfFileBuffer::uflow()
{
	wchar_t result = traits_type::eof();

	if ( m_currentChar != traits_type::eof() ) {
		result = m_currentChar;
		m_fileBuffer.pubseekoff( 2, ios_base::cur, ios_base::in );
	} else {
		char char1 = (char)m_fileBuffer.sbumpc();
		if ( char1 == InnerBuffer::traits_type::eof() ) {
			result = traits_type::eof();
		} else {
			char char2 = (char)m_fileBuffer.sbumpc();
			if ( char2 != InnerBuffer::traits_type::eof() ) {
				result = c2w( char1, char2 );
			} else {
				result = traits_type::eof();
				m_fileBuffer.pubseekoff( -1, ios_base::cur, ios_base::in );
			}
		}
	}
	// No current character any longer.
	m_currentChar = traits_type::eof();

	return (int_type)result;
}

/**
** @brief Get current character / last character fetched.
**
*/
UtfFileBuffer::int_type UtfFileBuffer::underflow()
{
	if ( m_currentChar == traits_type::eof() ) {
#		ifdef _DEBUG
		InnerBuffer::pos_type oldPos = m_fileBuffer.pubseekoff( 0, ios_base::cur, ios::in );
		InnerBuffer::pos_type newPos;
#		endif

		char char1 = (char)m_fileBuffer.sbumpc();

		if ( char1 == InnerBuffer::traits_type::eof() ) {
			m_currentChar = traits_type::eof();
		} else {
			char char2 = (char)m_fileBuffer.sbumpc();
			if ( char2 == InnerBuffer::traits_type::eof() ) {
				m_currentChar = traits_type::eof();
				m_fileBuffer.pubseekoff( -1, ios_base::cur, ios_base::in );
			} else {
				m_currentChar   = c2w( char1, char2 );
#				ifdef _DEBUG
					newPos = m_fileBuffer.pubseekoff( 0, ios_base::cur, ios_base::in );
					assert( newPos - oldPos == 2 );
					newPos = m_fileBuffer.pubseekoff( -2, ios_base::cur, ios_base::in );
					assert( oldPos - newPos == 0 );
#				else 
				m_fileBuffer.pubseekoff( -2, ios_base::cur, ios_base::in );
#				endif
			}
		}
	}
	return m_currentChar;
}


/**
** @brief Put a character into the stream.
** 
** @returns If the function cannot succeed, it returns -1. The default behavior is to return zero.
*/
UtfFileBuffer::int_type UtfFileBuffer::overflow( int_type character )
{
	if ( character == traits_type::eof() ) {
		// can't reserve buffer
		assert( false );
		return traits_type::eof();
	} else {
		Chars chars = w2c( wchar_t(character) );
		streamsize rc = m_fileBuffer.sputn( (const char*)&chars, 2 );
		return rc == InnerBuffer::traits_type::eof() ? traits_type::eof() : int_type(character);
	}
}



/**
** @brief Put back a character.
*/
UtfFileBuffer::int_type UtfFileBuffer::pbackfail( int_type meta /* = traits_type::eof() */ )
{
	int_type result;

	pos_type newPos = this->pubseekoff( -1, ios_base::cur, ios_base::in );

	if ( newPos == streampos(_BADOFF) ) {
		result = traits_type::eof();
	} else {
		// get character at current read position.
		result = underflow();
		if ( result != traits_type::eof() ) {
			if ( meta == traits_type::eof() ) {
				// Ok
			} else if ( meta != result ) {
				// Can't putback abritary characters.
				result = traits_type::eof();
			}
		}
	}

	return result;
}


/**
** @brief Alter the current positions of the inner streams.
*/
UtfFileBuffer::pos_type UtfFileBuffer::seekpos( pos_type position, ios_base::openmode which /*= ios_base::in | ios_base::out */)
{
	InnerBuffer::pos_type innerPos = position * 2 + 2;

	innerPos = m_fileBuffer.pubseekpos( innerPos, which );

	m_currentChar = traits_type::eof();

	return innerPos == streampos(_BADOFF) ? streampos(_BADOFF) : position;
}

/**
** @brief Alter the current positions of the inner streams.
*/
UtfFileBuffer::pos_type UtfFileBuffer::seekoff( off_type offset, ios_base::seekdir direction,  ios_base::openmode which /* = ios_base::in | ios_base::out */ )
{
	pos_type position;
	if ( direction == ios_base::beg ) {
		position = seekpos( offset, which );
	} else {
		InnerBuffer::off_type innerOffset   = offset * 2;
		InnerBuffer::pos_type innerPosition = m_fileBuffer.pubseekoff( innerOffset, direction, which );

		if ( innerPosition < streampos(2)|| innerPosition == streampos(_BADOFF) ) {
			position = streampos(_BADOFF);
		} else {
			position = (innerPosition - streampos(2)) / 2;
		}
		m_currentChar = traits_type::eof();
	}
	return position;
}

#ifdef _WIN32

// --------------------------------------------------------------------
// WinDebugOutBuffer
// --------------------------------------------------------------------

WinDebugOutBuffer::WinDebugOutBuffer()
{
	memset( m_buffer, 0, sizeof( m_buffer ) );
	m_buffer[sizeof( m_buffer ) / sizeof( wchar_t ) - 1] = L'\0';
	setp( m_buffer, &m_buffer[sizeof( m_buffer ) / sizeof( wchar_t ) - 2] );
}

/**
** @brief Flush the buffer.
*/
int WinDebugOutBuffer::sync()
{
	size_t writeCount = this->pptr() - this->pbase();
	if (  writeCount != 0 ) {
		m_buffer[writeCount] = 0;
		::OutputDebugStringW( m_buffer );
		pbump( -(int)writeCount );
	}
	return 0;
}

/**
** @brief Put a character into the stream.
*/
WinDebugOutBuffer::int_type WinDebugOutBuffer::overflow( int_type character )
{
	if ( character != traits_type::eof() ) {
		wchar_t buffer[2] = { character, L'\0' };
		::OutputDebugStringW( buffer );
	}
	return traits_type::not_eof(character);
}

/**
** @brief Write a string to the debug out console.
*/
streamsize WinDebugOutBuffer::xsputn( const wchar_t* buffer, streamsize count )
{
	if ( count != streamsize(0) ) {
		::OutputDebugStringW( buffer );
	}
	return count;
}

// --------------------------------------------------------------------
// WinDebugOutStream
// --------------------------------------------------------------------
WinDebugOutStream::WinDebugOutStream()
: base( &m_buffer )
{
}

#endif

} // namespace sqtpp

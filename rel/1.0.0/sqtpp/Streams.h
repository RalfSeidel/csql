/**
** @file
** @author Ralf Seidel
** @brief Various streams and stream buffer.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_STREAMS_H
#define SQTPP_STREAMS_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp 
{

/**
** @brief A stream buffer for sequential reading and writting unicode files.
**
** This file buffer quiet simple encapsulates a standard file buffer by
** translating each wchar_t operation into standard file operation affecting
** two 8 bit characters.
*/
class UtfFileBuffer : public std::basic_streambuf<wchar_t>
{
private:
	// Base class type definition.
	typedef std::basic_streambuf<wchar_t> base;
	// Inner stream buffer type definition.
	typedef std::basic_filebuf<char>      InnerBuffer;

	InnerBuffer m_fileBuffer;

	// The last character fetched by the uflow operation.
	wchar_t     m_currentChar;

	// Flag indicating if the file processed uses big endian conventions (CP 1201).
	bool        m_bBigEndian;

	/**
	** @brief A pair of two 8 bit characters used for character conversion.
	*/
	struct Chars
	{
		char char1;
		char char2;
	};
public:
	// Initialising constructor.
	UtfFileBuffer();
private:
	// Copy constructor (Not implemented).
	UtfFileBuffer( const UtfFileBuffer& that );
	// Assignment operator (Not implemented).
	UtfFileBuffer& operator= ( const UtfFileBuffer& that );

public:

	// Open the file.
	UtfFileBuffer* open( const wchar_t* pszFileName, ios_base::openmode openMode, int protFlags = (int)ios_base::_Openprot );

	// Close the file.
	UtfFileBuffer* close();

	// Check if the file processed is a big endian (UTF-16BE / CP 1201) file.
	bool isBigEndian() const throw() { return m_bBigEndian; }

	// Check if the file processed is a little endian (UTF-16 / CP 1200) file.
	bool isLittleEndian() const throw() { return !m_bBigEndian; }

protected:
	// Synchronize with file content.
	virtual int sync();

	// Get last character fetched.
	virtual int_type underflow();

	// Put a character into the stream.
	virtual int_type overflow( int_type character = traits_type::eof() );

	// Fetch characters into the read buffer.
	virtual int_type uflow();

	// Put back a character.
	virtual int_type pbackfail( int_type meta = traits_type::eof() );

	// Alter the current positions of the inner streams,
	virtual pos_type seekpos( pos_type position, ios_base::openmode which = ios_base::in | ios_base::out );

	// Alter the current positions of the inner streams,
	virtual pos_type seekoff( off_type offset, ios_base::seekdir direction,  ios_base::openmode hhich = ios_base::in | ios_base::out );

	// Read characters into the read buffer.
	//virtual streamsize xsgetn( wchar_t* buffer, streamsize count );

	// Read characters into the write buffer.
	//virtual streamsize xsputn( wchar_t* buffer, streamsize count );
private:
	// Convert two characters read from the 8 bit stream into a wide character.
	wchar_t c2w( char char1, char char2 )  const throw();

	// Convert a wide character into two 8 bit characters.
	Chars w2c( wchar_t wchar ) const throw();
};

#ifdef _WIN32

/**
** @brief A stream buffer to facilitate the windows OutputDebugString trace function.
*/
class WinDebugOutBuffer : public std::basic_streambuf<wchar_t>
{
private:
	// Base class type definition.
	typedef std::basic_streambuf<wchar_t> base;

	// A output buffer for character operations.
	wchar_t m_buffer[512];
public:
	// Constructor.
	WinDebugOutBuffer();

private:
	// Copy constructor (Not implemented).
	WinDebugOutBuffer( const WinDebugOutBuffer& that );
	// Assignment operator (Not implemented).
	WinDebugOutBuffer& operator= ( const WinDebugOutBuffer& that );

public:
	// Flush the output buffer.
	virtual int sync();

	// Put a character into the stream.
	virtual int_type overflow( int_type character = traits_type::eof() );

	// Put a character into the stream.
	virtual streamsize xsputn( const wchar_t* buffer, streamsize count );
};


/**
** @brief A stream to facilitate the windows OutputDebugString trace function.
*/
class WinDebugOutStream : public std::wostream
{
private:
	typedef public std::wostream base;

	// The stream buffer.
	WinDebugOutBuffer m_buffer;

public:
	// Constructor.
	WinDebugOutStream();

private:
	// Copy constructor (Not implemented).
	WinDebugOutStream( const WinDebugOutStream& that );
	// Assignment operator (Not implemented).
	WinDebugOutStream& operator= ( const WinDebugOutStream& that );
};


#endif // _WIN32

} // namespace sqtpp

#endif // SQTPP_STREAMS_H

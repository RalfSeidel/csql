#include "stdafx.h"
#include "../File.h"
#include "../Streams.h"

#include "StreamsTest.h"

namespace sqtpp {
namespace test {


// --------------------------------------------------------------------
// AnsiFileBufferTest
// --------------------------------------------------------------------

typedef basic_filebuf<char> AnsiFileBuffer;
typedef basic_fstream<char> AnsiFileStream;

AnsiFileBufferTest::AnsiFileBufferTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void AnsiFileBufferTest::run()
{
	AnsiFileBufferTest test;

	wclog << L"AnsiFileBufferTest" << endl;

	if ( !File::isDirectory( L"test/cmd/input" ) ) {
		wclog << L"Checks skiped because the test input directory does not exist." << endl;
	} else {
		test.openTest();
		test.sbumpcTest();
		test.uflowTest();
		test.seekposTest();
		test.seekoffTest();
		test.sputbackTest();
	}
	if ( !File::isDirectory( L"test/cmd/sqtppout" ) ) {
		wclog << L"Tests skiped because the test output directory does not exist." << endl;
	} else {
		test.writeStringTest();
	}
}


void AnsiFileBufferTest::openTest()
{
	AnsiFileBuffer buffer;
	AnsiFileBuffer* pBuffer = buffer.open( "test/cmd/input/ansiabc.txt", ios_base::in );
	assert( pBuffer != NULL );

	buffer.close();
}

void AnsiFileBufferTest::sbumpcTest()
{
	AnsiFileBuffer buffer;
	AnsiFileBuffer* pBuffer = buffer.open( "test/cmd/input/ansiabc.txt", ios_base::in );
	assert( pBuffer != NULL );

	AnsiFileBuffer::int_type ch;

	// Note: snextc skips the first character
	ch = buffer.sbumpc();
	assert( ch == 'A' );

	ch = buffer.sbumpc();
	assert( ch == 'B' );

	// skip until Z
	for ( int i = 0; i < 23; ++i ) {
		ch = buffer.sbumpc();
	}

	ch = buffer.sbumpc();
	assert( ch == 'Z' );

	ch = buffer.sbumpc();
	assert( ch == AnsiFileBuffer::traits_type::eof() );
}


void AnsiFileBufferTest::uflowTest()
{
	AnsiFileBuffer buffer;
	AnsiFileBuffer* pBuffer = buffer.open( "test/cmd/input/ansiabc.txt", ios_base::in );
	assert( pBuffer != NULL );

	AnsiFileBuffer::int_type ch;

	// Note: snextc skips the first character
	ch = buffer.snextc();
	assert( ch == 'B' );

	ch = buffer.sgetc();
	assert( ch == 'B' );

	ch = buffer.snextc();
	assert( ch == 'C' );

	ch = buffer.snextc();
	assert( ch == 'D' );

	ch = buffer.sbumpc();
	assert( ch == L'D' );

	ch = buffer.sbumpc();
	assert( ch == L'E' );
}

void AnsiFileBufferTest::seekposTest()
{
	AnsiFileBuffer buffer;
	AnsiFileBuffer* pBuffer = buffer.open( "test/cmd/input/ansiabc.txt", ios_base::in );
	assert( pBuffer != NULL );
	AnsiFileBuffer::int_type ch;

	buffer.pubseekpos( 25 );
	ch = buffer.sgetc();
	assert( ch == 'Z' );

	buffer.pubseekpos( 0 );
	ch = buffer.sgetc();
	assert( ch == 'A' );

	buffer.pubseekpos( 1 );
	ch = buffer.sgetc();
	assert( ch == 'B' );

	buffer.pubseekpos( 24 );
	ch = buffer.sgetc();
	assert( ch == 'Y' );
}

void AnsiFileBufferTest::seekoffTest()
{
	AnsiFileBuffer buffer;
	AnsiFileBuffer* pBuffer = buffer.open( "test/cmd/input/Ansiabc.txt", ios_base::in );
	assert( pBuffer != NULL );
	AnsiFileBuffer::int_type ch;

	ch = buffer.sgetc();
	assert( ch == 'A' );

	buffer.pubseekoff( 25, ios_base::cur );
	ch = buffer.sgetc();
	assert( ch == 'Z' );

	buffer.pubseekoff( -24, ios_base::cur );
	ch = buffer.sgetc();
	assert( ch == 'B' );

	buffer.pubseekoff( 1, ios_base::cur );
	ch = buffer.sgetc();
	assert( ch == 'C' );

	buffer.pubseekoff( 22, ios_base::cur );
	ch = buffer.sgetc();
	assert( ch == 'Y' );

	buffer.pubseekoff( 0, ios_base::beg );
	ch = buffer.sgetc();
	assert( ch == 'A' );

	buffer.pubseekoff( -1, ios_base::end );
	ch = buffer.sgetc();
	assert( ch == 'Z' );
}


void AnsiFileBufferTest::sputbackTest()
{
	AnsiFileBuffer buffer;
	AnsiFileBuffer* pBuffer = buffer.open( "test/cmd/input/ansiabc.txt", ios_base::in );
	assert( pBuffer != NULL );
	AnsiFileBuffer::int_type ch;

	ch = buffer.sgetc();
	assert( ch == 'A' );

	ch = buffer.snextc();
	assert( ch == 'B' );

	ch = buffer.sungetc();
	assert( ch == 'A' );

	ch = buffer.sgetc();
	assert( ch == 'A' );

	ch = buffer.sbumpc();
	assert( ch == 'A' );

	ch = buffer.sgetc();
	assert( ch == 'B' );
}


void AnsiFileBufferTest::writeStringTest()
{
	AnsiFileBuffer buffer;
	const char* pszFileName = "test/cmd/sqtppout/ansiabc.txt";
	const char* pszString   = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	AnsiFileBuffer* pBuffer = buffer.open( pszFileName, ios_base::out | ios_base::trunc );
	assert( pBuffer != NULL );
	streamsize length = streamsize( strlen( pszString ) );
	pBuffer->sputn( pszString, length );
	pBuffer->close();
}


// --------------------------------------------------------------------
// AnsiFileStreamTest
// --------------------------------------------------------------------

AnsiFileStreamTest::AnsiFileStreamTest()
{
}

void AnsiFileStreamTest::run()
{
	AnsiFileStreamTest test;

	wclog << L"AnsiFileStreamTest" << endl;

	if ( !File::isDirectory( L"test/cmd/input" ) ) {
		wclog << L"Checks skiped because the test input directory does not exist." << endl;
	} else {
		test.peekTest();
	}
}

void AnsiFileStreamTest::peekTest()
{
	AnsiFileStream stream;
	stream.open( "test/cmd/input/ansiabc.txt", ios_base::in );
	assert( stream.good() );
	AnsiFileStream::int_type chExpected = 'A';
	AnsiFileStream::int_type chFound;

	for ( int i = 0; i < 26; ++i ) {
		chFound = stream.peek();
		assert( chFound == chExpected );
		chFound = stream.get();
		assert( chFound == chExpected );
		++chExpected;
	}
	chFound = stream.peek();
	assert( chFound == AnsiFileStream::traits_type::eof() );
	assert( stream.eof() );
}


// --------------------------------------------------------------------
// UtfFileBufferTest
// --------------------------------------------------------------------

UtfFileBufferTest::UtfFileBufferTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void UtfFileBufferTest::run()
{
	UtfFileBufferTest test;

	wclog << L"UtfFileBufferTest" << endl;

	if ( !File::isDirectory( L"test/cmd/input" ) ) {
		wclog << L"Checks skiped because the test input directory does not exist." << endl;
	} else {
		test.openTest();
		test.sbumpcTest();
		test.uflowTest();
		test.seekposTest();
		test.seekoffTest();
		test.sputbackTest();
#		if _MSC_VER >= 1400
		test.readRussiaTest();
#		endif
	}
	if ( !File::isDirectory( L"test/output" ) ) {
		wclog << L"Tests skiped because the test output directory does not exist." << endl;
	} else {
		test.writeStringTest();
	}
}


void UtfFileBufferTest::openTest()
{
	UtfFileBuffer buffer;
	UtfFileBuffer* pBuffer = buffer.open( L"test/cmd/input/UTF16.txt", ios_base::in );
	assert( pBuffer != NULL );
	assert( !buffer.isBigEndian() );

	buffer.close();

	pBuffer = buffer.open( L"test/cmd/input/UTF16BE.txt", ios_base::in );
	assert( pBuffer != NULL );
	assert( buffer.isBigEndian() );

	buffer.close();
}

void UtfFileBufferTest::sbumpcTest()
{
	struct Impl_ {
	void operator()( const wchar_t* pwszFileName )
	{
		UtfFileBuffer buffer;
		UtfFileBuffer* pBuffer = buffer.open( pwszFileName, ios_base::in );
		assert( pBuffer != NULL );
		wchar_t wchar;

		// Note: snextc skips the first character
		wchar = buffer.sbumpc();
		assert( wchar == 'A' );

		wchar = buffer.sbumpc();
		assert( wchar == 'B' );


		// skip until Z
		for ( int i = 0; i < 23; ++i ) {
			wchar = buffer.sbumpc();
		}

		wchar = buffer.sbumpc();
		assert( wchar == 'Z' );

		wchar = buffer.sbumpc();
		assert( wchar == UtfFileBuffer::traits_type::eof() );

		buffer.close();
	}} impl;

	impl( L"test/cmd/input/utfabc.txt" );
	impl( L"test/cmd/input/utfbeabc.txt" );
}


void UtfFileBufferTest::uflowTest()
{
	struct Impl_ {
	void operator()( const wchar_t* pwszFileName )
	{
		UtfFileBuffer buffer;
		UtfFileBuffer* pBuffer = buffer.open( pwszFileName, ios_base::in );
		assert( pBuffer != NULL );
		wchar_t wchar;


		wchar = buffer.snextc();
		assert( wchar == L'B' );

		wchar = buffer.sgetc();
		assert( wchar == L'B' );

		wchar = buffer.snextc();
		assert( wchar == L'C' );

		wchar = buffer.snextc();
		assert( wchar == L'D' );

		wchar = buffer.sbumpc();
		assert( wchar == L'D' );

		wchar = buffer.sbumpc();
		assert( wchar == L'E' );

		buffer.close();
	}} impl;

	impl( L"test/cmd/input/utfabc.txt" );
	impl( L"test/cmd/input/utfbeabc.txt" );
}

void UtfFileBufferTest::seekposTest()
{
	UtfFileBuffer buffer;
	UtfFileBuffer* pBuffer = buffer.open( L"test/cmd/input/utfabc.txt", ios_base::in );
	assert( pBuffer != NULL );
	wchar_t wc;

	buffer.pubseekpos( 25 );
	wc = buffer.sgetc();
	assert( wc == L'Z' );

	buffer.pubseekpos( 0 );
	wc = buffer.sgetc();
	assert( wc == L'A' );

	buffer.pubseekpos( 1 );
	wc = buffer.sgetc();
	assert( wc == L'B' );

	buffer.pubseekpos( 24 );
	wc = buffer.sgetc();
	assert( wc == L'Y' );
}

void UtfFileBufferTest::seekoffTest()
{
	UtfFileBuffer buffer;
	UtfFileBuffer* pBuffer = buffer.open( L"test/cmd/input/utfabc.txt", ios_base::in );
	assert( pBuffer != NULL );
	wchar_t wc;

	wc = buffer.sgetc();
	assert( wc == L'A' );

	buffer.pubseekoff( 25, ios_base::cur );
	wc = buffer.sgetc();
	assert( wc == L'Z' );

	buffer.pubseekoff( -24, ios_base::cur );
	wc = buffer.sgetc();
	assert( wc == L'B' );

	buffer.pubseekoff( 1, ios_base::cur );
	wc = buffer.sgetc();
	assert( wc == L'C' );

	buffer.pubseekoff( 22, ios_base::cur );
	wc = buffer.sgetc();
	assert( wc == L'Y' );

	buffer.pubseekoff( 0, ios_base::beg );
	wc = buffer.sgetc();
	assert( wc == L'A' );

	buffer.pubseekoff( -1, ios_base::end );
	wc = buffer.sgetc();
	assert( wc == L'Z' );
}


void UtfFileBufferTest::sputbackTest()
{
	UtfFileBuffer buffer;
	UtfFileBuffer* pBuffer = buffer.open( L"test/cmd/input/utfabc.txt", ios_base::in );
	assert( pBuffer != NULL );
	wchar_t wc;

	wc = buffer.sgetc();
	assert( wc == L'A' );

	wc = buffer.snextc();
	assert( wc == L'B' );

	wc = buffer.sungetc();
	assert( wc == L'A' );

	wc = buffer.sgetc();
	assert( wc == L'A' );

	wc = buffer.sbumpc();
	assert( wc == L'A' );

	wc = buffer.sgetc();
	assert( wc == L'B' );
}

void UtfFileBufferTest::readRussiaTest()
{
	UtfFileBuffer buffer;
	const wchar_t* pszFileName = L"test/cmd/input/\x0420\x043E\x0441\x0441\x0438\x044F.txt";
	UtfFileBuffer* pBuffer = buffer.open( pszFileName, ios_base::in );
	assert( pBuffer != NULL );
	wchar_t wc;

	wc = buffer.sbumpc();
	assert( wc == L'\x0420' );

	wc = buffer.sbumpc();
	assert( wc == L'\x043E' );
	
	wc = buffer.sbumpc();
	assert( wc == L'\x0441' );
	
	wc = buffer.sbumpc();
	assert( wc == L'\x0441' );
	
	wc = buffer.sbumpc();
	assert( wc == L'\x0438' );
	
	wc = buffer.sbumpc();
	assert( wc == L'\x044F' );

	wc = buffer.sbumpc();
	assert( wc == UtfFileBuffer::traits_type::eof() );
}

void UtfFileBufferTest::writeStringTest()
{
	UtfFileBuffer buffer;
	const wchar_t* pszFileName = L"test/output/utfabc.txt";
	const wchar_t* pszString   = L"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	UtfFileBuffer* pBuffer = buffer.open( pszFileName, ios_base::out | ios_base::trunc );
	assert( pBuffer != NULL );

	streamsize length = streamsize( wcslen( pszString ) );
	pBuffer->sputn( pszString, length );
	pBuffer->close();

	// Russia
	pszFileName = L"test/output/russia.txt";
	pszString   = L"\x0420\x043E\x0441\x0441\x0438\x044F";
	pBuffer = buffer.open( pszFileName, ios_base::out | ios_base::trunc );
	assert( pBuffer != NULL );

	length = streamsize( wcslen( pszString ) );
	pBuffer->sputn( pszString, length );
	pBuffer->close();
}




} // namespace test
} // namespace sqtpp

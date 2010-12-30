#include "stdafx.h"
#include "File.h"
#include "Streams.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {


// --------------------------------------------------------------------
// AnsiFileBufferTest
// --------------------------------------------------------------------

typedef basic_filebuf<char> AnsiFileBuffer;

[TestClass]
public ref class AnsiFileBufferTest : public TestBase
{
public:

	[TestMethod]
	void openTest()
	{
		AnsiFileBuffer buffer;
		wstring filePath = getAnsiabcFilePath();
		AnsiFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );

		buffer.close();
	}

	[TestMethod]
	void sbumpcTest()
	{
		AnsiFileBuffer buffer;
		wstring filePath = getAnsiabcFilePath();
		AnsiFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );

		AnsiFileBuffer::int_type ch;

		// Note: snextc skips the first character
		ch = buffer.sbumpc();
		Assert::IsTrue( ch == 'A' );

		ch = buffer.sbumpc();
		Assert::IsTrue( ch == 'B' );

		// skip until Z
		for ( int i = 0; i < 23; ++i ) {
			ch = buffer.sbumpc();
		}

		ch = buffer.sbumpc();
		Assert::IsTrue( ch == 'Z' );

		ch = buffer.sbumpc();
		Assert::IsTrue( ch == AnsiFileBuffer::traits_type::eof() );
	}


	[TestMethod]
	void uflowTest()
	{
		AnsiFileBuffer buffer;
		wstring filePath = getAnsiabcFilePath();
		AnsiFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );

		AnsiFileBuffer::int_type ch;

		// Note: snextc skips the first character
		ch = buffer.snextc();
		Assert::IsTrue( ch == 'B' );

		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'B' );

		ch = buffer.snextc();
		Assert::IsTrue( ch == 'C' );

		ch = buffer.snextc();
		Assert::IsTrue( ch == 'D' );

		ch = buffer.sbumpc();
		Assert::IsTrue( ch == L'D' );

		ch = buffer.sbumpc();
		Assert::IsTrue( ch == L'E' );
	}

	[TestMethod]
	void seekposTest()
	{
		AnsiFileBuffer buffer;
		wstring filePath = getAnsiabcFilePath();
		AnsiFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		AnsiFileBuffer::int_type ch;

		buffer.pubseekpos( 25 );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'Z' );

		buffer.pubseekpos( 0 );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'A' );

		buffer.pubseekpos( 1 );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'B' );

		buffer.pubseekpos( 24 );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'Y' );
	}

	[TestMethod]
	void seekoffTest()
	{
		AnsiFileBuffer buffer;
		wstring filePath = getAnsiabcFilePath();
		AnsiFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		AnsiFileBuffer::int_type ch;

		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'A' );

		buffer.pubseekoff( 25, ios_base::cur );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'Z' );

		buffer.pubseekoff( -24, ios_base::cur );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'B' );

		buffer.pubseekoff( 1, ios_base::cur );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'C' );

		buffer.pubseekoff( 22, ios_base::cur );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'Y' );

		buffer.pubseekoff( 0, ios_base::beg );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'A' );

		buffer.pubseekoff( -1, ios_base::end );
		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'Z' );
	}


	[TestMethod]
	void sputbackTest()
	{
		AnsiFileBuffer buffer;
		wstring filePath = getAnsiabcFilePath();
		AnsiFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		AnsiFileBuffer::int_type ch;

		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'A' );

		ch = buffer.snextc();
		Assert::IsTrue( ch == 'B' );

		ch = buffer.sungetc();
		Assert::IsTrue( ch == 'A' );

		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'A' );

		ch = buffer.sbumpc();
		Assert::IsTrue( ch == 'A' );

		ch = buffer.sgetc();
		Assert::IsTrue( ch == 'B' );
	}


	[TestMethod]
	void writeStringTest()
	{
		AnsiFileBuffer buffer;
		const char* pszFileName = "ansiabc.txt";
		const char* pszString   = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		AnsiFileBuffer* pBuffer = buffer.open( pszFileName, ios_base::out | ios_base::trunc );
		Assert::IsTrue( pBuffer != NULL );
		streamsize length = streamsize( strlen( pszString ) );
		pBuffer->sputn( pszString, length );
		pBuffer->close();
	}

private:
	wstring getAnsiabcFilePath()
	{
		wstring directory = TestBase::TestFileDirectory::get();
		wstring filePath  = directory + L"ansiabc.txt";
		return filePath;
	}
}; // class






} // namespace test
} // namespace sqtpp

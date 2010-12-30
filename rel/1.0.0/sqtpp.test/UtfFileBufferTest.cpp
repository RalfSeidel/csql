#include "stdafx.h"
#include "Streams.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {


[TestClass]
public ref class UtfFileBufferTest : public TestBase
{
public:
	[TestMethod]
	void UtfFileBufferTest::openTest()
	{
		UtfFileBuffer buffer;
		wstring filePath = getUtfAbcFilePath();
		UtfFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		Assert::IsTrue( !buffer.isBigEndian() );

		buffer.close();

		filePath = getUtfbeAbcFilePath();
		pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		Assert::IsTrue( buffer.isBigEndian() );

		buffer.close();
	}

	[TestMethod]
	void UtfFileBufferTest::sbumpcTest()
	{
		wstring filePath = getUtfAbcFilePath();
		sbumpcTestCore( filePath.c_str() );
		filePath = getUtfbeAbcFilePath();
		sbumpcTestCore( filePath.c_str() );
	}


	[TestMethod]
	void UtfFileBufferTest::uflowTest()
	{

		wstring filePath = getUtfAbcFilePath();
		uflowTestCore( filePath.c_str() );
		filePath = getUtfbeAbcFilePath();
		uflowTestCore( filePath.c_str() );
	}

	[TestMethod]
	void UtfFileBufferTest::seekposTest()
	{
		UtfFileBuffer buffer;
		wstring filePath = getUtfAbcFilePath();
		UtfFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		wchar_t wc;

		buffer.pubseekpos( 25 );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'Z' );

		buffer.pubseekpos( 0 );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'A' );

		buffer.pubseekpos( 1 );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'B' );

		buffer.pubseekpos( 24 );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'Y' );
	}

	[TestMethod]
	void UtfFileBufferTest::seekoffTest()
	{
		UtfFileBuffer buffer;
		wstring filePath = getUtfAbcFilePath();
		UtfFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		wchar_t wc;

		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'A' );

		buffer.pubseekoff( 25, ios_base::cur );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'Z' );

		buffer.pubseekoff( -24, ios_base::cur );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'B' );

		buffer.pubseekoff( 1, ios_base::cur );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'C' );

		buffer.pubseekoff( 22, ios_base::cur );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'Y' );

		buffer.pubseekoff( 0, ios_base::beg );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'A' );

		buffer.pubseekoff( -1, ios_base::end );
		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'Z' );
	}


	[TestMethod]
	void UtfFileBufferTest::sputbackTest()
	{
		UtfFileBuffer buffer;
		wstring filePath = getUtfAbcFilePath();
		UtfFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		wchar_t wc;

		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'A' );

		wc = buffer.snextc();
		Assert::IsTrue( wc == L'B' );

		wc = buffer.sungetc();
		Assert::IsTrue( wc == L'A' );

		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'A' );

		wc = buffer.sbumpc();
		Assert::IsTrue( wc == L'A' );

		wc = buffer.sgetc();
		Assert::IsTrue( wc == L'B' );
	}

	[TestMethod]
	void UtfFileBufferTest::readRussiaTest()
	{
		UtfFileBuffer buffer;
		wstring filePath = getRussiaFilePath();
		UtfFileBuffer* pBuffer = buffer.open( filePath.c_str(), ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		wchar_t wc;

		wc = buffer.sbumpc();
		Assert::IsTrue( wc == L'\x0420' );

		wc = buffer.sbumpc();
		Assert::IsTrue( wc == L'\x043E' );
		
		wc = buffer.sbumpc();
		Assert::IsTrue( wc == L'\x0441' );
		
		wc = buffer.sbumpc();
		Assert::IsTrue( wc == L'\x0441' );
		
		wc = buffer.sbumpc();
		Assert::IsTrue( wc == L'\x0438' );
		
		wc = buffer.sbumpc();
		Assert::IsTrue( wc == L'\x044F' );

		wc = buffer.sbumpc();
		Assert::IsTrue( wc == UtfFileBuffer::traits_type::eof() );
	}

	[TestMethod]
	void UtfFileBufferTest::writeStringTest()
	{
		UtfFileBuffer buffer;
		const wchar_t* pszFileName = L"utfabc.txt";
		const wchar_t* pszString   = L"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		UtfFileBuffer* pBuffer = buffer.open( pszFileName, ios_base::out | ios_base::trunc );
		Assert::IsTrue( pBuffer != NULL );

		streamsize length = streamsize( wcslen( pszString ) );
		pBuffer->sputn( pszString, length );
		pBuffer->close();

		// Russia
		pszFileName = L"russia.txt";
		pszString   = L"\x0420\x043E\x0441\x0441\x0438\x044F";
		pBuffer = buffer.open( pszFileName, ios_base::out | ios_base::trunc );
		Assert::IsTrue( pBuffer != NULL );

		length = streamsize( wcslen( pszString ) );
		pBuffer->sputn( pszString, length );
		pBuffer->close();
	}

private:
	void sbumpcTestCore( const wchar_t* pwszFileName )
	{
		UtfFileBuffer buffer;
		UtfFileBuffer* pBuffer = buffer.open( pwszFileName, ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		wchar_t wchar;

		// Note: snextc skips the first character
		wchar = buffer.sbumpc();
		Assert::IsTrue( wchar == 'A' );

		wchar = buffer.sbumpc();
		Assert::IsTrue( wchar == 'B' );


		// skip until Z
		for ( int i = 0; i < 23; ++i ) {
			wchar = buffer.sbumpc();
		}

		wchar = buffer.sbumpc();
		Assert::IsTrue( wchar == 'Z' );

		wchar = buffer.sbumpc();
		Assert::IsTrue( wchar == UtfFileBuffer::traits_type::eof() );

		buffer.close();
	}

	void uflowTestCore( const wchar_t* pwszFileName )
	{
		UtfFileBuffer buffer;
		UtfFileBuffer* pBuffer = buffer.open( pwszFileName, ios_base::in );
		Assert::IsTrue( pBuffer != NULL );
		wchar_t wchar;


		wchar = buffer.snextc();
		Assert::IsTrue( wchar == L'B' );

		wchar = buffer.sgetc();
		Assert::IsTrue( wchar == L'B' );

		wchar = buffer.snextc();
		Assert::IsTrue( wchar == L'C' );

		wchar = buffer.snextc();
		Assert::IsTrue( wchar == L'D' );

		wchar = buffer.sbumpc();
		Assert::IsTrue( wchar == L'D' );

		wchar = buffer.sbumpc();
		Assert::IsTrue( wchar == L'E' );

		buffer.close();
	}

private:
	wstring getUtfAbcFilePath()
	{
		wstring directory = TestBase::TestFileDirectory::get();
		wstring filePath  = directory + L"utfabc.txt";
		return filePath;
	}

	wstring getUtfbeAbcFilePath()
	{
		wstring directory = TestBase::TestFileDirectory::get();
		wstring filePath  = directory + L"utfbeabc.txt";
		return filePath;
	}

	wstring getRussiaFilePath()
	{
		wstring directory = TestBase::TestFileDirectory::get();
		wstring filePath  = directory + L"\x0420\x043E\x0441\x0441\x0438\x044F.txt";
		return filePath;
	}
}; // class


} // namespace test
} // namespace sqtpp

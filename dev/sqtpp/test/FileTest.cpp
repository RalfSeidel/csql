#include "StdAfx.h"
#include <cassert>
#include "../File.h"
#include "FileTest.h"

namespace sqtpp {
namespace test {

/**
** @brief Run all defined tests for this class.
*/
void FileTest::run()
{
	FileTest test;
	/*
	struct Fin {
		FileTest& m_test;
		Fin( FileTest& test ) : m_test(test) { m_test.setUp(); }
		~Fin() { m_test.tearDown(); }
	private:
		Fin();
		Fin( const Fin& that );
		Fin& operator=( const Fin& that );
	};

	finaliser( test );
	*/
	test.getLineTest();

	if ( !File::isDirectory( L"test/input" ) ) {
		wclog << L"File checks skiped because the test input directory does not exist." << endl;
	} else {
		test.isFileTest();
		test.findFileTest();
	}


}

void FileTest::setUp()
{
}

void FileTest::tearDown()
{
}

void FileTest::getLineTest()
{
	wstringstream input( L"Line 1\nLine 2\nLine 3\n" );
	File          file;

	assert( file.getLine() == 0 );
	assert( file.getColumn() == 0 );

	file.attach( input );
	assert( file.getLine() == 1 );
	assert( file.getColumn() == 1 );

	file.incLine();
	assert( file.getLine() == 2 );
	assert( file.getColumn() == 1 );

	file.incColumn( 1 );
	assert( file.getLine() == 2 );
	assert( file.getColumn() == 2 );

	file.incColumn( 8 );
	assert( file.getLine() == 2 );
	assert( file.getColumn() == 10 );

	file.incLine();
	assert( file.getLine() == 3 );
	assert( file.getColumn() == 1 );
}


void FileTest::isFileTest()
{
	assert( File::isFile( L"test/input/buildin.h" ) );
	assert( File::isFile( L"test\\input\\buildin.h" ) );
}

void FileTest::findFileTest()
{
	vector<wstring> includeDirectories;
	File            file;
	wstring         path;

	file.open( L"test/input/include1.h" );

	path = file.findFile( L"include2.h", includeDirectories, false );
	assert( path.empty() );
	
	path = file.findFile( L"include2.h", includeDirectories, true );
	assert( !path.empty() );

	includeDirectories.push_back( L"test/input" );
	path = file.findFile( L"include2.h", includeDirectories, false );
	assert( !path.empty() );


}



} // namespace test
} // namespace sqtpp


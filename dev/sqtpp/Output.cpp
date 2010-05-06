#include "stdafx.h"
#include "Options.h"
#include "CodePage.h"
#include "Error.h"
#include "Output.h"
namespace sqtpp {

/**
** @brief Implementation of a object for emitting the output into a file created
** by the program itself.
*/
class FileOutput : public Output
{
private:
	std::wofstream* m_pFileStream;

public:
	FileOutput( std::wofstream* pFileStream )
	: Output( *pFileStream )
	, m_pFileStream( pFileStream )
	{
	}

	~FileOutput()
	{
		delete m_pFileStream;
	}

	/**
	** @brief Closes the output file stream. 
	*/
	virtual void close()
	{
		m_pFileStream->close();
	}
};

/**
** @brief Implementation of a object for emitting the output into a file created
** by the program itself.
*/
class TestOutput : public Output
{
private:
	std::wostream* m_pFileStream;

public:
	TestOutput( std::wostream* pFileStream )
	: Output( *pFileStream )
	, m_pFileStream( pFileStream )
	{
	}

	/**
	** @brief Closes the output file stream. 
	*/
	virtual void close()
	{
	}
};


/**
** @brief Implementation of a object for emitting the output to the console.
*/
class ConsoleOutput : public Output
{
public:
	ConsoleOutput()
	: Output( std::wcout )
	{
	}

	/**
	** @brief Close the output. 
	** 
	** Because the console cannot be closed this implemenation does nothing.
	*/
	virtual void close()
	{
	}
};


/**
** @brief Construtor.
**
** @param pOutStream The stream to attach to.
*/
Output::Output( std::wostream& outStream )
: m_outStream( outStream )
, m_pErrStream( &std::wcerr )
, m_pLogStream( &std::wclog )
{
}

/**
** @brief Destructor
*/
Output::~Output()
{
}

/**
** @brief Factory method to create the output stream according to the program options.
*/
Output* Output::createOutput( const Options& options )
{
	Output* pOutput = NULL;
	const wstring& sOutputFile  = options.getOutputFile();
	const int nOutputCodePage   = options.getOutputCodePage();
	const CodePageId codePageId = CodePageId( nOutputCodePage  );
	const CodePageInfo& codePageInfo = CodePageInfo::getCodePageInfo( codePageId );
	const locale& codePageLocale( codePageInfo.getLocale() );

	if ( sOutputFile.empty() ) {
		wcout.imbue( codePageLocale );
		pOutput = new ConsoleOutput();
	} else {
		const wchar_t* pszOpenMode = L"wbS";
		FILE* file = _wfopen( sOutputFile.c_str(), pszOpenMode );
		if ( file == NULL ) {
			throw error::C1083( sOutputFile );
		}
		std::wofstream* pOutStream = new std::wofstream( file );
		pOutStream->imbue( codePageLocale );

		pOutStream->exceptions( wofstream::failbit | wofstream::badbit );

		pOutput = new FileOutput( pOutStream );
		if ( codePageInfo.getFileBom() != NULL ) {
			fputs( codePageInfo.getFileBom(), file );
		}
	}

	if ( options.writeErrorsToOutput() ) {
		pOutput->m_pErrStream = &pOutput->m_outStream;
		pOutput->m_pLogStream = &pOutput->m_outStream;
	}

	return pOutput;
}

/**
** @brief Factory method to create the output stream for the unit tests.
*/
Output* Output::createOutput( std::wostream& stream )
{
	Output* pOutput = new TestOutput( &stream );
	return pOutput;
}


} // namespace

#include "stdafx.h"
#include "Options.h"
#include "Output.h"
#include "CmdArgs.h"
#include "Error.h"
#include "Streams.h"
#include "Processor.h"
#if defined( _DEBUG ) && defined( WIN32 )
#include <conio.h>
#include <windows.h>
#endif



using namespace sqtpp;
using namespace std;

/**
** @brief sqtpp main.
** @return success code:
** - 2 fatal error occured.
** - 1 a recoverable error was encountered.
** - 0 Success
*/
int wmain(int argc, const wchar_t* const argv[])
{
#	ifdef _DEBUG
#	ifdef _WIN32
	WinDebugOutBuffer logBuffer;
	wclog.rdbuf( &logBuffer );
#	endif
#	endif

	Options   options;
	Output*   pOutput = NULL;
	Processor cpp( options );
	wostream* pErrStream = &wcerr;
	error::Error::Severity maxMessageSeverity = error::Error::SEV_UNDEFINED;

	try {
		CmdArgs  cmdargs( argc, argv );
		try {
			cmdargs.parse( options );
		} catch ( const error::Error& error ) {
			(*pErrStream) << error;
			return 1;
		} catch ( const std::exception& ex ) {
			(*pErrStream) << ex.what();
			return 3;
		}
		const StringArray& files = cmdargs.getFiles();

		pOutput = Output::createOutput( options );
		pErrStream = &pOutput->getErrStream();

		cpp.setOutput( pOutput );

		if ( files.size() == 0 ) {
			throw error::D8003();
		}

		StringArray::const_iterator it = files.begin();
		while ( it != files.end() ) {
			const wstring& fileName = *it;
			cpp.processFile( fileName );
			++it;
		}
		cpp.close();
		maxMessageSeverity = cpp.getMaxMessageSeverity();
	} catch ( const error::Error& error ) {
		if ( maxMessageSeverity < error::Error::SEV_FATAL ) {
			maxMessageSeverity = error::Error::SEV_FATAL;
		}
		(*pErrStream)  << error;
	} catch ( const std::exception& ex ) {
		if ( maxMessageSeverity < error::Error::SEV_FATAL ) {
			maxMessageSeverity = error::Error::SEV_FATAL;
		}
		(*pErrStream)  << ex.what();
	}
	delete pOutput;

#	ifdef _DEBUG
#	ifdef WIN32
	if ( ::IsDebuggerPresent() ) {
		wcout << "Press any key to exit..." << flush;
		_getwch();
	}
#	else
	wchar_t c;
	wcin >> c;
#	endif
#	endif

	if ( maxMessageSeverity >= error::Error::SEV_FATAL ) {
		return 3;
	}

	if ( maxMessageSeverity >= error::Error::SEV_ERROR ) {
		return 2;
	}

	return 0;
}


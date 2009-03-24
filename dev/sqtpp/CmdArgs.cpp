#include "StdAfx.h"
#include "Error.h"
#include "CodePage.h"
#include "File.h"
#include "Options.h"
#include "CodePage.h"
#include "CmdArgs.h"

namespace sqtpp {

/**
** @brief Contructor.
** @param argc The command line argument count parameter of the standard c main entry function.
** @param argv The command line arguments array of the standard c main entry function.
*/
CmdArgs::CmdArgs( int argc, const wchar_t* const argv[] )
: m_argc( argc )
, m_argv( argv )
, m_bIgnoreMissingArgs( false )
{
}

/**
** @brief Destructor.
*/
CmdArgs::~CmdArgs()
{
}

/**
** @brief Get files to process.
*/
const StringArray& CmdArgs::getFiles()
{
	return m_files;
}


/**
** @brief /? Command line option.
**
** Usage and parameter help.
**
** @todo Implemement this methode.
*/
void CmdArgs::usage()
{
	wcout << L"sqtpp [Flags] Filepath" << endl;
	wcout << L"-?              " << L"Show this help" << endl;
	wcout << L"-V              " << L"Verbose message output" << endl;
	wcout << L"-o              " << L"Output path. If ommited the ouput will be send to stdout." << endl;
	wcout << L"-C[io]Codepage  " << L"Set codepage of input and and ouput." << endl;
	wcout << L"                List of supported code pages:" << endl;

	const CodePageInfo** ppCpi = CodePageInfo::getCodePages();
	while ( *ppCpi != NULL ) {
		const CodePageInfo* pCpi = *ppCpi;
		const CodePage  cp = pCpi->getCodePage();
		if ( cp != CP_UNDEFINED ) {
			const wstring&  name = pCpi->getIdentifier();
			wcout << "                \t" << cp << ": " << name << endl;
		}
		++ppCpi;
	}

	wcout << L"-DMACRO[=Expr]  " << L"Define macro at the command line." << endl;
	wcout << L"-UMACRO         " << L"Undefine predefined macro." << endl;
	wcout << L"-E[P]           " << L"Preprocess to stdout (P: without #line directives)." << endl;
	wcout << L"-IDirectories   " << L"Add directories to look included files for." << endl;
	wcout << L"-M[sm][+|-]     " << L"Allow multiline strings(s) or macros (m)" << endl;
	wcout << L"-Sd|s           " << L"String delimiter used in output. Single (s) or double (d) quotes." << endl;
	wcout << L"                " << L"The default settings depends on the language setting." << endl;
	wcout << L"                " << L"For SQL the default is single quotes." << endl;
	wcout << L"-TLanguage      " << L"Set input language." << endl;
	wcout << L"-e[+|-]         " << L"Option to eliminate empty lines in the output (+)." << endl;
	wcout << L"-c[b|l|s][+|-]  " << L"Option to keep the comments in the output (+) or to eliminate them (-)." << endl;
	wcout << L"                " << L"With the optional specifier b, l and s the elimination can be." << endl;
	wcout << L"                " << L"switched on or off for (b)lock, (l)ine or (s)ql comments." << endl;
	exit( 0 );
}


/**
** @brief Parse command line arguments and set correspondig options.
**
** Switches found at the command line go into the options. Any argument
** which is not a switch is assumed beeing a file path and is stored
** in this object file collection (m_files).
*/
void CmdArgs::parse( Options& options )
{
	int nextArg = 1;

	while ( nextArg < m_argc ) {
		const wchar_t* pszArgument = m_argv[nextArg];

		if ( *pszArgument == L'-' || *pszArgument == L'/' ){
			wchar_t chOption = pszArgument[1];
			switch ( chOption ) {
				case L'?':
					usage();
					break;
				case L'C':
					setCodepage( options, &pszArgument[2] );
					break;
				case L'D':
					addDefine( options, &pszArgument[2] );
					break;
				case L'E':
					setMscppStdoutOptions( options, &pszArgument[2] );
					break;
				case L'I':
					addInclude( options, &pszArgument[2] );
					break;
				case L'M':
					setMultilineOptions( options, &pszArgument[2] );
					break;
				case L'T':
					setLanguage( options, &pszArgument[2] );
					break;
				case L'U':
					addUndef( options, &pszArgument[2] );
					break;
				case L'S':
					setStringDelimiter( options, &pszArgument[2] );
					break;
				case L'X':
					setExtraOptions( options, &pszArgument[2] );
					break;
				case L'V':
					options.verbose( true );
					break;
				case L'c':
					setKeepComments( options, &pszArgument[2] );
					break;
				case L'e':
					setEliminateEmptyLines( options, &pszArgument[2] );
					break;
				case L'o':
					setOutputFile( options, &pszArgument[2] );
					break;
				default:
					// Invalid argument {1}.
					error::D9002 warning( pszArgument );
					wcerr << warning;
					break;
			}
		} else {
			break;
		}
		++nextArg;
	}


	if ( !m_bIgnoreMissingArgs && nextArg >= m_argc ) {
		// No input file specified in the command line arguments.
		throw error::D8003();
	}


	while ( nextArg < m_argc ) {
		wstring sFile( m_argv[nextArg] );

		File::checkFile( sFile );

		m_files.push_back( sFile );
		++nextArg;
	}
}

/**
** @brief /I Command line option: Add include directories.
**
** Set or add included directories. More than one directory can be passed
** with one switch by seperating the directories with a semicolon (Windows)
** or colon (Unix).
*/
void CmdArgs::addInclude( Options& options, const wchar_t* pwszArgument )
{
	options.addIncludeDirectories( pwszArgument );
}

/**
** @brief /D Command line option: Define macro.
** 
** Define macro at the command line.\p
** Syntax is: /DMACRO[(:|=|\#)MacroExpressions]
*/
void CmdArgs::addDefine( Options& options, const wchar_t* pwszArgument )
{
	if ( pwszArgument == NULL || *pwszArgument == L'\0' ) {
		// Missing argument for the command line option {1}.
		throw error::D8004( L"/D" );
	}
	// Lookup =, | or #
	const wchar_t* pwszAssign     = pwszArgument;
	const wchar_t* pwszExpression = L"";
	while ( *pwszAssign != L'\0' ) {
		if ( wcschr( L"=|#", *pwszAssign ) != NULL ) 
			break;
		++pwszAssign;
	}

	size_t length = pwszAssign - pwszArgument;

	if ( length == 0 ) {
		// Missing argument for the command line option {1}.
		throw error::D8004( L"/D" );
	}

	if ( *pwszAssign != L'\0' ) {
		pwszExpression = pwszAssign + 1;
	}

	wstring identifier = wstring( pwszArgument, length );
	wstring expression = wstring( pwszExpression );

	options.m_macroDefines[identifier] = expression;
}

/**
** @brief Add undefine of predefined macro (handler for the /U command line option).
**
*/
void CmdArgs::addUndef( Options& options, const wchar_t* pwszArgument )
{
	if ( pwszArgument == NULL || *pwszArgument == L'\0' ) {
		// Missing argument for the command line option {1}.
		throw error::D8004( L"/U" );
	}

	wstring identifier = wstring( pwszArgument );
	options.m_macroUndefines.push_back( identifier );
}

/**
** @brief Handle /C: Set codepage.
**
** <dl>
** <dt>/Co[codepage]</dt>
** <dd>Set the codepage for the output.</dd>
** <dt>/Ci[codepage]</dt>
** <dd>Set the codepage for the input.</dd>
** <dt>/C[codepage]</dt>
** <dd>Set the codepage for both, input and output.</dd>
** </dl>
*/
void CmdArgs::setCodepage( Options& options, const wchar_t* pwszArgument )
{
	bool    bInput    = false;
	bool    bOutput   = false;
	wchar_t wcNext    = pwszArgument[0];

	if ( wcNext == L'o' ) {
		bOutput = true;
		++pwszArgument;
	} else if ( wcNext == L'i' ) {
		bInput = true;
		++pwszArgument;
	} else {
		bInput  = true;
		bOutput = true;
	}

	wcNext = pwszArgument[0];
	if ( !isdigit( wcNext ) ) {
		if ( pwszArgument == NULL || *pwszArgument == L'\0' ) {
			// Missing argument for the command line option {1}.
			throw error::D8004( L"/C[io]Codepage" );
		}
	}
	long     lCodePage = wcstol( pwszArgument, NULL, 10 );
	CodePage cp        = CodePage(lCodePage);

	if ( CodePageInfo::findCodePageInfo( cp ) == NULL ) {
		// Invalid argument {1}.
		error::D9002 warning( pwszArgument );
		wcerr << warning;
	} else {
		if ( bInput ) {
			options.setInputCodePage( (unsigned short)cp );
		}
		if ( bOutput ) {
			options.setOutputCodePage( (unsigned short)cp );
		}
	}
}


/**
** @brief Set the source file language (handler for the /T command line option).
** 
** <table>
** <tr><th>Option</th><th>Language</th></tr>
** <tr><td>A</td><td>Assembler</td></tr>
** <tr><td>C</td><td>C</td></tr>
** <tr><td>H</td><td>HTML</td></tr>
** <tr><td>P</td><td>C++</td></tr>
** <tr><td>R</td><td>RC</td></tr>
** <tr><td>S</td><td>SQL</td></tr>
** <tr><td>T</td><td>Text</td></tr>
** <tr><td>X</td><td>XML</td></tr>
** </table>
*/
void CmdArgs::setLanguage( Options& options, const wchar_t* pwszArgument )
{
	if ( pwszArgument == NULL || *pwszArgument == L'\0' ) {
		// Missing argument for the command line option {1}.
		throw error::D8004( L"/T" );
	}
	switch ( pwszArgument[0] ) {
		case L'a':
		case L'A':
			options.setLanguage( Options::LNG_ASM );
			break;
		case L'c':
		case L'C':
			options.setLanguage( Options::LNG_C );
			break;
		case L'h':
		case L'H':
			options.setLanguage( Options::LNG_XML );
			break;
		case L'p':
		case L'P':
			options.setLanguage( Options::LNG_CPP );
			break;
		case L'r':
		case L'R':
			options.setLanguage( Options::LNG_RC );
			break;
		case L's':
		case L'S':
			options.setLanguage( Options::LNG_SQL );
			break;
		case L't':
		case L'T':
			options.setLanguage( Options::LNG_TEXT );
			break;
		case L'x':
		case L'X':
			options.setLanguage( Options::LNG_XML );
			break;
		default:
			// Unknown file type {1}.
			error::D9024 warning( pwszArgument );
			wcerr << warning;
			break;
	}
}


/**
** @brief Set output to stdout.
** 
** The ms cl sets the preprocessor output to stdout
** when the parameters /E or /EP are provided. To be able 
** to replace mscpp with sqtpp without modifieing the command
** line arguments we support theese options aswell.
**
** <dl>
** <dt>/E</dt>
** <dd>Preprocess to stdout</dd>
** <dt>/EP</dt>
** <dd>Preprocess to stdout and do not emit \#line directives.</dd>
** </dl>
**
*/
void CmdArgs::setMscppStdoutOptions( Options& options, const wchar_t* pwszArgument )
{
	wchar_t wcNext = pwszArgument[0];
	switch ( wcNext ) {
		case L'\0':
			options.emitLine( true );
			options.setOutputFile( wstring() );
			break;
		case L'P':
			options.emitLine( false );
			options.setOutputFile( wstring() );
			break;
		default:
			// Invalid argument {1}.
			error::D9002 warning( pwszArgument );
			wcerr << warning;
			break;
	}
}


/**
** @brief /M Set multi line options.
**
** Following options are supported:
** - /Ms: allow multi line strings literals.
** - /Mm: allow multi line macros (default).
*/
void CmdArgs::setMultilineOptions( Options& options, const wchar_t* pwszArgument )
{
	bool multiLineStrings = options.multiLineStringLiterals();
	bool multiLineMacros  = options.multiLineMacroExpansion();
	bool* pLastOption     = NULL;
	bool argWarning       = true;

	const wchar_t* pwcNext = pwszArgument;
	while ( *pwcNext != L'\0' ) {
		switch ( *pwcNext ) {
		case L's':
			multiLineStrings = true;
			pLastOption      = &multiLineStrings;
			break;
		case L'm':
			multiLineMacros = true;
			pLastOption     = &multiLineMacros;
			break;
		case L'+':
			if ( pLastOption != NULL ) {
				*pLastOption = true;
			}
			break;
		case L'-':
			if ( pLastOption != NULL ) {
				*pLastOption = false;
			}
			break;
		default:
			if ( argWarning ) {
				// Invalid argument {1}.
				error::D9002 warning( pwszArgument );
				wcerr << warning;
				argWarning = false;
			}
			break;
		}
		++pwcNext;
	}
	options.multiLineStringLiterals( multiLineStrings );
	options.multiLineMacroExpansion( multiLineMacros );
}


/**
** @brief /S Command line option: Set the output string delimiter.
** 
*/
void CmdArgs::setStringDelimiter( Options& options, const wchar_t* pwszArgument )
{
	switch ( pwszArgument[0] ) {
		case L's':
		case L'S':
			options.setStringDelimiter( Options::STRD_SINGLE );
			break;
		case L'd':
		case L'D':
			options.setStringDelimiter( Options::STRD_DOUBLE );
			break;
		default:
			// Unknown file type {1}.
			error::D9024 warning( pwszArgument );
			wcerr << warning;
			break;
	}
}

/**
** @brief /e[+|-] Eliminate empty lines in the ouput (on or off).
** 
** Default is off.
*/
void CmdArgs::setEliminateEmptyLines( Options& options, const wchar_t* pwszArgument )
{
	bool bEliminate = *pwszArgument == L'\0' || *pwszArgument == L'+';
	options.eliminateEmptyLines( bEliminate );
}

/**
** @brief /o Set the path of the output file.
*/
void CmdArgs::setOutputFile( Options& options, const wchar_t* pwszArgument )
{
	if ( *pwszArgument == L'\0' ) {
		// {1} requires {2}; option ignored
		error::D9007 warning( L"-o", L"[filename]");
		return;
	}

	if ( *pwszArgument == L'"' ) {
	} else {
		options.setOutputFile( pwszArgument );
	}
}


/**
** @brief /c[b|l|s][+|-] Option to emit the comments to the ouput (on or off).
** 
** Default is off.
**
** Note that in SQL mode the SQL line comments are always kept.
*/
void CmdArgs::setKeepComments( Options& options, const wchar_t* pwszArgument )
{
	bool bKeepBlockComments = true;
	bool bKeepLineComments = true;
	bool bKeepSqlComments = true;
	bool* pbKeepBlockComments = &bKeepBlockComments;
	bool* pbKeepLineComments = &bKeepLineComments;;
	bool* pbKeepSqlComments = &bKeepSqlComments;
	bool bFoundCommentType = false;
	const wchar_t* pwcNext = pwszArgument;
		
	while ( *pwcNext != '\0' ) {
		const wchar_t wcChar = *pwcNext;
		++pwcNext;

		switch ( wcChar ) {
			case L'b':
				pbKeepBlockComments = &bKeepBlockComments;
				bKeepBlockComments = true;
				if ( !bFoundCommentType ) {
					pbKeepLineComments = NULL;
					pbKeepSqlComments = NULL;
					bFoundCommentType = true;
				}
				break;
			case L'l':
				pbKeepLineComments = &bKeepLineComments;
				bKeepLineComments = true;
				if ( !bFoundCommentType ) {
					pbKeepBlockComments = NULL;
					pbKeepSqlComments = NULL;
					bFoundCommentType = true;
				}
				break;
			case L's':
				pbKeepSqlComments = &bKeepSqlComments;
				bKeepSqlComments = true;
				if ( !bFoundCommentType ) {
					pbKeepBlockComments = NULL;
					pbKeepLineComments = NULL;
					bFoundCommentType = true;
				}
				break;
			case L'+':
				if ( pbKeepBlockComments != NULL ) {
					*pbKeepBlockComments = true;
				}
				if ( pbKeepLineComments != NULL ) {
					*pbKeepLineComments = true;
				}
				if ( pbKeepSqlComments != NULL ) {
					*pbKeepSqlComments = true;
				}
				bFoundCommentType = false;
				break;
			case L'-':
				if ( pbKeepBlockComments != NULL ) {
					*pbKeepBlockComments = false;
				}
				if ( pbKeepLineComments != NULL ) {
					*pbKeepLineComments = false;
				}
				if ( pbKeepSqlComments != NULL ) {
					*pbKeepSqlComments = false;
				}
				bFoundCommentType = false;
				break;
			default:
				// Invalid argument {1}.
				error::D9002 warning( pwszArgument );
				wcerr << warning;
				break;
		}
	}
	options.keepBlockComments( bKeepBlockComments );
	options.keepLineComments( bKeepLineComments );
	options.keepSqlComments( bKeepSqlComments );
}


/**
** @brief /X Command line option: Support of some very special, not common usefull options.
** 
** Currently only the extra options \c NG is defined. It customizes sqtpp to be able 
** to understand some source code tags which are used in the scripts of one of our
** customers.
*/
void CmdArgs::setExtraOptions( Options& options, const wchar_t* pwszArgument )
{
	if ( wcscmp( pwszArgument, L"NG" ) == 0 ) {
		options.supportAdSalesNG( true );
	}
}

} // namespace

/**
** @file
** @author Ralf Seidel
** @brief Declaration of the lexical scanner (#sqtpp::Scanner).
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_CMD_ARGS_H
#define SQTPP_CMD_ARGS_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {


class Options;

/**
** @brief Command line argument parser / evaluator.
**
** @note The implementation of the command line parser is somehow a bit quick and dirty.
** 
** @todo Dokumentation of all valid command line arguments.
** @todo Make all options accessible through command line arguments.
*/
class CmdArgs
{
private:
	/// Main argc.
	const int                   m_argc;
	/// Main argv.
	const wchar_t* const* const m_argv;

	/// Input files found at the command line.
	StringArray                 m_files;

	/// Option for the tests of this class to ignore missing arguments
	bool                        m_bIgnoreMissingArgs;
public:
	// Constructor.
	CmdArgs( int argc, const wchar_t* const argv[] );
	// Destructor.
	~CmdArgs();
private:
	// Copy constructor (not implemented).
	CmdArgs( const CmdArgs& );
	// Assignment operator (not implemented).
	CmdArgs& operator=( const CmdArgs& that );


public:
	// Parse command line arguments and set correspondig options.
	void parse( Options& options );

	// Get the files to process.
	const StringArray& getFiles();


	/// @brief For testing: set option to ignore incomplete arguments.
	void ignoreMissingArgs( const bool bIgnore ) { m_bIgnoreMissingArgs = bIgnore; }

private:
	// Handle /?
	void usage();

	// Handle /I
	void addInclude( Options& options, const wchar_t* pwszArgument );

	// Handle /D
	void addDefine( Options& options, const wchar_t* pwszArgument );

	// Handle /U 
	void addUndef( Options& options, const wchar_t* pwszArgument );

	// Handle /C
	void setCodepage( Options& options, const wchar_t* pwszArgument );

	// Handle /T
	void setLanguage( Options& options, const wchar_t* pwszArgument );

	// Handle /E
	void setMscppStdoutOptions( Options& options, const wchar_t* pwszArgument );

	// Handle /M
	void setMultilineOptions( Options& options, const wchar_t* pwszArgument );

	// Handle /S
	void setStringDelimiter( Options& options, const wchar_t* pwszArgument );

	// Handle /X
	void setExtraOptions( Options& options, const wchar_t* pwszArgument );

	// Handle /e
	void setEliminateEmptyLines( Options& options, const wchar_t* pwszArgument );

	// Handle /i
	void setInputFile( Options& options, const wchar_t* pwszArgument );

	// Handle /o
	void setOutputFile( Options& options, const wchar_t* pwszArgument );

	// Handle /k
	void setKeepComments( Options& options, const wchar_t* pwszArgument );

	// Check the argument and elminiate leading and trailing quotes.
	static wstring getFilePath( const wchar_t* pwszArgument );
};


} // namespace


#endif // SQTPP_CMD_ARGS_H

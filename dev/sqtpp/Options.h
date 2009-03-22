/**
** @file
** @author Ralf Seidel
** @brief Declaration of the #sqtpp::Options.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_OPTIONS_H
#define SQTPP_OPTIONS_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

class CmdArgs;
class OptionsStack;

/**
** @brief Options for preprocessing.
**
** @todo Make options accessible through string interface to
**       allow modification of options by preprocessor
**       @code
**       \#pragma option( id, value )
**       @endcode
**       directives.
*/
class Options
{
private:
public:
	/**
	** @brief Enumeration of new line handling.
	*/
	enum NewLineOutput
	{
		/// Undefined / initial value.
		NLO_UNDEFINED,
		/// Keep new line characters as found in the input (default if output is a file).
		NLO_AS_IS,
		/// Use operating system standard line ending (default if output is stdout).
		NLO_OS_DEFAULT,
		/// Emit line feeds only (UNIX convention).
		NLO_LF,       
		/// Emit line feeds only (Mac convention).
		NLO_CR,       
		/// Emit line feeds only (Windows/DOS convention).
		NLO_CRLF      
	};

	/**
	** @brief Source code language.
	**
	** Used to predefine some of the preprocessor options. 
	** This option is controlled by the command line argument /T.
	*/ 
	enum Language {
		/// Undefined / initial value.
		LNG_UNDEFINED,
		/// Plain text.
		LNG_TEXT,
		/// XML / XHTML
		LNG_XML,
		/// Good old c
		LNG_C,
		/// c++
		LNG_CPP,
		/// Microsofts assembler
		LNG_ASM,
		/// Resource compiler
		LNG_RC,
		/// Transact SQL (Microsoft / Sybase SQL dialect)
		LNG_SQL
	};


	/**
	** @brief Debugging informations about the defined languages.
	*/
	class LanguageInfo {
	public:
		/// The language id
		const  Language language;
		/// The enumeration symbol.
		const  wchar_t* pwszSymbol;
		/// The symbol that will be defined for this language.
		const  wchar_t* pwszMacro;

		// Initialising constructor.
		LanguageInfo( Language eLanguage, const wchar_t* pwszSymbol, const wchar_t* pwszMacro );
	private:
		// Assignment operator (not implemented).
		LanguageInfo& operator= ( const LanguageInfo& that );
	};

	/**
	** @brief String quoting options.
	*/ 
	enum Quoting {
		/// Undefined / initial value.
		QUOT_UNDEFINED = 0,
		/// When quoting use c like backslash escape characters.
		QUOT_ESCAPE    = L'\\',
		/// When quoting duplicate the quote characters (default for SQL script preprocessing).
		QUOT_DOUBLE    = L'\"'
	};


	/**
	** @brief Options how emitted strings are delimited.
	*/ 
	enum StringDelimiter {
		/// Undefined / initial value.
		STRD_UNDEFINED  = 0,
		/// Use standard double quotes.
		STRD_DOUBLE     = L'\"',
		/// Use single quotes.
		STRD_SINGLE     = L'\''
	};


private:
	/**
	** @brief Character used to seperate directories.
	**
	** For Windows the character is a semicolon. For the others it is a colon.
	*/
	static const wchar_t m_wcDirectorySeperator;

	/**
	** @brief Informations about all definable source file languages.
	*/
	static const LanguageInfo m_languageInfo[];


	/**
	** @brief The input source code language.
	**
	** Default: C @par
	**
	** Possible values: @par
	** <table>
	** <tr><th>Language</th><th>Description</th></tr>
	** <tr><td>Text    </td><td>Does no preproccing but simply reads and writes the input</td></tr>
	** <tr><td>XML     </td><td>Optional removal of blanks and comments only.</td></tr>
	** <tr><td>C       </td><td>C Language</td></tr>
	** <tr><td>C++     </td><td>C++ Language</td></tr>
	** <tr><td>RC      </td><td>Microsoft Windows / OS/2 resource file language</td></tr>
	** <tr><td>ASM     </td><td>Assembler</td></tr>
	** <tr><td>SQL     </td><td>SQL Language</td></tr>
	** </table>
	*/
	Language m_eLanguage;

	/**
	** @brief Option how to quotes inside a string are formatted.
	**
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>N/A</td></tr>
	** <tr><td>XML     </td><td>N/A</td></tr>
	** <tr><td>C       </td><td>Backslah notation</td></tr>
	** <tr><td>C++     </td><td>Backslah notation</td></tr>
	** <tr><td>RC      </td><td>Backslah notation</td></tr>
	** <tr><td>ASM     </td><td>Backslah notation</td></tr>
	** <tr><td>SQL     </td><td>Double quotes</td></tr>
	** </table>
	*/
	Quoting  m_eQuoting;


	/**
	** @brief Option how to quote string.
	**
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>N/A</td></tr>
	** <tr><td>XML     </td><td>N/A</td></tr>
	** <tr><td>C       </td><td>double quotes (")</td></tr>
	** <tr><td>C++     </td><td>double quotes (")</td></tr>
	** <tr><td>RC      </td><td>double quotes (")</td></tr>
	** <tr><td>ASM     </td><td>double quotes (")</td></tr>
	** <tr><td>SQL     </td><td>single quotes (')</td></tr>
	** </table>
	*/
	StringDelimiter     m_eStringDelimiter;

	/**
	** @brief Option if an how new lines in the input stream should be converted.
	**
	** By default the new line characters CR & LF are returned in the ouput as they are 
	** found in the input stream. This option can be used to transform them into the 
	** conventions for Unix, Mac or Windows. 
	*/
	NewLineOutput        m_eNewLineOutput;

	/**
	** @brief Emit line number and file information?
	**
	** If this flag is set the pre processor will emit \#line directive to
	** synchronize the current input position with the position in the output 
	** stream.
	**
	** Defaults:
	**
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>false   </td></tr>
	** <tr><td>XML     </td><td>false   </td></tr>
	** <tr><td>C       </td><td>true    </td></tr>
	** <tr><td>C++     </td><td>true    </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>true    </td></tr>
	** </table>
	*/
	bool     m_bEmitLine;

	/**
	** @brief Keep block comments?
	**
	** Defaults:
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>true    </td></tr>
	** <tr><td>XML     </td><td>true    </td></tr>
	** <tr><td>C       </td><td>false   </td></tr>
	** <tr><td>C++     </td><td>false   </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>false   </td></tr>
	** </table>
	*/
	bool     m_bKeepBlockComments;

	/**
	** @brief Keep line comments?
	**
	** Defaults:
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>true    </td></tr>
	** <tr><td>XML     </td><td>true    </td></tr>
	** <tr><td>C       </td><td>false   </td></tr>
	** <tr><td>C++     </td><td>false   </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>false   </td></tr>
	** </table>
	*/
	bool     m_bKeepLineComments;

	/**
	** @brief Keep SQL line comments?
	**
	** Defaults:
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>true    </td></tr>
	** <tr><td>XML     </td><td>true    </td></tr>
	** <tr><td>C       </td><td>true    </td></tr>
	** <tr><td>C++     </td><td>true    </td></tr>
	** <tr><td>RC      </td><td>true    </td></tr>
	** <tr><td>ASM     </td><td>true    </td></tr>
	** <tr><td>SQL     </td><td>true    </td></tr>
	** </table>
	*/
	bool     m_bKeepSqlComments;

	/**
	** @brief Expand macros in several lines.
	**
	** Traditionally the C preprocess expands all macros in one single line. This makes the produced 
	** code sometimes hardly readable. If the <code>MultiLineMacroExpansion</code> option is set the 
	** expanded macro will have line breaks at the same position as its defintion.
	** 
	** Example:
	**
	** @code
	** #define MACRO \
	**               A \
	**               B \
	**               C 
	**
	** MACRO
	** @endcode
	**
	** will lead to 
	**
	** @code
	**          A
	**          B
	**          C
	** @endcode
	**
	** instead of 
	**
	** @code
	**          A         B         C
	** @endcode
	** 
	** Defaults:
	**
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>N/A     </td></tr>
	** <tr><td>XML     </td><td>N/A     </td></tr>
	** <tr><td>C       </td><td>false   </td></tr>
	** <tr><td>C++     </td><td>false   </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>true    </td></tr>
	** </table>
	*/
	bool     m_bMultiLineMacroExpansion;


	/**
	** @brief Allow strings to exceed the end of the line.
	**
	**
	*/
	bool     m_bMultiLineStringLiterals;

	/**
	** @brief Expand macros found in a macro argument list?
	**
	** Example:
	**
	** @code
	** #define CONCATE( DO, CONCATE ) DO#CONCATE
	** CONCATE( A, B )
	** CONCATE( CONCATE( A, B ), C )
	** @endcode
	**
	** By default the c preprocessor producess the following result:
	**
	** @code
	** AB
	** CONCATE( A, B )C
	** @endcode
	**
	** The this option is set the result will be:
	** @code
	** AB
	** ABC
	** @endcode
	**
	** @todo This option is currently not supported.
	** To implement this feature the processor must become a stack of token sources. Whenever
	** a macro is found in the argument list the current token source will be replaced with the
	** tokens of the expanded macros. 
	*/
	bool     m_bExpandMacroArguments;

	/**
	** @brief Never output more than one empty line.
	**
	** Defaults:
	**
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>false   </td></tr>
	** <tr><td>XML     </td><td>false   </td></tr>
	** <tr><td>C       </td><td>false   </td></tr>
	** <tr><td>C++     </td><td>false   </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>false   </td></tr>
	** </table>
	*/
	bool     m_bEliminateEmptyLines;

	/**
	** @brief Remove leading blanks from the ouput.
	**
	** Defaults:
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>false   </td></tr>
	** <tr><td>XML     </td><td>false   </td></tr>
	** <tr><td>C       </td><td>false   </td></tr>
	** <tr><td>C++     </td><td>false   </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>false   </td></tr>
	** </table>
	*/
	bool     m_bTrimLeadingBlanks;

	/**
	** @brief Remove trailing blanks from the ouput.
	**
	** Defaults:
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>false   </td></tr>
	** <tr><td>XML     </td><td>false   </td></tr>
	** <tr><td>C       </td><td>false   </td></tr>
	** <tr><td>C++     </td><td>false   </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>false   </td></tr>
	** </table>
	*/
	bool     m_bTrimTrailingBlanks;

	/**
	** @brief Ignore the current working directory when searching for include files?
	**
	** Defaults:
	**
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>N/A     </td></tr>
	** <tr><td>XML     </td><td>N/A     </td></tr>
	** <tr><td>C       </td><td>false   </td></tr>
	** <tr><td>C++     </td><td>false   </td></tr>
	** <tr><td>RC      </td><td>false   </td></tr>
	** <tr><td>ASM     </td><td>false   </td></tr>
	** <tr><td>SQL     </td><td>false   </td></tr>
	** </table>
	*/
	bool     m_bIgnoreCWD;

	/**
	** @brief Do not define any buildin macro?
	*/
	bool     m_bUndefAllBuildin;

	/**
	** @brief Enable support for AdSales NG source tags.
	*/
	bool     m_bSupportAdSalesNG;

	/**
	** @brief Verbose message output.
	*/
	bool	 m_bVerbose;

	/**
	** @brief The codepage of the input.
	** 
	** Default is 1252 (ANSI - Latin I).
	**
	** The following codepages are defined on windows:
	**
	** @htmlinclude "codepages.html"
	*/
	unsigned short m_nInputCodePage;


	/**
	** @brief The codepage of the output.
	** 
	** Default is 1252 (ANSI / WINDOWS 1252)
	**
	** The following codepages are defined on windows:
	**
	** @htmlinclude "doc/codepages.html"
	*/
	unsigned short m_nOutputCodePage;

	/**
	** @brief The file to which output should be written (default is "" / stdout).
	** 
	** If no file is specified the preprocessor will write all output to stdout.
	*/
	wstring         m_sOutputFile;

	/**
	** @brief Include directories specified as command line parameter.
	**
	** Defaults:
	**
	** <table>
	** <tr><th>Language</th><th>Default</th></tr>
	** <tr><td>Text    </td><td>N/A</td></tr>
	** <tr><td>C       </td><td></td></tr>
	** <tr><td>C++     </td><td></td></tr>
	** <tr><td>RC      </td><td></td></tr>
	** <tr><td>ASM     </td><td></td></tr>
	** <tr><td>SQL     </td><td></td></tr>
	** </table>
	*/
	wstring           m_sIncludeDirectories;

	/**
	** @brief Macros defined on the command line with /D.
	*/
	StringDictionary  m_macroDefines;

	/**
	** @brief Macros undefined on the command line with /U.
	*/
	StringArray       m_macroUndefines;

	/**
	** @brief Format string to be used when emmitting the __DATE__ macro.
	**
	** Default:
	**
	** @code
	** "%b %d %Y"
	** @endcode
	**
	*/
	std::wstring m_sDateFormat;

	/**
	** @brief Format string to be used when emmitting the __TIME__ macro.
	**
	** The formatted string will be enclosed in quotes. Which kind of quotes are used
	** depends on the quote option.
	** Defaults:
	**
	** @code
	** "%H:%M:%S"
	** @endcode
	*/
	std::wstring m_sTimeFormat;

	/**
	** @brief Format string to be used when emmitting the __TIMESTAMP__ macro.
	*/
	std::wstring m_sTimestampFormat;

	/**
	** @brief Collection of directories to search included files for.
	*/
	StringArray  m_includeDirectories;

public:
	// The constructor.
	Options();

	// The destructor.
	virtual ~Options() throw();

	// Get default options.
	const Options& getDefaultOptions();

	/// Get the source language format.
	Language    getLanguage() const throw()              { return m_eLanguage; }
	/// Set the source language format.
	void setLanguage( Language language ) throw()        { m_eLanguage = language; }

	/// Get the codepage of the (8 bit) input files.
	unsigned short getInputCodePage() const throw()      { return m_nInputCodePage; }
	/// Set the codepage of the (8 bit) input files.
	void setInputCodePage( unsigned short cp ) throw()   { m_nInputCodePage = cp; }

	/// Get the codepage for the output file.
	unsigned short getOutputCodePage() const throw()     { return m_nOutputCodePage; }
	/// Set the codepage of the (8 bit) input files.
	void setOutputCodePage( unsigned short cp ) throw()  { m_nOutputCodePage = cp; }

	/// Get the path of the file to which the output should be written.
	const wstring& getOutputFile() const throw()         { return m_sOutputFile; }
	/// Set the path of the file to which the output should be written.
	void setOutputFile( const wstring& sPath ) throw()   { m_sOutputFile = sPath; }

	/// Get some information about the given language.
	static const LanguageInfo& getLanguageInfo( Language language ) throw();

	/// Get some information about the configured language.
	const LanguageInfo& getLanguageInfo() const throw()  { return Options::getLanguageInfo( m_eLanguage ); }

	/// Get the quoting option.
	Quoting getStringQuoting() const throw()               { return m_eQuoting; }
	/// Get the string quoting option.
	void  setStringQuoting( Quoting quoting ) throw()      { m_eQuoting = quoting; }

	/// Get the new line (cr and/or lf) transformation option.
	NewLineOutput getNewLineOutput() const throw()       { return m_eNewLineOutput; }
	/// set the new line (cr and/or lf) transformation option.
	void setNewLineOutput( NewLineOutput option ) throw() { m_eNewLineOutput = option; }

	/// See #m_eStringDelimiter.
	StringDelimiter getStringDelimiter() const throw() { return m_eStringDelimiter; }
	/// See #m_eStringDelimiter.
	void setStringDelimiter( StringDelimiter delimiter ) throw() { m_eStringDelimiter = delimiter; }

	/// See #m_bVerbose
	bool verbose() const { return m_bVerbose; }
	/// See #m_bVerbose
	void verbose( bool bValue ) { m_bVerbose = bValue; }

	/// Get the date format string.
	const std::wstring getDateFormat() const;
	/// Set the date format string.
	void setDateFormat( const std::wstring& format )  { m_sDateFormat = format; }

	/// Get the time format string.
	const std::wstring getTimeFormat() const;
	/// Set the time format string.
	void setTimeFormat( const std::wstring& format )  { m_sTimeFormat = format; }

	/// Get the time format string.
	const std::wstring getTimestampFormat() const;
	/// Set the time format string.
	void setTimestampFormat( const std::wstring& format )  { m_sTimestampFormat = format; }

	/// Test if quote characters insided string are escaped by double quotes e.g. "A""B";
	bool doubleQuoteEscaping() const throw()          { return ( m_eQuoting & QUOT_DOUBLE ) != 0 ; }

	/// Do not define any buildin macro?
	bool undefAllBuildin() const throw()              { return m_bUndefAllBuildin; }

	/// Check if block comments should be emmited.
	bool keepBlockComments() const throw()                 { return m_bKeepBlockComments; }
	/// Set if block comments should be emmited.
	void keepBlockComments( bool bKeep ) throw()           { m_bKeepBlockComments = bKeep; }

	/// Check if line comments should be emmited.
	bool keepLineComments() const throw()                 { return m_bKeepLineComments; }
	/// Set if line comments should be emmited.
	void keepLineComments( bool bKeep ) throw()           { m_bKeepLineComments = bKeep; }

	/// Check if SQL comments should be emmited.
	bool keepSqlComments() const throw()                 { return m_bKeepSqlComments; }
	/// Set if SQL comments should be emmited.
	void keepSqlComments( bool bKeep ) throw()           { m_bKeepSqlComments = bKeep; }

	/// Switch any comment option on or off
	void keepComments( bool bKeep ) throw();

	/// Check if line information should be emitted by the processor.
	bool emitLine() const throw()                     { return m_bEmitLine; }
	/// Check if line information should be emitted by the processor.
	void emitLine( bool bEmit ) throw()               { m_bEmitLine = bEmit; }

	/// Check if new lines in macros should not be replace with a single space character.
	bool multiLineMacroExpansion() const throw()      { return m_bMultiLineMacroExpansion; }
	/// Check if new lines in macros should not be replaced with a single space character.
	void multiLineMacroExpansion( bool bExpand ) throw() { m_bMultiLineMacroExpansion = bExpand; }

	/// Test if string literals may exceed the end of a line.
	bool multiLineStringLiterals() const throw()      { return m_bMultiLineStringLiterals; };
	/// Set option to allow or disallow strings to exceed the end of a line
	void multiLineStringLiterals( bool bAllow ) throw() {  m_bMultiLineStringLiterals = bAllow; };

	/// Check if macro arguments are to expanded if they include any other argument (see #m_bExpandMacroArguments).
	bool expandMacroArguments() const throw()         { return m_bExpandMacroArguments; }
	/// Check if macro arguments are to expanded if they include any other argument (see #m_bExpandMacroArguments).
	void expandMacroArguments( bool bExpand ) throw() { m_bExpandMacroArguments = bExpand; }

	/// Check if support for S4M AdSalesNG source tags is enabled.
	bool supportAdSalesNG() const throw()             { return m_bSupportAdSalesNG; }
	/// Enable/disable support for S4M AdSales NG source tags.
	void supportAdSalesNG( bool bEnable ) throw()     { m_bSupportAdSalesNG = bEnable; }

	/// Check if leading blanks should be suppressed.
	bool trimLeadingBlanks() const throw()            { return m_bTrimLeadingBlanks; }

	/// Check if trailing blanks should be suppressed.
	bool trimTrailingBlanks() const throw()           { return m_bTrimTrailingBlanks; }

	/// Check if empty line should be suppressed.
	bool eliminateEmptyLines() const throw()          { return m_bEliminateEmptyLines; }

	/// Set if empty line should be suppressed.
	void eliminateEmptyLines( bool bEliminate ) throw() { m_bEliminateEmptyLines = bEliminate; }


	/// Add the given directories to the array of included directories.
	void addIncludeDirectories( const wchar_t* pwszIncludeDirectories );

	/// Get all included directories.
	const StringArray& getIncludeDirectories() const throw() { return m_includeDirectories; }

	/// Get macros to be undefined (passed at the command line).
	const StringArray& getUndefines() const throw() { return m_macroUndefines; }

	/// Get macros to be defined before first file is processed (passed at the command line).
	const StringDictionary& getDefines() const throw() { return m_macroDefines; }

private:
	/// Set the default options for the source code language.
	void setLanguageDefaults();

friend class sqtpp::CmdArgs;
};

/**
** @brief A stack of options (for pragma push/pop(Options).
*/
class OptionsStack : public std::stack<Options>
{
};


} // namespace

#endif // SQTPP_OPTIONS_H

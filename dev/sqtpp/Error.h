/**
** @file
** @author Ralf Seidel
** @brief Declaration of the errors and warning emitted by the pre processor.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_ERROR_H
#define SQTPP_ERROR_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {
	class Location;
	class File;
}

namespace sqtpp {
namespace error {

/**
** @brief A log entry.
*/
class Error : public std::exception
{
public:
	/**
	** @brief Severity levels of log messages.
	*/
	enum Severity {
		// Very verbose debug message.
		SEV_UNDEFINED         = 0,
		// Very verbose debug message.
		SEV_DEBUG_VERBOSE     = 10,
		// Debug messages.
		SEV_DEBUG             = 20,
		// Verbose informational message.
		SEV_INFO_VERBOSE      = 30,
		// Informational message.
		SEV_INFO              = 40,
		// Warning level 9.
		SEV_WARNING_L9        = 50,
		// Warning level 8.
		SEV_WARNING_L8        = 51,
		// Warning level 7.
		SEV_WARNING_L7        = 52,
		// Warning level 6.
		SEV_WARNING_L6        = 53,
		// Warning level 5.
		SEV_WARNING_L5        = 54,
		// Warning level 4.
		SEV_WARNING_L4        = 55,
		// Warning level 3.
		SEV_WARNING_L3        = 56,
		// Warning level 2.
		SEV_WARNING_L2        = 57,
		// Warning level 1.
		SEV_WARNING_L1        = 58,
		// Warning level 0.
		SEV_WARNING_L0        = 59,
		// Error
		SEV_ERROR             = 60,
		// Fatal error
		SEV_FATAL             = 70
	};

private:
	// The severity of the log message.
	Error::Severity m_severity;

	// The error code.
	std::wstring     m_sCode;

	// The log message text.
	std::wstring     m_sText;

	// The current prosessor file.
	std::wstring     m_sFile;

	// The line number where the error was found.
	size_t           m_nLine;

	// The column in which the error was found.
	size_t           m_nColumn;

public:
	// Default c'tor.
	Error() : m_severity( Error::SEV_UNDEFINED ), m_nLine(0), m_nColumn(0) {}
	// Initialising c'tor
	Error( Error::Severity severity ) : m_severity( severity ), m_nLine(0), m_nColumn(0) {}
	Error( Error::Severity severity, const wstring& wsCode, const wstring& wsText ) : m_severity( severity ), m_nLine(0), m_nColumn(0), m_sCode(wsCode), m_sText( wsText ) {}
	virtual ~Error() throw() {}

	// Get severity string.
	static const wchar_t* getSeverityString( Error::Severity severity ) throw();

	// Get the stl exception description.
    virtual const char* what() const;


	Error::Severity getSeverity() const throw() { return m_severity; }
	void setSeverity( Error::Severity severity ) throw() { m_severity = severity; }

	const std::wstring& getCode() const throw() { return m_sCode; }
	void setCode( const std::wstring& code ) throw() { m_sCode = code; }

	const std::wstring& getText() const throw() { return m_sText; }
	void setText( const std::wstring& text ) throw() { m_sText = text; }

	const std::wstring& getFilePath() const throw() { return m_sFile; }
	void setFilePath( const std::wstring& file ) throw() { m_sFile = file; }

	size_t getLine() const throw() { return m_nLine; }
	void setLine( size_t line ) throw() { m_nLine = line; }

	size_t getColumn() const throw() { return m_nColumn; }
	void setColumn( size_t column ) throw() { m_nColumn = column; }

	void setFileInfo( const File& file );

protected:
	// Expand error message with given arguments.
	void formatMessage( const wstring& sParameter1, const wstring& sParameter2 = wstring() );
};


/**
** @brief Base class for fatal errors.
*/
class FatalError : public Error
{
public:
	FatalError( const wstring& code, const wstring& text )
	: Error( Error::SEV_FATAL, code, text )
	{
	}
};

/**
** @brief Base class for normal errors.
*/
class NormalError : public Error
{
public:
	NormalError( const wstring& code, const wstring& text )
	: Error( Error::SEV_ERROR, code, text )
	{
	}
};

/**
** @brief Base class for warnings errors.
*/
class Warning : public Error
{
public:
	Warning( const wstring& code, const wstring& text )
	: Error( Error::SEV_WARNING_L1, code, text )
	{
	}

};



/**
** @brief Undefined fatal error.
*/
class UndefinedFatalError : public FatalError
{
public:
	UndefinedFatalError( const wstring& code )
	: FatalError( code , L"Undefined fatal error." )
	{
	}
};


/**
** @brief Internal error: {1}.
*/
class C1001 : public FatalError
{
public:
	C1001( const wstring& errorMessage )
	: FatalError( L"C1001", L"Internal error: {1}." )
	{
		formatMessage( errorMessage );
	}
};

/**
** @brief Internal error: {1}.
**
** Derivation of C1001 for CRT runtime errors.
*/
class C1001_CRT : public C1001
{
public:
	C1001_CRT( int crtErrorNo ) : C1001( _wcserror( crtErrorNo ) )
	{
	}
};





/**
** @brief Maximum error count reached; stopping pre processing.
*/
class C1003 : public FatalError
{
public:
	C1003( const wstring& fileName )
	: FatalError( L"C1003", L"Maximum error count reached; stopping pre processing." )
	{
		formatMessage( fileName );
	}
};


/**
** @brief Unexpected end of file.
*/
class C1004 : public FatalError
{
public:
	C1004()
	: FatalError( L"C1004", L"Unexpected end of file." )
	{
	}
};


/**
** @brief Unknwon command line argument: {1}.
*/
class C1007 : public FatalError
{
public:
	C1007( const wstring& argument )
	: FatalError( L"C1007", L"Unknwon command line argument: {1}." )
	{
		formatMessage( argument );
	}
};


/**
** @brief Missing command line argument. Input file not defined.
*/
class C1008 : public FatalError
{
public:
	C1008()
	: FatalError( L"C1008", L"Missing command line argument. Input file not defined." )
	{
	}
};

/**
** @brief Preprocessor limit exceeded: macros nested too deeply.
*/
class C1009 : public FatalError
{
public:
	C1009()
	: FatalError( L"C1009", L"Preprocessor limit exceeded: macros nested too deeply." )
	{
	}
};


/**
** @brief Undefined / unkown fatal error.
*/ 
class C1010 : public UndefinedFatalError
{
public:
	C1010() : UndefinedFatalError( L"C1010" )
	{
	}
};


/**
** @brief Unmatched parenthesis.
*/ 
class C1012 : public FatalError
{
public:
	C1012() : FatalError( L"C1012", L"Unmatched parenthesis." )
	{
	}
};


/**
** @brief {1} has already been included. Recursive includes are not supported.
*/ 
class C1014 : public FatalError
{
public:
	C1014( const wstring& sFilePath )
	: FatalError( L"C1014", L"{1} has already been included. Recursive includes are not supported." )
	{
		formatMessage( sFilePath );
	}
};




/**
** @brief Excpected macro identifier for preprocessor conditional {1}.
**
** \#ifdef or \#indef found without operand.
*/ 
class C1016 : public FatalError
{
public:
	C1016( const wstring& sDirective )
	: FatalError( L"C1016", L"Excpected macro identifier for preprocessor conditional {1}." )
	{
		formatMessage( sDirective );
	}
};


/**
** @brief Invalid conditional expression.
**
** The conditional expression doesn't evaluate to an integer. @par
** Example: @par
** @code
** #define X '1'
** #if X
** @endcode
*/ 
class C1017 : public FatalError
{
public:
	C1017()
	: FatalError( L"C1017", L"Invalid conditional expression: {1}" )
	{
	}
};

/**
** @brief Invalid expression: missing left operand for operator {1}.
**
** @code
** #if *1 
** #endif
** @endcode
*/ 
class C1017B : public FatalError
{
public:
	C1017B( const wstring& op ): FatalError( L"C1017", L"Invalid expression: missing left operand for operator {1}." )
	{
		formatMessage( op );
	}
};


/**
** @brief Expression is empty.
**
** @code
** #if ()
** #endif
** @endcode
*/ 
class C1017C : public FatalError
{
public:
	C1017C(): FatalError( L"C1017", L"Expression is empty." )
	{
	}
};


/**
** @brief Unexpected \#elif.
*/ 
class C1018 : public FatalError
{
public:
	C1018()
	: FatalError( L"C1018", L"Unexpected #elif." )
	{
	}
};


/**
** @brief Unexpected \#else.
*/ 
class C1019 : public FatalError
{
public:
	C1019()
	: FatalError( L"C1019", L"Unexpected #else." )
	{
	}
};


/**
** @brief Unexpected \#endif.
*/ 
class C1020 : public FatalError
{
public:
	C1020()
	: FatalError( L"C1020", L"Unexpected #endif." )
	{
	}
};



/**
** @brief Unknown preprocessor directive {1}.
*/ 
class C1021 : public FatalError
{
public:
	C1021( const std::wstring& identifier ) : FatalError( L"C1021", L"Unknown preprocessor directive {1}." )
	{
		formatMessage( identifier );
	}
};


/**
** @brief Unknown preprocessor directive {1}.
*/ 
class C1026 : public FatalError
{
public:
	C1026() : FatalError( L"C1026", L"Stack overflow, program too complex." )
	{
	}

	static C1026 instance();
};


/**
** @brief Preprocessor limit exceeded: To many symbols.
*/ 
class C1055 : public FatalError
{
public:
	C1055() : FatalError( L"C1055", L"Preprocessor limit exceeded: To many symbols." )
	{
	}
};


/**
** @brief Unexpected end of file while collecting arguments for the macro {1}.
*/ 
class C1057 : public FatalError
{
public:
	C1057( const std::wstring& identifier ) : FatalError( L"C1057", L"Unexpected end of file while collecting arguments for the macro {1}." )
	{
		formatMessage( identifier );
	}
};


/**
** @brief Unable to open file {1}.
*/ 
class C1068 : public FatalError
{
public:
	C1068( const wstring& fileName ) : FatalError( L"C1068", L"Unable to open file {1}." )
	{
		formatMessage( fileName );
	}
};


/**
** @brief Matching \#endif not found for conditional starting in line.
*/ 
class C1070 : public FatalError
{
public:
	/**
	** @brief Contructor with line number of the unmatched \#if
	** @param line The line where the unmatched conditional block starts.
	*/
	C1070( const Location* pLocation );
};


/**
** @brief Encountered end of file in comment.
*/ 
class C1071 : public FatalError
{
public:
	C1071() : FatalError( L"C1071", L"Encountered end of file in comment." )
	{
	}
};


/**
** @brief Path to long: {1}.
** 
** The length of a file path exceeds _MAX_PATH.
*/ 
class C1081 : public FatalError
{
public:
	C1081( const wstring& fileName )
	: FatalError( L"C1081", L"Path to long: {1}." )
	{
		formatMessage( fileName );
	}
};



/**
** @brief Cannot open file: '{1}': No such file or directory.
*/
class C1083 : public FatalError
{
public:
	C1083( const wstring& fileName )
	: FatalError( L"C1083", L"Cannot open file: '{1}': No such file or directory." )
	{
		formatMessage( fileName );
	}
};


/**
** @brief Error code for errors generated by the \#error directive.
*/
class C1189 : public FatalError
{
public:
	C1189( const wstring& message )
	: FatalError( L"C1189", message )
	{
	}
};


/**
** @brief Unkown fatal error - please contact author.
*/
class C1999 : public FatalError
{
public:
	C1999()
	: FatalError( L"C1999", L"Unkown fatal error - please contact author. " )
	{
	}
	C1999( const std::exception& ex )
		: FatalError( L"C1999", L"Unkown fatal error ({1})- please contact author." )
	{
		const char* pszMessage = ex.what();
		if ( pszMessage != NULL ) {
			size_t   length = strlen( pszMessage );
			wchar_t*    buffer = (wchar_t*)alloca((length + 1)* sizeof( wchar_t ));
			const char* pszMessageNext = NULL;
			wchar_t*    pwszBufferNext = NULL;
			const std::codecvt<wchar_t, char, mbstate_t>& codec = use_facet<codecvt<wchar_t, char, mbstate_t> >( locale() );
			mbstate_t state = 0;

			int result = codec.in( state
					             , pszMessage, &pszMessage[length], pszMessageNext
					             , buffer,     &buffer[length],     pwszBufferNext );
			if ( result == codecvt_base::ok ) {
				wstring msg( buffer );
				formatMessage( msg );
			} else {
				wstring msg( L"Unkown error" );
				formatMessage( msg );
			}
		}
	}


	static C1999 instance();
};


/**
** @brief String exceeds end of line.
*/
class C2001 : public NormalError
{
public:
	C2001()
	: NormalError( L"C2001", L"String exceeds end of line." )
	{
	}
};

/**
** @brief \#include exceeds end of line.
*/
class C2003 : public NormalError
{
public:
	C2003()
	: NormalError( L"C2003", L"#include exceeds end of line." )
	{
	}
};


/**
** @brief Missing file path for \#include directive. Found {1}.
*/
class C2006 : public NormalError
{
public:
	C2006( const wstring& expresion )
	: NormalError( L"C2006", L"Missing file path for #include directive. Found {1}." )
	{
		formatMessage( expresion );
	}
};


/**
** @brief Missing identifier for \#define directive.
*/
class C2007 : public Warning
{
public:
	C2007()
	: Warning( L"C2007", L"Missing identifier for #define directive." )
	{
	}
};


/**
** @brief Macro argument specified more than once: {1}.
*/
class C2009 : public NormalError
{
public:
	C2009( const wstring& expresion )
	: NormalError( L"C2009", L"Macro argument specified more than once: {1}." )
	{
		formatMessage( expresion );
	}
};


/**
** @brief Unexpected expression in macro parameter list: '{1}'.
*/
class C2010 : public NormalError
{
public:
	C2010( const wstring& expresion )
	: NormalError( L"C2010", L"Unexpected expression in macro parameter list: '{1}'" )
	{
		formatMessage( expresion );
	}
};

/**
** @brief The string {1} is to long for a integer expression evaluation.
*/
class C2015 : public NormalError
{
public:
	C2015( wstring str ) : NormalError( L"C2015", L"The string {1} is to long for an integer expression evaluation." )
	{
		formatMessage( str );
	}
};


/**
** @brief Division by zero.
*/
class C2124 : public NormalError
{
public:
	C2124() : NormalError( L"C2124", L"Division by zero." )
	{
	}
};

/**
** @brief Missing first argument for the \#\# operator.
*/
class C2160 : public NormalError
{
public:
	C2160() : NormalError( L"C2160", L"Missing first argument for the ## concate operator" )
	{
	}
};


/**
** @brief Missing second argument for the \#\# operator.
*/
class C2161 : public NormalError
{
public:
	C2161()
	: NormalError( L"C2161", L"Missing second argument for the ## concate operator" )
	{
	}
};


/**
** @brief The token following a stringizing operator (#) has to be a macro argument.
*/
class C2162A : public NormalError
{
public:
	C2162A()
	: NormalError( L"C2162", L"The token following a stringizing operator (#) has to be a macro argument." )
	{
	}
};


/**
** @brief The token following a charizing operator (#@) has to be a macro argument.
*/
class C2162B : public NormalError
{
public:
	C2162B()
	: NormalError( L"C2162", L"The token following a charizing operator (#@) has to be a macro argument." )
	{
	}
};


/**
** @brief To many parameters for macro '{1}'.
*/
class C4002 : public Warning
{
public:
	C4002( const wstring& macroIdentifier )
	: Warning( L"C4002", L"To many parameters for macro '{1}'" )
	{
		formatMessage( macroIdentifier );
	}
};


/**
** @brief Not enough parameters for macro {1}.
*/
class C4003 : public Warning
{
public:
	C4003( const wstring& macroIdentifier )
	: Warning( L"C4003", L"Not enough parameter for macro {1}." )
	{
		formatMessage( macroIdentifier );
	}
};



/**
** @brief Redefinition of macro {1}. The macro has already been defined in {2}.
*/
class C4005 : public Warning
{
public:
	C4005( const wstring& macroIdentifier, const wstring& prevDefinition )
		: Warning( L"C4005", L"Redefinition of macro {1}. The macro has already been defined in {2}." )
	{
		formatMessage( macroIdentifier, prevDefinition );
		// formatMessage( );
	}
};


/**
** @brief Missing identifier for the \#undef directive.
*/
class C4006 : public Warning
{
public:
	C4006()
	: Warning( L"C4006", L"Missing identifier for the #undef directive." )
	{
	}
};


/**
** @brief Continue line character found inside a line comment.
*/
class C4010 : public Warning
{
public:
	C4010() : Warning( L"C4010", L"Continue line character (\\) found inside a line comment." )
	{
	}
};

/**
** @brief Unexcpeted token found while parseing expression. 
*/
class C4067 : public Warning
{
public:
	C4067( const wstring& expression )
	: Warning( L"C4067", L"Unexpected token {1} found while parseing expression." )
	{
		formatMessage( expression );
	}
};


/**
** @brief Unexcpeted token found while parseing expression. Expected binary operator or end of expression.
*/
class C4067B : public Warning
{
public:
	C4067B()
	: Warning( L"C4067", L"Unexpected token found while parseing expression. Expected binary operator or end of expression." )
	{
	}
};

/**
** @brief Unexpected text following preprocessor directive - expected a newline.
*/
class C4067C : public Warning
{
public:
	C4067C()
	: Warning( L"C4067", L"Unexpected text following preprocessor directive - expected a newline." )
	{
	}
};



/**
** @brief Can't \#undef buildin macro {1}.
*/
class C4117 : public Warning
{
public:
	C4117( const wstring& identifier )
	: Warning( L"C4117", L"Can't #undef buildin macro {1}." )
	{
		formatMessage( identifier );
	}
};


/**
** @brief No input file specified in the command line arguments.
*/
class D8003 : public FatalError
{
public:
	D8003()
	: FatalError( L"D8003", L"No input file specified in the command line arguments." )
	{
	}
};

/**
** @brief Missing argument for the command line option {1}.
*/
class D8004 : public FatalError
{
public:
	D8004( const wstring& argument )
	: FatalError( L"D8004", L"Missing argument for the command line option {1}." )
	{
		formatMessage( argument );
	}
};


/**
** @brief Invalid argument {1}.
*/
class D9002 : public Warning 
{
public:
	D9002( const wstring& argument )
	: Warning( L"D9002", L"Invalid argument {1}." )
	{
		formatMessage( argument );
	}
};


/**
** @brief {1} requires {2}; option ignored.
*/
class D9007 : public Warning 
{
public:
	D9007( const wstring& argument, const wstring& option )
		: Warning( L"D9007", L"{1} requires {2}; option ignored." )
	{
		formatMessage( argument, option );
	}
};


/**
** @brief Unknown file type {1}.
*/
class D9024 : public Warning 
{
public:
	D9024( const wstring& argument )
	: Warning( L"D9024", L"Unkonwn file type {1}." )
	{
		formatMessage( argument );
	}
};




} // namespace error
} // namespace sqtpp

// Error stream out operator.
std::wostream& operator<<(std::wostream& os, const sqtpp::error::Error::Severity& severity );

// Error stream out operator.
std::wostream& operator<<(std::wostream& os, const sqtpp::error::Error& error );

#endif // SQTPP_ERROR_H

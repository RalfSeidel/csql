#include "stdafx.h"
#include <cassert>
#include <sys/types.h>
#include <sys/stat.h>
#include <mbctype.h>
#include <strstream>
#include "CodePage.h"
#include "CodePageConverter.h"
#include "Streams.h"
#include "Exceptions.h"
#include "Error.h"
#include "Token.h"
#include "Util.h"
#include "File.h"

namespace sqtpp {

struct File::Data {
	/// The instance counter/id
	int             m_nInstanceId;

	/// Number of references to the file data
	int             m_nRefCount;

	/// The path under which the file was openend.
	std::wstring    m_relativePath;

	/// The full qualified path of the input file.
	std::wstring    m_absolutePath;

	/// The default new line characters in this file.
	std::wstring    m_sDefaultNewline;

	/// The file locale (code page).
	std::locale     m_locale;

	/// The input stream.
	std::istream*   m_pInternalStream;

	/// The input stream.
	std::wistream*  m_pExternalStream;

	/// True if the interternal stream is attached only i.e. 
	/// memory managment is done by the caller.
	bool            m_isAttached;

	/// Flag indicatin if this file should be read only once.
	bool            m_includeOnce;

	/// Current line number read.
	size_t          m_nLineNumber;

	/// Current column read (1 based).
	size_t          m_nLineColumn;

	/// Counter for the file scope __COUNTER__ macro.
	int             m_nCounter;

	/// The first (non white space) token encountered in this file.
	Token           m_firstToken;
	/// The second (non white space) token encountered in this file.
	Token           m_secondToken;
	/// The third (non white space) token encountered in this file.
	Token           m_thirdToken;
	/// The fourth (non white space) token encountered in this file.
	Token           m_fourthToken;
	/// The last (non white space) token encountered in this file.
	Token           m_lastToken;

	/// Construtor.
	Data()
	{
		m_nRefCount         = 1;
		m_pInternalStream   = NULL;
		m_pExternalStream   = NULL;
		m_isAttached        = false;
		m_includeOnce       = false;
		m_locale            = std::locale::classic();
		m_nLineNumber       = 0;
		m_nLineColumn       = 0;
		m_nCounter          = 0;
		m_firstToken        = TOK_UNDEFINED;
		m_secondToken       = TOK_UNDEFINED;
		m_thirdToken        = TOK_UNDEFINED;
		m_fourthToken       = TOK_UNDEFINED;
		m_lastToken         = TOK_UNDEFINED;
	}
	/// Destructor.
	~Data()
	{
		if ( m_isAttached ) {
			if ( m_pInternalStream != NULL ) {
				// The internal stream was attached. 
				// The external converting stream is managed by this object.
				delete m_pExternalStream;
			}
		} else {
			delete m_pExternalStream;
			delete m_pInternalStream;
		}
	}
private:
	Data( const Data& );
	Data& operator= ( const Data& );
};

int File::m_nInstanceCounter = 0;

/**
** @brief Default constructor.
*/
File::File()
{
	m_pData = new Data();
	m_pData->m_nInstanceId  = ++m_nInstanceCounter;
	m_pData->m_nRefCount    = 1;
}

/**
** @brief Destructor.
*/
File::~File() throw()
{
	if ( --m_pData->m_nRefCount == 0 ) {
		delete m_pData;
	}
#if _DEBUG
	m_pData = NULL;
#endif
}

/**
** @brief Copy construtor.
*/
File::File( const File& that )
{
	that.m_pData->m_nRefCount++;
	this->m_pData = that.m_pData;
}

/**
** @brief Assignment operator.
*/
File& File::operator=( const File& that )
{
	that.m_pData->m_nRefCount++;
	if ( --m_pData->m_nRefCount == 0 ) {
		delete m_pData;
	}
	this->m_pData = that.m_pData;

	return *this;
}


/**
** @brief Comparison operator.
*/
bool File::operator< ( const File& that ) const
{
	return this->m_pData->m_absolutePath < that.m_pData->m_absolutePath;
}


/**
** @brief Get the instance id
*/
int File::getInstanceId() const throw()
{
	return m_pData->m_nInstanceId;
}

/**
** @brief Check if this file should be opened and read only once.
*/
bool File::isIncludeOnce() const throw()    
{ 
	return m_pData->m_includeOnce; 
}

/**
** @brief Set flag if this file should be opened and read only once.
*/
void File::setIncludeOnce( bool includeOnce ) throw() 
{ 
	m_pData->m_includeOnce = includeOnce; 
}

/**
** @brief Get the file path.
*/
const wstring& File::getPath() const throw()
{
	return m_pData->m_relativePath;
}


/**
** @brief Get the default new line charactes.
*/
const wstring& File::getDefaultNewLine() const throw()
{
	return m_pData->m_sDefaultNewline;
}

/**
** @brief Set the default new line charactes.
*/
void File::setDefaultNewLine( const wstring& sNewLine ) throw()
{
	m_pData->m_sDefaultNewline = sNewLine;
}


/**
** @brief Get the file locale (code page).
*/
const locale& File::getLocale() const throw()
{
	/*
	if ( m_pData->m_pInternalStream != NULL ) {
		return m_pData->m_pInternalStream->rdbuf()->getloc();
	} else {
		return m_pData->m_pExternalStream->rdbuf()->getloc();
	}
	*/
	return m_pData->m_locale;
}


/**
** @brief Get the current line number.
*/
size_t File::getLine() const throw()
{
	return m_pData->m_nLineNumber;
}

/**
** @brief Increment the current line number.
**
** @note Incementing the line number implicit resets the column 
**       to one.
*/
void File::incLine()
{
	++m_pData->m_nLineNumber;
	m_pData->m_nLineColumn = 1;
}

/**
** @brief Get the current column.
*/
size_t File::getColumn() const throw()
{
	return m_pData->m_nLineColumn;
}

/**
** @brief Increment the current column.
*/
void File::incColumn( size_t amount ) const throw()
{
	m_pData->m_nLineColumn+= amount;
}


/**
** @brief Helper for the __COUNTER__ maro.
*/
int File::getNextCounter() const throw()
{
	int cntr = m_pData->m_nCounter++;
	return cntr;
}


/**
** @brief Set the last (non white space) token found by the processor.
*/
void File::setLastToken( Token token )
{
	bool isWhite = token == TOK_BLOCK_COMMENT
	            || token == TOK_LINE_COMMENT
	            || token == TOK_SPACE
				|| token == TOK_NEW_LINE
				|| token == TOK_END_OF_FILE
				|| token == TOK_DIR_MESSAGE
				|| token == TOK_DIR_PRAGMA;

	if ( !isWhite ) {
		if ( m_pData->m_firstToken == TOK_UNDEFINED ) {
			m_pData->m_firstToken = token;
		} else if ( m_pData->m_secondToken == TOK_UNDEFINED ) {
			m_pData->m_secondToken = token;
		} else if ( m_pData->m_thirdToken == TOK_UNDEFINED ) {
			m_pData->m_thirdToken = token;
		} else if ( m_pData->m_fourthToken == TOK_UNDEFINED ) {
			m_pData->m_fourthToken = token;
		}
		m_pData->m_lastToken = token;
	}
}

/**
** @brief Check if the first, second and last (non white space) token found is 
** a \#ifndef, \#define / \#endif pair. If this is true sqtpp will include the file only
** once.
*/
bool File::isAutoIncludedOnce()
{
	return m_pData->m_firstToken  == TOK_DIR_IFNDEF
		&& m_pData->m_secondToken == TOK_IDENTIFIER
		&& m_pData->m_thirdToken  == TOK_DIR_DEFINE
		&& m_pData->m_fourthToken == TOK_IDENTIFIER
		&& m_pData->m_lastToken   == TOK_DIR_ENDIF;
}



/**
** @brief Attach to a unicode wistream.
**
** @param is The stream to attach to. 
*/
std::wistream& File::attach( std::wistream& is )
{
	m_pData->m_pExternalStream = &is;
	m_pData->m_pInternalStream = NULL;
	m_pData->m_nLineNumber = 1;
	m_pData->m_nLineColumn = 1;
	m_pData->m_isAttached  = true;

	int exceptions = ios::badbit;
#	if	_MSC_VER > 10
	exceptions|= ios::_Hardfail;
#	endif

	m_pData->m_pExternalStream->exceptions( exceptions );

	return *m_pData->m_pExternalStream;
}

/**
** @brief Get the full absolute path for the given relative file path.
** 
** @param filePath The relative file path.
** @returns Full / absolute file path (if the file exists).
*/
const wstring File::getFullPath( const wstring& filePath ) /* throw( Error ) */
{
	wchar_t  wcBuffer[_MAX_PATH];
	wchar_t* pwszFullPath = _wfullpath( wcBuffer, filePath.c_str(), _MAX_PATH );
	if ( pwszFullPath == NULL ) {
		// Internal error.
		throw error::C1001_CRT( errno );
	} else {
		// Translate back slashes into forward slashes.
		for ( wchar_t* pch = pwszFullPath; *pch != L'\0'; ++pch ) {
			if ( *pch == L'\\' ) {
				*pch = L'/';
			}
		}

		return wstring( pwszFullPath );
	}
}


/**
** @brief Get the full absolute path of a file located in the given directory.
*/
const wstring File::getFullPath( const wstring& directoryPath, const wstring& filePath ) /* throw( Error ) */
{
	wchar_t  wcBuffer1[_MAX_PATH];
	wchar_t  wcBuffer2[_MAX_PATH];

	memset( wcBuffer1, 0, sizeof( wcBuffer1 ) );
	memset( wcBuffer2, 0, sizeof( wcBuffer2 ) );
	_wmakepath( wcBuffer1, NULL, directoryPath.c_str(), filePath.c_str(), NULL );

	wchar_t* pwszFullPath = _wfullpath( wcBuffer2, wcBuffer1, _MAX_PATH );
	if ( pwszFullPath == NULL ) {
		// Internal error.
		throw error::C1001_CRT( errno );
	} else {
		return wstring( pwszFullPath );
	}
}


/**
** @brief Get the full absolute directory path of the given file path.
** 
** @param fullPath The relative file path.
** @returns The drive and directory part of the path.
*/
const wstring File::getDirectory( const wstring& fullPath ) /* throw( Error ) */
{
	wchar_t device[_MAX_DRIVE];
	wchar_t directory[_MAX_DIR];
	_wsplitpath( fullPath.c_str(), device, directory, NULL, NULL );

	wstring directoryPath = getFullPath( wstring( device ) + wstring( directory ) );

	return directoryPath;
}


/**
** @brief Check if the file exists.
** 
** @param directoryPath The path of the directory.
** @returns True if the path exists and is a directory.
*/
bool File::isDirectory( const wstring& directoryPath ) throw()
{
	struct _stat  fileInfo;
	memset( &fileInfo, 0, sizeof( fileInfo ) );

	int     crtrc = _wstat( directoryPath.c_str(), &fileInfo );
	if ( crtrc == 0 && (fileInfo.st_mode & _S_IFDIR) != 0 ) {
		return true;
	} else {
		return false;
	}
}


/**
** @brief Check if the file exists.
** 
** @param filePath The file path.
** @returns True if the file exists.
*/
bool File::isFile( const wstring& filePath ) throw()
{
	struct _stat  fileInfo;
	memset( &fileInfo, 0, sizeof( fileInfo ) );

	int     crtrc = _wstat( filePath.c_str(), &fileInfo );
	if ( crtrc == 0 && (fileInfo.st_mode & _S_IFREG) != 0 ) {
		return true;
	} else {
		return false;
	}
}


/**
** @brief Check if the file exists and can be openend.
** 
** @param filePath The file path.
** @returns The absolute path of the file.
*/
const wstring File::checkFile( const wstring& filePath ) /* throw( Error ) */
{
	struct _stat fileInfo;
	int    crtrc;
	memset( &fileInfo, 0, sizeof( fileInfo ) );

	if ( filePath.length() == 0 ) {
		throw RuntimeError( "Filename not specified" );
	} else if ( filePath.length() > _MAX_PATH ) {
		// Path to long: {1}.
		throw error::C1081( filePath );
	}

	crtrc = _wstat( filePath.c_str(), &fileInfo );
	switch ( crtrc ) {
		case 0:
			// Check if really a file.
			if ( (fileInfo.st_mode & _S_IFREG) == 0 ) {
				// No such file.
				throw error::C1083( filePath );
			}
			if ( (fileInfo.st_mode & _S_IREAD) == 0 ) {
				// Unable to open file {1}.
				throw error::C1068( filePath );
			}

			break;
		case -1:
			// File not found.
			throw error::C1083( filePath );
			break;
		default:
			// Internal error.
			throw error::C1001_CRT( crtrc );
	}


	wchar_t  wcBuffer[_MAX_PATH];
	wchar_t* pwszFullPath = _wfullpath( wcBuffer, filePath.c_str(), _MAX_PATH );
	if ( pwszFullPath == NULL ) {
		crtrc = *_errno();
		// Internal error.
		throw error::C1001_CRT( crtrc );
	}
	wstring sFullPath( pwszFullPath );
	//free( pwzsFullPath );
	return sFullPath;
}



/**
** @brief Find the file in one of the given directories.
** 
** @param   filePath The relative file path as mentioned in a \#include directive.
** @param   includeDirectories The directories to search the file for.
** @param   bSearchCwd Set to true if the current directory should be checked to.
**          If set the methode will lookup in the directory of this file and 
**          in the process working directory first.
** @returns True if the file exists. False if not.
*/
std::wstring File::findFile( const wstring& filePath, const vector<wstring>& includeDirectories, bool bSearchCwd ) const
{
	wstring sFullPath;
	if ( bSearchCwd ) {
		// Lookup inside this file directory.
		wstring sThisDirectory = File::getDirectory( this->getPath() );
		wstring sCheckPath     = sThisDirectory + filePath;
		if ( isFile( sCheckPath ) ) {
			sFullPath = sCheckPath;
			return sFullPath;
		}

		if ( isFile( filePath ) ) {
			sFullPath = getFullPath( filePath );
			return sFullPath;
		}
	}
	for ( vector<wstring>::const_iterator it = includeDirectories.begin(); it != includeDirectories.end(); ++it ) {
		const wstring& sDirectory = *it; 
		const wstring  sCheckPath = File::getFullPath( sDirectory, filePath );
		if ( isFile( sCheckPath ) ){
			sFullPath = sCheckPath;
			return sFullPath;
		}
	}

	return wstring();
}

/**
** @brief Get the file content stream.
** 
** @todo Code page detection.
** 
** @param fileName The file path.
** @param defaultCodePageId The default code page for files having no bom at their beginning.
*/
std::wistream& File::open( const std::wstring& fileName, const CodePageId defaultCodePageId )
{
	if ( m_pData->m_pExternalStream ) {
		throw LogicError( "File already open" );
	}

	// Check if the exists and is accessable.
	wstring sFullPath = checkFile( fileName );

	const CodePageInfo* const pDefaultCodePage = CodePageInfo::findCodePageInfo( defaultCodePageId );
	if ( pDefaultCodePage == NULL ) {
		throw error::C1205( defaultCodePageId );
	}
	const CodePageInfo* const pCodePage = detectCodePage( fileName, pDefaultCodePage );
	const CodePageId codePageId = pCodePage->getCodePageId();

	if ( codePageId == CPID_UTF32 || codePageId == CPID_UTF32BE ) {
		//throw NotSupportedError( "The code page is not supported" );
		throw NotSupportedError();
	}

	std::wifstream* pInnerStream = new std::wifstream();
	const locale& fileLocale = pCodePage->getLocale();
	pInnerStream->imbue( fileLocale ) ;
	pInnerStream->open( fileName.c_str(), std::ios_base::in | std::ios_base::binary  );

	const char* fileBom = pCodePage->getFileBom();
	if ( pCodePage->getFileBom() != NULL ) {
		size_t bomLength = strlen( pCodePage->getFileBom() );
		pInnerStream->rdbuf()->pubseekpos( bomLength );
	}
	attach( *pInnerStream );


#	ifdef _DEBUG
	const locale locCheck = pInnerStream->rdbuf()->getloc();
	const string locName = locCheck.name();
	assert( locName == fileLocale.name() );
#	endif


	m_pData->m_relativePath = fileName;
	m_pData->m_absolutePath = sFullPath;
	m_pData->m_isAttached   = false;

	return *m_pData->m_pExternalStream;
}

/**
** @brief Get the file content stream.
*/
std::wistream& File::open( const std::wstring& fileName )
{
	return open( fileName, CPID_UNDEFINED );
}


/**
** @brief Get the file content stream.
*/
std::wistream& File::getStream()
{
	if ( m_pData->m_pExternalStream == NULL ) {
		throw std::logic_error( "The input stream is undefined." );
	}
	return *m_pData->m_pExternalStream;
}


/**
** @brief Auto detect the character set of the file.
**
** @param fileName The path of the file to open.
**
** @todo Determine code page of non unicode files.
*/
const CodePageInfo* File::detectCodePage( const std::wstring& fileName, const CodePageInfo* pDefaultCodePage  )
{
	std::ifstream ifs;

	ifs.open( fileName.c_str(), ios::in | ios::binary  );
	const CodePageInfo* pCodePageInfo = detectCodePageByBom( ifs );
	ifs.close();

	if ( pCodePageInfo == NULL && pDefaultCodePage == NULL ) {
		// TODO: detect code page of non unicode files.
	}

	return pCodePageInfo == NULL ? pDefaultCodePage : pCodePageInfo ;
}

/**
** @brief Read the first bytes of a file and try to determine it's encoding
** by examinig the first bytes of the file.
*/
const CodePageInfo* File::detectCodePageByBom( std::ifstream& fileStream )
{
	const CodePageInfo** codePageInfos = CodePageInfo::getCodePages();
	char  fileStart[512];

	fileStream.seekg( ios_base::beg );
	fileStream.read( fileStart, sizeof( fileStart ) );
	size_t bytesRead = fileStream.gcount();
	if ( bytesRead < sizeof(fileStart) ) {
		fileStart[bytesRead] = '\0';
	} else {
		fileStart[bytesRead-1] = '\0';
	}

	// Determine max length to read necessary to determine the encoding.
	size_t maxBomLength = 0;
	for ( const CodePageInfo* const* ppCodePageInfo = codePageInfos; *ppCodePageInfo != NULL; ppCodePageInfo++ ) {
		const CodePageInfo& cpInfo = **ppCodePageInfo;
		const char* cpBom = cpInfo.getFileBom();
		if ( cpBom == NULL )
			continue;
		size_t length = strlen( cpBom );
		if ( strncmp( cpBom, fileStart, min( length, bytesRead ) ) == 0 )
			return *ppCodePageInfo;
	}
	return NULL;
}




} // namespace

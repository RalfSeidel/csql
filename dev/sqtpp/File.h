/**
** @file
** @author Ralf Seidel
** @brief Declaration of the include #sqtpp::File information.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_FILE_H
#define SQTPP_FILE_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

class Error;
class CodePageInfo;
struct Range;
enum Token;
enum CodePageId;

class File
{
private:
	static int m_nInstanceCounter;

	// Structur of file data.
	struct Data;

	// The file data.
	Data*  m_pData;


public:
	// Initializing c'tor.
	File();

	// Destructor.
	~File() throw();

	// Copy constructor.
	File( const File& that );

	// Assignment operator.
	File& operator= ( const File& that );

	// Comparison operator.
	bool operator< ( const File& that ) const;

	// Get the full absolute path for the given relative file path.
	static const wstring getFullPath( const wstring& filePath ) /* throw( Error ) */;

	// Get the full absolute path of a file located in the given directory.
	static const wstring getFullPath( const wstring& directoryPath, const wstring& filePath ) /* throw( Error ) */;

	// Get the full absolute directory path of the given file path.
	static const wstring getDirectory( const wstring& fullPath ) /* throw( Error ) */;

	// Check if directory exists.
	static bool isDirectory( const wstring& directoryPath ) throw();

	// Check if the specified file exists.
	static bool isFile( const wstring& filePath ) throw();

	// Check if file exists and is readable - throw error if not.
	static const wstring checkFile( const wstring& filePath ) /* throw( Error ) */;

	// Open the file.
	std::wistream& open( const std::wstring& fileName, const CodePageId defaultCodePageId );

	// Open the file.
	std::wistream& open( const std::wstring& fileName );

	// Attach to the given wistream
	std::wistream& attach( std::wistream& is );

	// Attach to the given istream
	std::wistream& attach( const std::locale& loc, std::istream& is );

	// Get the file input stream.
	std::wistream& getStream();

	// Get the instance id
	int getInstanceId() const throw();

	// Get the nesting include level of this file.
	int getIncludeLevel() const throw();

	// Set the nesting include level of this file.
	void setIncludeLevel( int includeLevel ) throw();

	// Check if this file is the root file processed.
	bool isRootFile() const throw();

	// Check if this file should be opened and read only once.
	bool isIncludeOnce() const throw();

	// Set flag if this file should be opened and read only once.
	void setIncludeOnce( bool includeOnce ) throw();

	// Get the default new line charactes.
	const wstring& getDefaultNewLine() const throw();

	// Set the default new line charactes.
	void setDefaultNewLine( const wstring& sNewLine ) throw();

	// Get the file path.
	const wstring& getPath() const throw();

	// Get the file locale (code page).
	const locale& getLocale() const throw();

	// Get the current position in the file.
	size_t getPosition() const throw();

	// Set the current position in the file.
	void setPosition( size_t nPosition ) throw();

	// Get the the range of the current token measured in characters.
	const Range& getCurrentTokenRange() const throw();

	// Get the current line number.
	size_t getLine() const throw();

	// Increment the current line number.
	void incLine();

	// Get the current column.
	size_t getColumn() const throw();

	// Increment the current column.
	void incColumn( size_t amount ) const throw();

	// Get the file counter (for the __COUNTER__ macro).
	int  getNextCounter() const throw();

	// Set the last (non white space) token found by the processor.
	void setLastToken( Token token, const Range& tokenRange );

	// Check if the first, second and last (non white space) token found is a #ifndef, #define / #endif pair.
	bool isAutoIncludedOnce();

private:
	// Auto detect the character set of the file.
	static const CodePageInfo* detectCodePage( const std::wstring& fileName, const CodePageInfo* pDefaultCodePage );

	// Auto detect the character set of the file.
	static const CodePageInfo* detectCodePageByBom( std::ifstream& fileStream );
};

/**
** @brief Stack of file objects.
*/
class FileStack : public std::stack<File>
{
public:
	const container_type& container()
	{
		return c;
	}
};

} // namespace



#endif // SQTPP_FILE_H

/**
** @file
** @author Ralf Seidel
** @brief Declaration of the #sqtpp::FileFinder tool class.
**
** © 2010 by SQL Service GmbH Wuppertal
*/
#ifndef SQTPP_FILE_FINDER_H
#define SQTPP_FILE_FINDER_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

class FileFinder
{
private:
	/// The directory of the current file.
	const std::wstring currentFileDirectory;

	/// The include directories.
	const std::vector<std::wstring>& includeDirectories;

	/// Option flag to search files in the current directory
	/// or in the include directories only.
	bool bFindInCurrentDirectory;

private:
	/// Not implemented copy constructor.
	FileFinder( const FileFinder& );
	/// Not implemented assignment operator.
	FileFinder& operator= ( const FileFinder& );
public:

	/// Find files in the include directory.
	FileFinder( const std::vector<std::wstring>& includeDirectories );

	/// File files in the include directory and additionally in the directory of the current file.
	FileFinder( const std::vector<std::wstring>& includeDirectories, const std::wstring& currentFilePath );

	// Find the specified file and return it's full path.
	std::wstring findFile( const std::wstring& filePath );
};


} // namespace

#endif // SQTPP_FILE_FINDER_H

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
	/// The include directories.
	const std::vector<std::wstring>& includeDirectories;

	/// Option flag to search files in the current directory
	/// or in the include directories only.
	bool bUseCurrentDirectory;

private:
	/// Not implemented copy constructor.
	FileFinder( const FileFinder& );
	/// Not implemented assignment operator.
	FileFinder& operator= ( const FileFinder& );
public:
	FileFinder( const std::vector<std::wstring>& includeDirectories );


	/// Set the option to find files specified
	/// in a location relative to the current working directory.
	void useCurrentDirectory( bool bUseCurrentDirectory )
	{
		this->bUseCurrentDirectory = bUseCurrentDirectory;
	}

	// Find the specified file and return it's full path.
	std::wstring findFile( const std::wstring& filePath );
};


} // namespace

#endif // SQTPP_FILE_FINDER_H

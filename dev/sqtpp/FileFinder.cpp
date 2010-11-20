/**
** @file
** @author Ralf Seidel
** @brief Implementation of the #sqtpp::FileFinder tool class.
**
** © 2010 by SQL Service GmbH Wuppertal
*/
#include "StdAfx.h"
#include "File.h"
#include "FileFinder.h"

namespace sqtpp {


FileFinder::FileFinder( const std::vector<wstring>& includeDirectories )
: includeDirectories( includeDirectories )
, bFindInCurrentDirectory( false )
{
}

FileFinder::FileFinder( const std::vector<wstring>& includeDirectories, const std::wstring& currentFilePath )
: includeDirectories( includeDirectories )
, currentFileDirectory( File::getDirectory( currentFilePath ) )
, bFindInCurrentDirectory( true )
{
}

std::wstring FileFinder::findFile( const wstring& filePath )
{
	if ( bFindInCurrentDirectory ) {
		// Lookup relative to the directory of the current file.
		if ( !currentFileDirectory.empty()  ) {
			wstring sFullPath = File::getFullPath( currentFileDirectory, filePath );
			if ( File::isFile( sFullPath ) ){
				return sFullPath;
			}
		}

		// Lookup relative to the current working directory.
		if ( File::isFile( filePath ) ) {
			wstring sFullPath = File::getFullPath( filePath );
			return sFullPath;
		}
	} 

	for ( vector<wstring>::const_iterator it = includeDirectories.begin(); it != includeDirectories.end(); ++it ) {
		const wstring& sDirectory = *it; 
		const wstring  sFullPath = File::getFullPath( sDirectory, filePath );
		if ( File::isFile( sFullPath ) ){
			return sFullPath;
		}
	}

	return wstring();
}



}
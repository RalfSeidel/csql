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
{
}

std::wstring FileFinder::findFile( const wstring& filePath )
{
	wstring sFullPath;
	if ( bUseCurrentDirectory ) {
		// Lookup relative to the current working directory.
		if ( File::isFile( filePath ) ) {
			sFullPath = File::getFullPath( filePath );
			return sFullPath;
		}

		// Lookup relative to the path of the including file
		/*
		wstring sThisDirectory = File::getDirectory( this->getPath() );
		wstring sCheckPath     = sThisDirectory + filePath;
		if ( File::isFile( sCheckPath ) ) {
			sFullPath = sCheckPath;
			return sFullPath;
		}
		*/

	}
	for ( vector<wstring>::const_iterator it = includeDirectories.begin(); it != includeDirectories.end(); ++it ) {
		const wstring& sDirectory = *it; 
		const wstring  sCheckPath = File::getFullPath( sDirectory, filePath );
		if ( File::isFile( sCheckPath ) ){
			sFullPath = sCheckPath;
			return sFullPath;
		}
	}

	return wstring();
}



}
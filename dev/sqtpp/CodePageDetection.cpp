/**
** @file
** @author Ralf Seidel
** @brief Implementate of the the custom code page converter used 
**        for converting wide character strings in the input and output files.
**
**
** © 2009 by Heinrich und Seidel GbR Wuppertal.
*/
#include "stdafx.h"
#include "Error.h"
#include "CodePage.h"
#include "CodePageDetection.h"

namespace sqtpp {


/**
** @brief Auto detect the character set of the file.
**
** @param fileName The path of the file to open.
** @param defaultCodePageId The default code page if no unique clue to a file code page was found.
*/
const CodePageInfo* CodePageDetection::detectCodePage( const std::wstring& fileName, const CodePageId defaultCodePageId )
{
	const CodePageInfo* const pDefaultCodePage = CodePageInfo::findCodePageInfo( defaultCodePageId );
	if ( pDefaultCodePage == NULL ) {
		throw error::C1205( defaultCodePageId );
	}
	const CodePageInfo* const pCodePage = detectCodePage( fileName, pDefaultCodePage );
	return pCodePage;
}

/**
** @brief Auto detect the character set of the file.
**
** @param fileName The path of the file to open.
**
** @todo Determine code page of non unicode files.
*/
const CodePageInfo* CodePageDetection::detectCodePage( const std::wstring& fileName, const CodePageInfo* pDefaultCodePage  )
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
const CodePageInfo* CodePageDetection::detectCodePageByBom( std::ifstream& fileStream )
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

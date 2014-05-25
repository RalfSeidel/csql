/**
** @file
** @author Ralf Seidel
** @brief Declaration of the service class defined to detect
** the code page of a file.
**
** © 2010 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_CODEPAGEDETECTION_H
#define SQTPP_CODEPAGEDETECTION_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

enum CodePageId;
class CodePageInfo;

class CodePageDetection
{
private:
	/// Private constructor because this class defines static service methods only.
	CodePageDetection();

public:

	/// Auto detect the character set of the file.
	static const CodePageInfo* detectCodePage( const std::wstring& fileName, const CodePageId defaultCodePageId );
private:
	static const CodePageInfo* detectCodePage( const std::wstring& fileName, const CodePageInfo* pDefaultCodePage );
	static const CodePageInfo* detectCodePageByBom( std::ifstream& fileStream );

	static size_t min( size_t a, size_t b )
	{
		return a < b ? a : b;
	}
};


} // namespace

#endif // SQTPP_CODEPAGEDETECTION_H

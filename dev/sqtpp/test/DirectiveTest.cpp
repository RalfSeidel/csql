#include "stdafx.h"
#include <cassert>
#include "../Directive.h"
#include "DirectiveTest.h"

namespace sqtpp {
namespace test {

DirectiveTest::DirectiveTest()
{
}

/**
** @brief Run all defined tests for this class.
*/
void DirectiveTest::run()
{
	DirectiveTest test;

	test.getDirectiveTest();
	test.findDirectiveInfoTest();
}

/**
** @brief Test the DirectiveInfo::getDirective methode.
*/
void DirectiveTest::getDirectiveTest()
{
	Directive      d;
	const wchar_t* i;

	i = L"ifdef";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_IFDEF );

	i = L"define";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_DEFINE );

	i = L"undef";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_UNDEF );

	i = L"undefall";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_UNDEFALL );

	i = L"if";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_IF );

	i = L"ifdef";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_IFDEF );

	i = L"ifndef";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_IFNDEF );

	i = L"elif";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_ELIF );

	i = L"else";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_ELSE );

	i = L"endif";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_ENDIF );

	i = L"include";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_INCLUDE );

	i = L"line";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_LINE );

	i = L"error";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_ERROR );

	i = L"pragma";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_PRAGMA );

	i = L"import";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_IMPORT );

	i = L"using";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_USING );

	i = L"message";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_MESSAGE );

	i = L"exec";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_EXEC );

	i = L"tmp";
	d = DirectiveInfo::getDirective( i );
	assert( d == DIR_UNDEFINED );

}

/**
** @brief Test the DirectiveInfo::findDirectiveInfo methode.
*/
void DirectiveTest::findDirectiveInfoTest()
{
	const DirectiveInfo* pdi;  
	const wchar_t* i;

	i = L"ifdef";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_IFDEF );
	assert( pdi->getToken() == TOK_DIR_IFDEF );
	assert( pdi->getIdentifier() == i );

	i = L"define";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_DEFINE );
	assert( pdi->getToken() == TOK_DIR_DEFINE );
	assert( pdi->getIdentifier() == i );

	i = L"undef";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_UNDEF );
	assert( pdi->getToken() == TOK_DIR_UNDEF );
	assert( pdi->getIdentifier() == i );

	i = L"if";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_IF );
	assert( pdi->getToken() == TOK_DIR_IF );
	assert( pdi->getIdentifier() == i );

	i = L"ifdef";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_IFDEF );
	assert( pdi->getToken() == TOK_DIR_IFDEF );
	assert( pdi->getIdentifier() == i );

	i = L"ifndef";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_IFNDEF );
	assert( pdi->getToken() == TOK_DIR_IFNDEF );
	assert( pdi->getIdentifier() == i );

	i = L"elif";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_ELIF );
	assert( pdi->getToken() == TOK_DIR_ELIF );
	assert( pdi->getIdentifier() == i );

	i = L"else";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_ELSE );
	assert( pdi->getToken() == TOK_DIR_ELSE );
	assert( pdi->getIdentifier() == i );

	i = L"endif";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_ENDIF );
	assert( pdi->getToken() == TOK_DIR_ENDIF );
	assert( pdi->getIdentifier() == i );

	i = L"include";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_INCLUDE );
	assert( pdi->getToken() == TOK_DIR_INCLUDE );
	assert( pdi->getIdentifier() == i );

	i = L"line";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_LINE );
	assert( pdi->getToken() == TOK_DIR_LINE );
	assert( pdi->getIdentifier() == i );

	i = L"error";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_ERROR );
	assert( pdi->getToken() == TOK_DIR_ERROR );
	assert( pdi->getIdentifier() == i );

	i = L"pragma";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_PRAGMA );
	assert( pdi->getToken() == TOK_DIR_PRAGMA );
	assert( pdi->getIdentifier() == i );

	i = L"import";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_IMPORT );
	assert( pdi->getToken() == TOK_DIR_IMPORT );
	assert( pdi->getIdentifier() == i );

	i = L"using";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_USING );
	assert( pdi->getToken() == TOK_DIR_USING );
	assert( pdi->getIdentifier() == i );

	i = L"message";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_MESSAGE );
	assert( pdi->getToken() == TOK_DIR_MESSAGE );
	assert( pdi->getIdentifier() == i );

	i = L"exec";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi->getDirective() == DIR_EXEC );
	assert( pdi->getToken() == TOK_DIR_EXEC );
	assert( pdi->getIdentifier() == i );

	i = L"tmp";
	pdi = DirectiveInfo::findDirectiveInfo( i );
	assert( pdi == NULL );

}

} // namespace test
} // namespace sqtpp

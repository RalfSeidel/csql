#include "stdafx.h"
#include "Directive.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class DirectiveTest : public TestBase
{
public:

	/**
	** @brief Test the DirectiveInfo::getDirective methode.
	*/
	[TestMethod]
	void getDirectiveTest()
	{
		Directive      d;
		const wchar_t* i;

		i = L"ifdef";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_IFDEF );

		i = L"define";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_DEFINE );

		i = L"undef";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_UNDEF );

		i = L"undefall";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_UNDEFALL );

		i = L"if";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_IF );

		i = L"ifdef";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_IFDEF );

		i = L"ifndef";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_IFNDEF );

		i = L"elif";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_ELIF );

		i = L"else";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_ELSE );

		i = L"endif";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_ENDIF );

		i = L"include";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_INCLUDE );

		i = L"line";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_LINE );

		i = L"error";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_ERROR );

		i = L"pragma";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_PRAGMA );

		i = L"import";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_IMPORT );

		i = L"using";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_USING );

		i = L"message";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_MESSAGE );

		i = L"exec";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_EXEC );

		i = L"tmp";
		d = DirectiveInfo::getDirective( i );
		Assert::IsTrue( d == DIR_UNDEFINED );

	}

	/**
	** @brief Test the DirectiveInfo::findDirectiveInfo methode.
	*/
	[TestMethod]
	void findDirectiveInfoTest()
	{
		const DirectiveInfo* pdi;  
		const wchar_t* i;

		i = L"ifdef";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_IFDEF );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_IFDEF );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"define";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_DEFINE );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_DEFINE );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"undef";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_UNDEF );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_UNDEF );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"if";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_IF );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_IF );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"ifdef";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_IFDEF );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_IFDEF );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"ifndef";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_IFNDEF );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_IFNDEF );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"elif";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_ELIF );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_ELIF );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"else";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_ELSE );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_ELSE );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"endif";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_ENDIF );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_ENDIF );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"include";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_INCLUDE );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_INCLUDE );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"line";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_LINE );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_LINE );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"error";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_ERROR );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_ERROR );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"pragma";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_PRAGMA );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_PRAGMA );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"import";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_IMPORT );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_IMPORT );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"using";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_USING );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_USING );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"message";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_MESSAGE );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_MESSAGE );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"exec";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi->getDirective() == DIR_EXEC );
		Assert::IsTrue( pdi->getToken() == TOK_DIR_EXEC );
		Assert::IsTrue( pdi->getIdentifier() == i );

		i = L"tmp";
		pdi = DirectiveInfo::findDirectiveInfo( i );
		Assert::IsTrue( pdi == NULL );

	}
} ; // class

} // namespace test
} // namespace sqtpp

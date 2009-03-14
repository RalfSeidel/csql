#include "stdafx.h"
#include "Directive.h"

namespace sqtpp {

const DirectiveInfo DirectiveInfo::Undefined( DIR_UNDEFINED, TOK_DIRECTIVE, L"" );
const DirectiveInfo DirectiveInfo::Define( DIR_DEFINE, TOK_DIR_DEFINE, L"define" );
const DirectiveInfo DirectiveInfo::Undef( DIR_UNDEF, TOK_DIR_UNDEF, L"undef" );
const DirectiveInfo DirectiveInfo::Undefall( DIR_UNDEFALL, TOK_DIR_UNDEFALL, L"undefall" );
const DirectiveInfo DirectiveInfo::If( DIR_IF, TOK_DIR_IF, L"if" );
const DirectiveInfo DirectiveInfo::Ifdef( DIR_IFDEF, TOK_DIR_IFDEF, L"ifdef" );
const DirectiveInfo DirectiveInfo::Ifndef( DIR_IFNDEF, TOK_DIR_IFNDEF, L"ifndef" );
const DirectiveInfo DirectiveInfo::Elif( DIR_ELIF, TOK_DIR_ELIF, L"elif" );
const DirectiveInfo DirectiveInfo::Else( DIR_ELSE, TOK_DIR_ELSE, L"else" );
const DirectiveInfo DirectiveInfo::Endif( DIR_ENDIF, TOK_DIR_ENDIF, L"endif" );
const DirectiveInfo DirectiveInfo::Include( DIR_INCLUDE, TOK_DIR_INCLUDE, L"include" );
const DirectiveInfo DirectiveInfo::Line( DIR_LINE, TOK_DIR_LINE, L"line" );
const DirectiveInfo DirectiveInfo::Error( DIR_ERROR, TOK_DIR_ERROR, L"error" );
const DirectiveInfo DirectiveInfo::Pragma( DIR_PRAGMA, TOK_DIR_PRAGMA, L"pragma" );
const DirectiveInfo DirectiveInfo::Import( DIR_IMPORT, TOK_DIR_IMPORT, L"import" );
const DirectiveInfo DirectiveInfo::Using( DIR_USING, TOK_DIR_USING, L"using" );
const DirectiveInfo DirectiveInfo::Message( DIR_MESSAGE, TOK_DIR_MESSAGE, L"message" );
const DirectiveInfo DirectiveInfo::Exec( DIR_EXEC, TOK_DIR_EXEC, L"exec" );

/**
** @brief Array of all defined pre processor directives.
*/
const DirectiveInfo* DirectiveInfo::m_directives[] = {
	&DirectiveInfo::Define,
	&DirectiveInfo::Undef,
	&DirectiveInfo::Undefall,
	&DirectiveInfo::If,
	&DirectiveInfo::Ifdef,
	&DirectiveInfo::Ifndef,
	&DirectiveInfo::Elif,
	&DirectiveInfo::Else,
	&DirectiveInfo::Endif,
	&DirectiveInfo::Include,
	&DirectiveInfo::Line,
	&DirectiveInfo::Error,
	&DirectiveInfo::Pragma,
	&DirectiveInfo::Import,
	&DirectiveInfo::Using,
	&DirectiveInfo::Message,
	&DirectiveInfo::Exec,
	NULL
};


/**
** Initializing constructor.
*/
DirectiveInfo::DirectiveInfo( Directive directive, Token token, const wchar_t* text )
//: m_identifier( _wcsdup( text ) )
: m_directive( directive )
, m_token( token )
, m_identifier( text )
{
}

DirectiveInfo::~DirectiveInfo() throw()
{
	//delete[] m_identifier;
}

/**
** @brief Get directive of given identifier.
*/
const DirectiveInfo* DirectiveInfo::findDirectiveInfo( const wstring& identifier )
{
	const DirectiveInfo* const* ppInfo = m_directives;

	while ( *ppInfo != NULL ) {
		const DirectiveInfo& info = **ppInfo;
		if ( info == identifier ) {
			return *ppInfo;
		}
		++ppInfo;
	}

	return NULL;
}

/**
** @brief Get directive of given identifier.
*/
const Directive DirectiveInfo::getDirective( const wstring& identifier )
{
	const DirectiveInfo* pInfo = findDirectiveInfo( identifier );
	if ( pInfo == NULL ) {
		return DIR_UNDEFINED;
	} else {
		return pInfo->getDirective();
	}
}



/**
** @brief Check if given text machtes with directive.
*/
bool DirectiveInfo::operator== ( const wchar_t* text   ) const
{
	return wcscmp( m_identifier.c_str(), text ) == 0;
}

/**
** @brief Check if given text machtes with directive.
*/
bool DirectiveInfo::operator== ( const wstring& text   ) const
{
	return wcscmp( m_identifier.c_str(), text.c_str() ) == 0;
}

/**
** @brief Check if given directiv machtes with this directive.
*/
bool DirectiveInfo::operator== ( const DirectiveInfo& that ) const
{
	return m_identifier == that.m_identifier;
}


} // namespace

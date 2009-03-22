/**
** @file
** @author Ralf Seidel
** @brief Deklaration of predefined / buildin macros.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_BUILDIN_H
#define SQTPP_BUILDIN_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Macro.h"

namespace sqtpp {

class Options;
class MacroSet;

/**
** @brief baseclass for all builin / predefined macros.
*/
class BuildinMacro : public Macro
{
private:
	typedef Macro base;
protected:
	BuildinMacro( const wchar_t* identifier, MacroExpander& macroExpander  );
public:
	static void addBuildinMacros( const Options& options, MacroSet& macros );
};

/**
** @brief __FILE__
*/
class BuildinFile : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinFileExpander;
	static BuildinFileExpander m_expander;
public:
	BuildinFile();
};

/**
** @brief __LINE__
*/
class BuildinLine : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinLineExpander;
	static BuildinLineExpander m_expander;
public:
	BuildinLine();
};

/**
** @brief __DATE__
*/
class BuildinDate : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinDateExpander;
	static BuildinDateExpander m_expander;
public:
	BuildinDate();
};

/**
** @brief __TIME__
*/
class BuildinTime : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinTimeExpander;
	static BuildinTimeExpander m_expander;
public:
	BuildinTime();
};

/**
** @brief __TIMESTAMP__
*/
class BuildinTimestamp : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinTimestampExpander;
	static BuildinTimestampExpander m_expander;
public:
	BuildinTimestamp();
};

/**
** @brief __USER__
*/
class BuildinUser : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinUserExpander;
	static BuildinUserExpander m_expander;
public:
	BuildinUser();
};

/**
** @brief __HOST__
*/
class BuildinHost : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinHostExpander;
	static BuildinHostExpander m_expander;
public:
	BuildinHost();
};

/**
** @brief __TEMP__
*/
class BuildinTemp : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinTempExpander;
	static BuildinTempExpander m_expander;
public:
	BuildinTemp();
};

/**
** @brief __COUNTER__
*/
class BuildinCounter : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinCounterExpander;
	static BuildinCounterExpander m_expander;
public:
	BuildinCounter();
};

/**
** @brief __EVAL( expr ) - Evaluate arithmetik integer expression.
**
** 
*/
class BuildinEval : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinEvalExpander;
	static BuildinEvalExpander m_expander;
public:
	BuildinEval();
};

/**
** @brief __QUOTE( expr ) - Stringify expression.
**
** Unlike the std c stringfy operator this macro can be used anywhere. 
** Furthermore it expands the expression (if it contains any macro)
** before stringification.
*/
class BuildinQuote : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinQuoteExpander;
	static BuildinQuoteExpander m_expander;
public:
	BuildinQuote();
};


/**
** @brief __UNDEF( prefix ) - Undefine all macros whose names start 
** with prefix.
**
** This macro doesn't expand to anything. Instead if used if will drop
** all previous macro definitions that begin with prefix.
** 
*/
class BuildinUndefAll : public BuildinMacro 
{
private:
	typedef BuildinMacro base;
	class  BuildinUndefAllExpander;
	static BuildinUndefAllExpander m_expander;
public:
	BuildinUndefAll();
};


} // namespace

#endif // SQTPP_BUILDIN_H

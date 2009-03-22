/**
** @file
** @author Ralf Seidel
** @brief Declaration of exceptions.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.\p
**
** The only reason for the existens of this file is my unability to get used
** to the c++ stdlib nameing conventions. If any of theese exceptions is 
** throw the main routin of sqtpp will catch the exception and throw
** an error::C1001 (Internal error)
*/
#ifndef SQTPP_EXCEPTIONS_H
#define SQTPP_EXCEPTIONS_H
#if _MSC_VER > 10
#pragma once
#endif
#pragma once

#include <stdexcept>

namespace sqtpp {

/**
** @brief General runtime error.
*/
class RuntimeError : public std::runtime_error
{
private:
	typedef std::runtime_error base;

public:
	RuntimeError();
	RuntimeError( const char* message );
	~RuntimeError() throw();
};

/**
** @brief Logical error.
*/
class LogicError : public std::logic_error
{
private:
	typedef std::logic_error base;

public:
	LogicError();
	LogicError( const char* message );
	~LogicError() throw() {};
};

/**
** @brief Unexpected switch case.
**
** This exception is always thrown if the program encounters a switch case
** it doesn't exptect. The most common place for this exception is the
** default branch of a switch checking some enumertions. E.g.:
** 
** @code
** switch ( enumvalue ) {
** case enum1:
**     [...]
**     break;
** case enum2:
**     [...]
**     break;
** default:
**     throw UnexpectedSwitchError( "Unknown enum value" );
** } // switch
** @endcode
*/
class UnexpectedSwitchError : public LogicError 
{
private:
	typedef LogicError base;
public:
	UnexpectedSwitchError();
	UnexpectedSwitchError( const char* pszMessage );
	~UnexpectedSwitchError() throw() {};
};

/**
** @brief Not supported or implement functionality.
*/
class NotSupportedError : public sqtpp::RuntimeError
{
private:
	typedef sqtpp::RuntimeError base;

public:
	NotSupportedError();
	~NotSupportedError() throw() {}
};

} // namespace

#endif // SQTPP_EXCEPTIONS_H

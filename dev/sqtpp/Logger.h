/**
** @file
** @author Ralf Seidel
** @brief Declaration of the trace / log hanlder #sqtpp::Logger.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_LOGGER_H
#define SQTPP_LOGGER_H
#if _MSC_VER > 10
#pragma once
#endif

namespace sqtpp {

/**
** @brief Message logger.
**
** @todo Define trace switch and implement conditional log/trace handling.
*/
class Logger
{
public:
	// Default c'tor.
	Logger();
	// Default d'tor.
	~Logger();


	// log a message.
};


} // namespace

#endif // SQTPP_LOGGER_H

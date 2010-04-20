/**
** @file
** @author Ralf Seidel
** @brief Declaration of the #sqtpp::Output class.
**
** © 2009 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_OUTPUT_H
#define SQTPP_OUTPUT_H
#if _MSC_VER > 10
#pragma once
#endif


namespace sqtpp {
	class Options;
}

namespace sqtpp {

/**
** @brief Encapsulate the stream for the sqtpp output.
*/
class Output
{
private:
	/// The output stream.
	std::wostream& m_outStream;
	/// The error out stream (wcerr by default).
	std::wostream* m_pErrStream;
	/// The log out stream (wclog by default).
	std::wostream* m_pLogStream;

protected:
	Output( std::wostream& outStream );
public:
	virtual ~Output();

	virtual void close() = NULL;

	std::wostream& getStream()
	{
		return m_outStream;
	}

	std::wostream& getErrStream()
	{
		return *m_pErrStream;;
	}

	std::wostream& getLogStream()
	{
		return *m_pLogStream;;
	}

private:
	/// Private copy constructor - not implemented.
	Output( const Output& );

	/// Private assignement operator - not implemented.
	Output& operator=( const Output& output );

public:
	static Output* createOutput( const Options& options );
	static Output* createOutput( std::wostream& stream );
};

} // namespace



#endif // SQTPP_OUTPUT_H

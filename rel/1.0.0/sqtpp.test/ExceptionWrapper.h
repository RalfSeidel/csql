/*!\file
** \brief Declaration of exception class which to translate native exception 
**        thrown by old fashioned libraries into managed code exception.
**
** © 2010 by SQL Service GmbH
*/
#pragma once

namespace sqtpp {
namespace test {
namespace Interop {

public ref class StlException : System::Exception
{
public:
	StlException( System::String^ message );

	static void Throw( const std::exception& stlException  );
};


} // namespace Interop
} // namespace test
} // namespace sqtpp

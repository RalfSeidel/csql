using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
	public enum ExitCode
	{
		Success,
		GeneralError,
		ArgumentsError,
		FileIOError,
		PreprocessorError,
		SqlConnectionError,
		SqlCommandError,
		UnexpectedError

	}
}

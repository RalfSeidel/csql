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
		/// <summary>
		/// Error occured while initialising the database provider (driver).
		/// </summary>
		SqlIntializeError,
		SqlConnectionError,
		SqlCommandError,
		UnexpectedError

	}
}

namespace csql
{
	/// <summary>
	/// Exit codes of the processor.
	/// </summary>
	public enum ExitCode
	{
		/// <summary>
		/// Processing completed without any error.
		/// </summary>
		Success,
		/// <summary>
		/// Unspecified error.
		/// </summary>
		GeneralError,
		/// <summary>
		/// Invalid command line arguments.
		/// </summary>
		ArgumentsError,
		/// <summary>
		/// Some file operation failed.
		/// </summary>
		FileIOError,
		/// <summary>
		/// The preprocessor returned an error.
		/// </summary>
		PreprocessorError,
		/// <summary>
		/// Error occured while initialising the database provider (driver).
		/// </summary>
		SqlInitializeError,
		/// <summary>
		/// Connection to database failed.
		/// </summary>
		SqlConnectionError,
		/// <summary>
		/// Error in SQL command or batch occurred.
		/// </summary>
		SqlCommandError,
		/// <summary>
		/// Some other exception occurred.
		/// </summary>
		UnexpectedError

	}
}

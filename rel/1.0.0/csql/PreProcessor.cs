using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace csql
{
	internal class PreProcessor
	{
		private readonly CSqlOptions m_options;
		private string m_pipeName;

		/// <summary>
		/// Construtor with processor options.
		/// </summary>
		public PreProcessor( CSqlOptions options )
		{
			this.m_options = options;
		}

		/// <summary>
		/// Gets the path of the preprocessor sqtpp.
		/// </summary>
		/// <remarks>
		/// Currently the preprocessor is expected to be found in the same directory as 
		/// csql itself. 
		/// </remarks>
		/// <value>
		/// The preprocessor path which is the directory of the csql executable combined 
		/// with the file name of the preprocessor executable.
		/// </value>
		public static string Command
		{
			get
			{
				Assembly assembly = Assembly.GetExecutingAssembly();

				string thisPath = assembly.Location;
				string root = System.IO.Path.GetPathRoot( thisPath );
				string folder = System.IO.Path.GetDirectoryName( thisPath );
				string sqtppPath = System.IO.Path.Combine( System.IO.Path.Combine( root, folder ), "sqtpp.exe" );

				return sqtppPath;
			}
		}

		/// <summary>
		/// Gets the arguments for the preprocessor sqtpp.
		/// </summary>
		/// <value>The preprocessor arguemtns.</value>
		public string Arguments
		{
			get
			{
				SqtppOptions ppOption = m_options.PreprocessorOptions;
				ppOption.InputFile = m_options.ScriptFile;
				ppOption.OutputFile = m_options.UseNamedPipes ? NamedPipe.GetPipePath( NamedPipeName ) : m_options.TempFile;
				string ppArguments = ppOption.CommandLineArguments;

				return ppArguments;
			}
		}

		/// <summary>
		/// Gets the name of the named pipe.
		/// </summary>
		/// <value>The name of the named pipe.</value>
		public string NamedPipeName
		{
			get
			{
				Debug.Assert( m_options.UseNamedPipes );
				if ( m_pipeName == null ) {
					int currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
					m_pipeName = "de.sqlservice.sqtpp\\" + currentProcessId.ToString( CultureInfo.InvariantCulture );
				}
				return m_pipeName;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// Options for the preprocessor call.
	/// </summary>
	public class SqtppOptions
	{
		#region Data Member

		/// <summary>
		/// Macro defintions passed to sqtpp with the /D argument
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly Dictionary<string, string> macroDefinitions = new Dictionary<string, string>();

		/// <summary>
		/// List of additional include directories
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private readonly List<string> includeDirectories = new List<string>();

		#endregion

		/// <summary>
		/// Option to emit #line directives or not (default is <c>true</c>)
		/// </summary>
		public bool EmitLineDirectives
		{
			get;
			set;
		}

		/// <summary>
		/// Macro defintions passed to sqtpp with the /D argument
		/// </summary>
		public IDictionary<string, string> MacroDefinitions
		{
			get { return macroDefinitions; }
		}

		/// <summary>
		/// List of include directories
		/// </summary>
		public ICollection<string> IncludeDirectories
		{
			get { return this.includeDirectories; }
		}

		/// <summary>
		/// Path of the main input file.
		/// </summary>
		public string InputFile { get; set; }

		/// <summary>
		/// Path of the main output file.
		/// </summary>
		public string OutputFile { get; set; }

		/// <summary>
		/// Range to execute i.e. in terms for the preprocessor
		/// the range in the input file to emit output form.
		/// </summary>
		/// <remarks>
		/// The range is measured in characters from the beginning 
		/// of the input file.
		/// </remarks>
		private Range range;

		/// <summary>
		/// Every other argument/option e.g. /Co1200
		/// </summary>
		public string AdvancedArguments { get; set; }

		/// <summary>
		/// Builds the command line arguments for the preprocessor.
		/// </summary>
		/// <value>The command line arguments.</value>
		public string CommandLineArguments
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				string separator = " ";

				if ( this.EmitLineDirectives ) {
					sb.Append( separator );
					sb.Append( "/E" );
					separator = " ";
				}

				foreach ( var directory in this.includeDirectories ) {
					if ( !String.IsNullOrEmpty( directory ) ) {
						sb.Append( separator );
						sb.Append( "/I" );
						if ( directory.IndexOf( ' ' ) >= 0 ) {
							sb.Append( '"' );
							sb.Append( directory );
							sb.Append( '"' );
						} else {
							sb.Append( directory );
						}
						separator = " ";
					}
				}

				foreach ( var pd in this.macroDefinitions ) {
					if ( !String.IsNullOrEmpty( pd.Key ) ) {
						sb.Append( separator );
						sb.Append( "/D" ).Append( pd.Key );
						if ( !String.IsNullOrEmpty( pd.Value ) ) {
							sb.Append( "=" ).Append( pd.Value );
						}
						separator = " ";
					}
				}

				if ( !range.IsEmpty ) {
					sb.Append( separator );
					sb.Append( "/r" );
					sb.Append( range.FirstCharacter );
					sb.Append( "-" );
					sb.Append( range.LastCharacter );
					separator = " ";
				}

				if ( !String.IsNullOrEmpty( AdvancedArguments ) ) {
					sb.Append( separator );
					sb.Append( AdvancedArguments );
					separator = " ";
				}

				if ( !String.IsNullOrEmpty( OutputFile ) ) {
					sb.Append( separator );
					sb.Append( "/o" );
					if ( OutputFile.IndexOf( ' ' ) >= 0 ) {
						sb.Append( '"' );
						sb.Append( OutputFile );
						sb.Append( '"' );
					} else {
						sb.Append( OutputFile );
					}
					separator = " ";
				}

				sb.Append( separator );
				if ( InputFile.IndexOf( ' ' ) >= 0 ) {
					sb.Append( '"' );
					sb.Append( InputFile );
					sb.Append( '"' );
				} else {
					sb.Append( InputFile );
				}

				return sb.ToString();
			}
		}

		public void SetRange( int fromChar, int toChar )
		{
			this.range.FirstCharacter = fromChar;
			this.range.LastCharacter = toChar;
		}


		public SqtppOptions()
		{
			this.EmitLineDirectives = true;
		}


		private struct Range
		{
			public int FirstCharacter;
			public int LastCharacter;

			public bool IsEmpty
			{
				get
				{
					return FirstCharacter < 0 || LastCharacter <= FirstCharacter;
				}
			}
		}
	}
}

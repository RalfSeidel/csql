using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace csql.addin.Commands
{
	internal class VariableSubstitutor
	{
		private readonly IDocumentEnvironment environment;
		private static readonly Regex variableExpression = new Regex( @"\$\([a-z]+\)", RegexOptions.Compiled | RegexOptions.IgnoreCase );

		public VariableSubstitutor( IDocumentEnvironment environment )
		{
			this.environment = environment;
		}

		public string Substitute( string value )
		{
			if ( string.IsNullOrEmpty( value ) )
				return value;

			var matches = variableExpression.Matches( value );
			if ( matches.Count == 0 ) 
				return value;

			StringBuilder substitutedValue = new StringBuilder();
			int lastIndex = 0;
			foreach ( Match match in matches ) {
				if ( match.Index > lastIndex ) {
					substitutedValue.Append( value.Substring( lastIndex, match.Index - lastIndex ) );
				}

				string variable = match.Value;
				string variableValue = GetVariableValue( variable );
				substitutedValue.Append( variableValue );

				lastIndex = match.Index + match.Length;
			}
			if ( lastIndex < value.Length ) {
				substitutedValue.Append( value.Substring( lastIndex ) );
			}

			return substitutedValue.ToString();
		}


		private string GetVariableValue( string variable )
		{
			string name = variable.Substring( 2, variable.Length - 3 );

			EnvironmentVariable variableId;
			try {
				variableId = (EnvironmentVariable)Enum.Parse( typeof( EnvironmentVariable ), name, true );
			}
			catch ( System.ArgumentException )
			{
				Trace.WriteLine( "Unsupported variable: " + name );
				return variable;
			}

			switch ( variableId ) {
				case EnvironmentVariable.SolutionDirectory:
					return environment.SolutionDirectory;
				case EnvironmentVariable.ProjectDirectory:
					return environment.ProjectDirectory;
				case EnvironmentVariable.ItemDirectory:
					return environment.ItemDirectory;
				case EnvironmentVariable.TargetDirectory:
					return environment.TargetDirectory;
				default:
					throw new NotSupportedException( "Unsupported variable: " + name );
			}
		}
	}
}

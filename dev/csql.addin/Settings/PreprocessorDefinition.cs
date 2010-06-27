using System;
using System.ComponentModel;

namespace csql.addin.Settings
{
	/// <summary>
	/// Macro definition for the pre processor (sqtpp)
	/// </summary>
	[DefaultProperty( "Name" )]
	[Serializable]
	public class PreprocessorDefinition
	{
		[Description( "The macro name" )]
		public string Name { get; set; }

		[Description( "The macro value" )]
		[DefaultValue( "" )]
		public string Value { get; set; }

		[DefaultValue( true )]
		[DisplayName( "Enabled" )]
		[Description( "Option to enable/disable a preprocessor macro." )]
		public bool IsEnabled { get; set; }


		/// <summary>
		/// Default constructor.
		/// </summary>
		public PreprocessorDefinition()
		{
			IsEnabled = true;
			Name = "Name";
			Value = string.Empty;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		public PreprocessorDefinition( PreprocessorDefinition preprocessorDefinition )
		{
			this.IsEnabled = preprocessorDefinition.IsEnabled;
			this.Name = preprocessorDefinition.Name;
			this.Value = preprocessorDefinition.Value;
		}
	}
}

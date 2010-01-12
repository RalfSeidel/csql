
namespace csql
{
	/// <summary>
	/// Some global settings.
	/// </summary>
	public static class GlobalSettings
	{
		/// <summary>
		/// The trace level of the program.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="Just don't want user code to change the reference but the level (content) only" )]
		public static readonly VerbositySwitch Verbosity = new VerbositySwitch();
	}
}

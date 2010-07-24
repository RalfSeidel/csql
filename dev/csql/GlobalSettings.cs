using System.Diagnostics.CodeAnalysis;

namespace csql
{
	/// <summary>
	/// Some global settings.
	/// </summary>
	public static class GlobalSettings
	{
		/// <summary>
		/// The name of this product to be used for gui titles and output headers.
		/// </summary>
		[SuppressMessage( "Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CSql", Justification = "CSql is the best readable spelling for the product." )]
		public const string CSqlProductName = "csql";

		/// <summary>
		/// The trace level of the program.
		/// </summary>
		[SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Just don't want user code to change the reference but the level (content) only" )]
		public static readonly VerbositySwitch Verbosity = new VerbositySwitch();
	}
}

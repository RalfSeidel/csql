using System;
using System.Diagnostics;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;

namespace Sqt.VisualStudio.Util
{
	public static class VsApplicationExtensions
	{
		/// <summary>
		/// Gets a variable from the visual studio solution or if no solution
		/// is available from the global visual studio store.
		/// </summary>
		/// <remarks>
		/// <see cref="http://msdn.microsoft.com/en-us/library/96t389k3.aspx">Persisting Information in Projects and Solutions</see>
		/// for further informations about the Visual Studio Persistens API. Note that
		/// although the API uses raw objects as variable values it is not possible
		/// to save anything else than strings.
		/// </remarks>
		/// <param name="application">The visual studio application object.</param>
		/// <param name="variableName">Name of the variable.</param>
		/// <returns>The variable value or <c>null</c> if the variable is not defined.</returns>
		public static bool TryGetVariableFromSolutionOrGlobals( this _DTE application, string variableName, out object result )
		{
			if ( application.Solution != null ) {
				Globals solutionGlobals = application.Solution.Globals;
				if ( solutionGlobals.get_VariableExists( variableName ) ) {
					result = solutionGlobals[variableName];
					return true;
				}
			}
			Globals globals = application.Globals;
			if ( globals.get_VariableExists( variableName ) ) {
				result = globals[variableName];
				return true;
			}
			Debug.WriteLine( "VsApplicationExtensions.GetVariableFromSolutionOrGlobals - Variable not found: " + variableName );
			result = null;
			return false;
		}


		/// <summary>
		/// Gets the menu bar with the specified english name.
		/// </summary>
		/// <seealso cref="http://www.mztools.com/articles/2007/mz2007002.aspx">HOWTO: Locate commandbars in international versions of Visual Studio</seealso>
		/// <param name="application">The visual studio application object.</param>
		/// <param name="menuBarName">Then english name of the menu bar, e.g. Tools, Data, Window...</param>
		/// <returns>
		/// The menu bar with the specified english name or <c>null</c>
		/// if the menu bar was not found.
		/// </returns>
		public static bool TryGetMenuBar( this _DTE application, string menuBarName, out CommandBar menuBar )
		{
			CommandBars commandBars = (CommandBars)application.CommandBars;
			CommandBar mainMenuBar = (CommandBar)commandBars["MenuBar"];
			foreach ( CommandBarControl ctrl in mainMenuBar.Controls ) {
				MsoControlType type = ctrl.Type;
				if ( type != MsoControlType.msoControlPopup )
					continue;

				CommandBarPopup ctrlPopup = (CommandBarPopup)ctrl;
				menuBar = ctrlPopup.CommandBar;
				string subMenuName = menuBar.Name;
				if ( subMenuName.Equals( menuBarName, StringComparison.InvariantCultureIgnoreCase ) ) {
					return true;
				}
			}
			Debug.WriteLine( "VsApplicationExtensions.GetMenuBar - Menu bar not found: " + menuBarName );
			menuBar = null;
			return false;
		}
	}
}

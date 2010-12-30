using System;
using EnvDTE;
using EnvDTE80;

namespace csql.addin.Gui
{
	/// <summary>
	/// Accessor for the output window pane.
	/// </summary>
	internal static class Output
	{
		/// <summary>
		/// Get the output window pane for traceing and emitting messages.
		/// </summary>
		public static OutputWindowPane GetOutputPane( DTE2 application )
		{
			string outputPaneName = GlobalSettings.CSqlProductName;

			ToolWindows toolWindows = application.ToolWindows;
			OutputWindow outputWindow = toolWindows.OutputWindow;

			if ( outputWindow == null )
				return null;

			foreach ( OutputWindowPane pane in outputWindow.OutputWindowPanes ) {
				if ( String.Equals( outputPaneName, pane.Name ) )
					return pane;
			}

			OutputWindowPane outputWindowPane = outputWindow.OutputWindowPanes.Add( GlobalSettings.CSqlProductName );
			return outputWindowPane;
		}

		/// <summary>
		/// Get the output window pane for traceing and emitting messages.
		/// </summary>
		public static OutputWindowPane GetAndActivateOutputPane( DTE2 application )
		{
			OutputWindowPane outputWindowPane = GetOutputPane( application );
			if ( outputWindowPane != null ) {
				outputWindowPane.Activate();
				ToolWindows toolWindows = application.ToolWindows;
				OutputWindow outputWindow = toolWindows.OutputWindow;
				if ( outputWindow.Parent != null ) {
					outputWindow.Parent.Visible = true;
				}
			}

			return outputWindowPane;
		}
	}
}

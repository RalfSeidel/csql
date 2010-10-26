﻿using System;
using System.Diagnostics;
using csql.addin.Settings;
using EnvDTE;
using EnvDTE80;
using Sqt.DbcProvider;
using Sqt.VisualStudio;

namespace csql.addin.Commands
{
	/// <summary>
	/// Command to execute the (c)sql script in the current editor window.
	/// </summary>
	internal class ExecuteScriptCommand : VsCommand
	{
		private static ExecuteScriptCommand instance;
		private ScriptExecutor currentExecutor;

		internal ExecuteScriptCommand()
			: base( "Execute" )
		{
			ExecuteScriptCommand.instance = this;
			Guid textEditorGuid = new Guid( ContextGuids.vsContextGuidTextEditor );
			this.VsContextGuids = new Guid[] { textEditorGuid };
		}

		/// <summary>
		/// Gets the current instance of this command.
		/// </summary>
		internal static ExecuteScriptCommand Instance
		{
			get { return ExecuteScriptCommand.instance; }
		}

		/// <summary>
		/// Gets a value indicating whether a script is currently executed.
		/// </summary>
		internal bool IsExecuting
		{
			get { return this.currentExecutor != null; }
		}

		internal void CancelExecution()
		{
			ScriptExecutor executor = currentExecutor;
			if ( executor == null )
				return;

			executor.Cancel();
		}


		public override bool CanExecute( VsCommandEventArgs e )
		{
			if ( IsExecuting )
				return false;

			DTE2 application = e.Application;
			Document activeDocument = application.ActiveDocument;
			if ( activeDocument == null ) {
				return false;
			}

			SettingsManager settingsManager = SettingsManager.GetInstance( application );
			CSqlParameter scriptParameter = settingsManager.CurrentScriptParameter;

			string fileName = activeDocument.FullName;
			bool isSqlScript = FileClassification.IsSqlScript( scriptParameter, fileName );
			return isSqlScript;
		}

		public override void Execute( VsCommandEventArgs e )
		{
			DTE2 application = e.Application;

			var activeDocument = application.ActiveDocument;
			var documentPath   = activeDocument.FullName;
			if ( !activeDocument.Saved && !activeDocument.ReadOnly ) {
				activeDocument.Save( documentPath );
			}


			SettingsManager settingsManager = SettingsManager.GetInstance( application );
			DbConnectionParameter dbConnectionParameter = settingsManager.CurrentDbConnectionParameter;
			var csqGuiParameter = settingsManager.CurrentScriptParameter;
			var csqlOptions = CreateCSqlOptions( dbConnectionParameter, csqGuiParameter, documentPath );

			TextSelection selection = activeDocument.Selection as TextSelection;
			int fromChar = -1;
			int toChar = -1;

			if ( selection != null ) {
				var fromPoint = selection.TopPoint;
				var toPoint = selection.BottomPoint;

				if ( fromPoint != null && toPoint != null ) {
					fromChar = fromPoint.AbsoluteCharOffset - 1;
					toChar = toPoint.AbsoluteCharOffset - 1;
				}
			}
			if ( fromChar >= 0 && toChar > fromChar ) {
				csqlOptions.PreprocessorOptions.SetRange( fromChar, toChar );
			}

			var thread = new System.Threading.Thread( Thread_Start );
			var parameter = new ThreadParameter();
			parameter.Application = application;
			parameter.CSqlOptions = csqlOptions;

			thread.IsBackground = true;
			thread.Start( parameter );

			if ( !csqGuiParameter.IsOutputFileEnabled ) {
				settingsManager.SaveDbConnectionParameterInMruHistory( dbConnectionParameter );
			}

		}

		private static SqtppOptions CreatePreprocessorArguments( CSqlParameter csqlParameter )
		{
			SqtppOptions result = new SqtppOptions();

			foreach ( var pd in csqlParameter.PreprocessorDefinitions ) {
				if ( pd.IsEnabled && !String.IsNullOrEmpty( pd.Name ) ) {
					result.MacroDefinitions.Add( pd.Name, pd.Value );
				}
			}

			foreach ( var directory in csqlParameter.IncludeDirectories ) {
				if ( !String.IsNullOrEmpty( directory ) ) {
					result.IncludeDirectories.Add( directory );
				}
			}

			if ( !String.IsNullOrEmpty( csqlParameter.AdvancedPreprocessorParameter ) ) {
				result.AdvancedArguments = csqlParameter.AdvancedPreprocessorParameter;
			}

			return result;
		}

		private static CSqlOptions CreateCSqlOptions( DbConnectionParameter dbConnectionParameter, CSqlParameter csqlParameter, string scriptfile )
		{
			CSqlOptions csqlOptions = new CSqlOptions();

			csqlOptions.ScriptFile = scriptfile;
			if ( csqlParameter.IsOutputFileEnabled )
				csqlOptions.DistributionFile = csqlParameter.OutputFile;
			else
				csqlOptions.DistributionFile = null;

			if ( csqlParameter.IsTemporaryFileEnabled )
				csqlOptions.TempFile = csqlParameter.TemporaryFile;
			else
				csqlOptions.TempFile = null;

			csqlOptions.BreakOnError = csqlParameter.IsBreakOnErrorEnabled;
			csqlOptions.UsePreprocessor = csqlParameter.IsPreprocessorEnabled;
			csqlOptions.ConnectionParameter = dbConnectionParameter;
			csqlOptions.PreprocessorOptions = CreatePreprocessorArguments( csqlParameter );
			csqlOptions.AddPreprocessorMacros();

			csqlOptions.Verbosity.Level = csqlParameter.Verbosity;

			csqlOptions.NoLogo = false;
			csqlOptions.MaxResultColumnWidth = csqlParameter.MaxResultColumnWidth;

			return csqlOptions;
		}

		private void Thread_Start( object obj )
		{
			ThreadParameter parameter = (ThreadParameter)obj;
			DTE2 application = parameter.Application;
			CSqlOptions settings = parameter.CSqlOptions;
			OutputPaneTraceListener traceListener = null;

			try {
				var outputPane = Gui.Output.GetAndActivateOutputPane( application );
				if ( outputPane != null ) {
					outputPane.Clear();
					traceListener = new OutputPaneTraceListener( outputPane );
				}

				if ( traceListener != null ) {
					Trace.Listeners.Add( traceListener );
				}
				this.currentExecutor = new ScriptExecutor( settings );
				this.currentExecutor.Execute();

				if ( !String.IsNullOrEmpty( settings.DistributionFile ) ) {
					application.ItemOperations.OpenFile( settings.DistributionFile, Constants.vsViewKindCode );
				}
				currentExecutor = null;
			}
			finally {
				currentExecutor = null;
				if ( traceListener != null ) {
					Trace.Listeners.Remove( traceListener );
					traceListener.Close();
					traceListener.Dispose();
				}
			}
			Commands2 commands = application.Commands as Commands2;
			if ( commands != null ) {
				commands.UpdateCommandUI( false );
			}
		}

		private class ThreadParameter
		{
			public DTE2 Application { get; set; }
			public CSqlOptions CSqlOptions { get; set; }
		}
	}
}

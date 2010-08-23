using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace csql
{
	/// <summary>
	/// TODO: Add Description
	/// </summary>
	internal class ProcessorContext
	{
		private string m_currentFile;
		private int m_currentFileLineNo;
		private int m_currentBatchNo;
		private int m_currentBatchLineNo;
		private IList<BatchContext> m_currentBatchContexts;
		private StringBuilder m_batchBuilder;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessorContext"/> class.
		/// </summary>
		/// <param name="mainScript">The main script processed.</param>
		public ProcessorContext( string mainScript )
		{
			m_currentFile = mainScript;
			m_currentFileLineNo = 1;
			m_currentBatchNo = 1;
			m_currentBatchLineNo = 1;
			m_batchBuilder = new StringBuilder( 4096 );
			m_currentBatchContexts = new List<BatchContext>();
			ResetBatchContext();
		}

		/// <summary>
		/// Gets the path of the current file processed.
		/// </summary>
		/// <value>The current file path.</value>
		public string CurrentFile
		{
			get { return this.m_currentFile; }
		}

		/// <summary>
		/// Gets the line offset of the current input file.
		/// </summary>
		/// <value>The file line no.</value>
		public int CurrentFileLineNumber
		{
			get { return this.m_currentFileLineNo; }
		}

		/// <summary>
		/// Gets the current batch counter.
		/// </summary>
		/// <value>The current batch no.</value>
		public int CurrentBatchNo
		{
			get { return this.m_currentBatchNo; }
		}

		/// <summary>
		/// Gets the line offset of the current batch to the start of the input file.
		/// </summary>
		/// <value>The current batch line no.</value>
		public int CurrentBatchLineNo
		{
			get { return this.m_currentBatchLineNo; }
		}

		/// <summary>
		/// Gets the text of the batch to execute next.
		/// </summary>
		/// <value>The current command batch text.</value>
		public string CurrentBatch
		{
			get { return this.m_batchBuilder.ToString(); }
		}

		public BatchContext FirstBatchContext
		{
			get { return this.m_currentBatchContexts[0]; }
		}

		public IList<BatchContext> BatchContexts
		{
			get { return m_currentBatchContexts; }
		}

		public void AppendLine( string line )
		{
			m_batchBuilder.AppendLine( line );
			IncrementLineNumber();
			IncrementBatchLineNumber();
		}

		public void SetNewBatchContext( string file, int lineNumber )
		{
			m_currentFile = file;
			m_currentFileLineNo = lineNumber;

			if ( m_batchBuilder.Length == 0 ) {
				BatchContext startContext = FirstBatchContext;
				startContext.File = m_currentFile;
				startContext.LineNumber = m_currentFileLineNo;
			}
			else {
				BatchContext newContext = new BatchContext( m_currentBatchLineNo, m_currentFile, m_currentFileLineNo );
				m_currentBatchContexts.Add( newContext );
			}
		}

		public void StartNextBatch()
		{
			m_batchBuilder.Length = 0;
			m_currentBatchLineNo = 1;
			ResetBatchContext();
		}

		public void IncrementLineNumber()
		{
			++this.m_currentFileLineNo;
		}

		public void IncrementBatchNumber()
		{
			++this.m_currentBatchNo;
		}

		public void IncrementBatchLineNumber()
		{
			++this.m_currentBatchLineNo;
		}


		/// <summary>
		/// Format a error or warning message that occurred in the current context.
		/// </summary>
		/// <param name="severity">
		/// The severity of the (error) message.
		/// </param>
		/// <param name="message">
		/// The error message.
		/// </param>
		/// <param name="errorLineNumber">
		/// The line number where error was reported.
		/// </param>
		/// <returns>The formated message.</returns>
		public string FormatError( TraceLevel severity, string message, int errorLineNumber )
		{
			IList<BatchContext> batchContexts = BatchContexts;
			int contextCount = batchContexts.Count;
			BatchContext context = null;
			for ( int i = contextCount - 1; i >= 0; --i ) {
				context = batchContexts[i];
				if ( context.BatchOffset <= errorLineNumber )
					break;
			}
			Debug.Assert( context != null );

			if ( context == null ) {
				string error = String.Format( "{0}({1}): {2}: {3}", CurrentFile, errorLineNumber, severity, message );
				return error;
			}
			else {
				string contextFile = context.File;
				int contextLineNumber = context.LineNumber + errorLineNumber - context.BatchOffset;
				string error = String.Format( "{0}({1}): {2}: {3}", contextFile, contextLineNumber, severity, message );
				return error;
			}
		}


		/// <summary>
		/// Clear the batch context.
		/// </summary>
		private void ResetBatchContext()
		{
			m_currentBatchContexts.Clear();
			if ( !String.IsNullOrEmpty( m_currentFile ) ) {
				BatchContext initialContext = new BatchContext( m_currentBatchLineNo, m_currentFile, m_currentFileLineNo );
				m_currentBatchContexts.Add( initialContext );
			}
		}

	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace csql.ResultTrace
{
	/// <summary>
	/// Helper class to trace the rows of a data reader.
	/// </summary>
	internal class DataReaderTracer
	{
		/// <summary>
		/// The data reader for whos data is to be traced.
		/// </summary>
		private readonly IDataReader dataReader;

		/// <summary>
		/// Meta data for each column.
		/// </summary>
		private readonly IList<ColumnDescriptor> columnDescriptors;

		/// <summary>
		/// Some options specified by the user.
		/// </summary>
		private readonly DataReaderTraceOptions options;

		public DataReaderTracer( IDataReader dataReader, DataReaderTraceOptions options )
		{
			Debug.Assert( dataReader != null && !dataReader.IsClosed );
			Debug.Assert( options != null );

			this.dataReader = dataReader;
			this.options = options;
			List<ColumnDescriptor> columnInfos = new List<ColumnDescriptor>();
			for ( int i = 0; i < dataReader.FieldCount; ++i ) {
				string name = dataReader.GetName( i );
				Type type = dataReader.GetFieldType( i );
				ColumnDescriptor columnInfo = new ColumnDescriptor( i, name, type, options.MaxResultColumnWidth );
				columnInfos.Add( columnInfo );
			}

			this.columnDescriptors = columnInfos.ToArray();
		}

		private int ColumnCount
		{
			get { return this.columnDescriptors.Count; }
		}


		/// <summary>
		/// Traces all to the default trace output.
		/// </summary>
		public void TraceAll()
		{
			TextWriter output = new TraceWriter();
			TraceAll( output );
		}

		private void TraceAll( TextWriter output )
		{
			List<Row> prefetchRows = new List<Row>();

			int prefetchCount = 10;
			bool prefetchedAll = false;
			for ( int i = 0; i < prefetchCount; ++i ) {
				if ( !this.dataReader.Read() ) {
					prefetchedAll = true;
					break;
				}

				Row row = new Row( this.dataReader );
				prefetchRows.Add( row );
			}
			AutoFormatAllColumns( prefetchRows, prefetchedAll );

			if ( !AreAllColumnNamesEmpty( this.columnDescriptors ) ) {
				TraceHeader( output );
			}

			foreach ( Row row in prefetchRows ) {
				TraceRow( output, row );
			}


			if ( !prefetchedAll ) {
				while ( this.dataReader.Read() ) {
					Row row = new Row( dataReader );
					TraceRow( output, row );
				}
			}
		}

		private void TraceHeader( TextWriter output )
		{
			StringBuilder rowBuffer = new StringBuilder();
			for ( int i = 0; i < ColumnCount; ++i ) {
				var columnInfo = columnDescriptors[i];
				var columnFormat = columnInfo.Format;
				var columnName = columnInfo.Name;
				rowBuffer.Append( columnName );
				if ( i == ColumnCount - 1 ) {
					rowBuffer.Append( ' ' );
				} else {
					int fillCount = columnFormat.MaxWidth - columnName.Length + 1;
					if ( fillCount > 0 ) {
						rowBuffer.Append( new string( ' ', fillCount ) );
					}
				} 
			}
			output.WriteLine( rowBuffer.ToString() );
			rowBuffer.Length = 0;

			for ( int i = 0; i < ColumnCount; ++i ) {
				var columnInfo = columnDescriptors[i];
				var columnFormat = columnInfo.Format;
				var columnWidth  = columnFormat.MaxWidth;
				rowBuffer.Append( new string( '-', columnWidth ) );
				rowBuffer.Append( ' ' );
			}
			output.WriteLine( rowBuffer.ToString() );
		}

		public void TraceRow( TextWriter output, Row row )
		{
			int columnCount = row.Count;
			int maxLineCount = 0;
			int totalWidth = 0;

			// Split each column into several lines.
			List<string>[] allColumnLines = new List<string>[columnDescriptors.Count];
			for ( int i = 0; i < columnCount; ++i ) {
				ColumnDescriptor columnDescriptor = this.columnDescriptors[i];
				ColumnFormat columnFormat = columnDescriptor.Format;
				object columnValue = row[i];
				string columnText = columnFormat.Format( columnValue );
				List<string> columnLines = BreakColumnText( columnDescriptor, columnText );
				int columnLineCount = columnLines.Count;
				if ( columnLineCount > maxLineCount ) {
					maxLineCount = columnLineCount;
				}
				allColumnLines[i] = columnLines;
				totalWidth += columnFormat.MaxWidth + 1;
			}

			for ( int lineIndex = 0; lineIndex < maxLineCount; ++lineIndex ) {
				StringBuilder rowBuffer = new StringBuilder( totalWidth );
				for ( int i = 0; i < ColumnCount; ++i ) {
					var columnInfo = columnDescriptors[i];
					var columnFormat = columnInfo.Format;
					var columnLines = allColumnLines[i];
					bool isLastColumn = IsLastNonEmptyColumn( allColumnLines, lineIndex, i );
					if ( lineIndex < columnLines.Count ) {
						string lineText = columnLines[lineIndex];
						rowBuffer.Append( lineText );
						// Fill with blanks upon the beginning of the next column
						if ( isLastColumn ) {
							rowBuffer.Append( ' ' );
						} else {
							rowBuffer.Append( new string( ' ', columnFormat.MaxWidth - lineText.Length + 1 ) );
						}
					} else {
						// Fill with blanks upon the beginning of the next column
						if ( isLastColumn ) {
							rowBuffer.Append( ' ' );
						} else {
							rowBuffer.Append( new string( ' ', columnFormat.MaxWidth + 1 ) );
						}
					}
				}
				output.WriteLine( rowBuffer.ToString() );
			}
		}


		private bool IsLastNonEmptyColumn( IList<IList<string>> allColumnLines, int lineIndex, int columnIndex )
		{
			for ( int i = columnIndex + 1; i < allColumnLines.Count; ++i ) {
				IList<string> columnLines = allColumnLines[i];
				if ( lineIndex < columnLines.Count ) {
					string line = columnLines[lineIndex];
					if ( !String.IsNullOrEmpty( line ) )
						return false;
				}
			}
			return true;
		}


		private static List<string> BreakColumnText( ColumnDescriptor columnInfo, string columnText )
		{
			int tracedCharCount = 0;
			var result = new List<string>();
			var columnFormat = columnInfo.Format;
			while ( tracedCharCount < columnText.Length ) {
				int rowCharCount = Math.Min( columnText.Length - tracedCharCount, columnFormat.MaxWidth );
				var rowText = columnText.Substring( tracedCharCount, rowCharCount );
				result.Add( rowText );

				tracedCharCount += rowCharCount;
			}
			return result;
		}


		private bool AreAllColumnNamesEmpty( IEnumerable<ColumnDescriptor> columnDescriptors )
		{
			foreach ( ColumnDescriptor columnDescriptor in columnDescriptors ) {
				string name = columnDescriptor.Name;
				if ( !String.IsNullOrEmpty( name ) )
					return false;
			}
			return true;
		}

		private bool AreAllColumnValuesNull( IEnumerable columnValues )
		{
			foreach ( object value in columnValues ) {
				if ( !DBNull.Value.Equals( value ) )
					return false;
			}
			return true;
		}

		private void AutoFormatAllColumns( List<Row> prefetchRows, bool prefetchedAll )
		{
			int columnCount = columnDescriptors.Count;

			for ( int i = 0; i < columnCount; ++i ) {
				List<object> columnValues = new List<object>( prefetchRows.Count );
				foreach ( Row row in prefetchRows ) {
					object columnValue = row[i];
					columnValues.Add( columnValue );
				}
				ColumnDescriptor columnDescriptor = this.columnDescriptors[i];
				ColumnFormat columnFormat = columnDescriptor.Format;

				if ( prefetchedAll && AreAllColumnValuesNull( columnValues ) ) {
					columnFormat.MaxWidth = ColumnFormat.NullText.Length;
				}
				else {
					columnFormat.AutoFormat( columnValues, prefetchedAll );
				}

				if ( columnFormat.LimitWidthByUserOptions && columnFormat.MaxWidth > this.options.MaxResultColumnWidth ) {
					columnFormat.MaxWidth = this.options.MaxResultColumnWidth;
				}

				if ( columnFormat.MaxWidth < columnDescriptor.Name.Length ) {
					columnFormat.MaxWidth = columnDescriptor.Name.Length;
				}
			}
		}

		private class TraceWriter : TextWriter
		{
			public TraceWriter()
			{
			}

			public override System.Text.Encoding Encoding
			{
				get { return System.Text.Encoding.Default; }
			}

			public override void Write( char value )
			{
				Trace.Write( value );
			}
			public override void Write( string value )
			{
				Trace.Write( value );
			}
		}


	}
}

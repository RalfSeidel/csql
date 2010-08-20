using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Text;

namespace csql.ResultTrace
{
	/// <summary>
	/// Helper class to trace the rows of a data reader.
	/// </summary>
	public class DataReaderTracer
	{
		/// <summary>
		/// The data reader for whos data is to be traced.
		/// </summary>
		private readonly IDataReader dataReader;

		/// <summary>
		/// Meta data for each column.
		/// </summary>
		private readonly IList<ColumnInfo> columnInfos;

		public DataReaderTracer( IDataReader dataReader )
		{
			Debug.Assert( dataReader != null && !dataReader.IsClosed );
			this.dataReader = dataReader;
			List<ColumnInfo> columnInfos = new List<ColumnInfo>();
			for ( int i = 0; i < dataReader.FieldCount; ++i ) {
				string name = dataReader.GetName( i );
				Type type = dataReader.GetFieldType( i );
				ColumnInfo columnInfo = new ColumnInfo( i, name, type );
				columnInfos.Add( columnInfo );
			}

			this.columnInfos = columnInfos.ToArray();
		}

		public void TraceAll( TextWriter output )
		{
			TraceHeader( output );
			while ( this.dataReader.Read() ) {
				TraceCurrentRow( output );
			}
		}

		/// <summary>
		/// Traces all to the default trace output.
		/// </summary>
		public void TraceAll()
		{
			TextWriter output = new TraceWriter();
			TraceAll( output );
		}

		public void TraceHeader( TextWriter output )
		{
			for ( int i = 0; i < this.dataReader.FieldCount; ++i ) {
				var columnInfo = columnInfos[i];
				var columnName = columnInfo.Name;
				output.Write( columnName );
				output.Write( new string( ' ', columnInfo.MaxWidth - columnName.Length + 1 ) );
			}
			output.WriteLine();
			for ( int i = 0; i < this.dataReader.FieldCount; ++i ) {
				var columnInfo = columnInfos[i];
				var columnWidth = columnInfo.MaxWidth;
				output.Write( new string( '-', columnWidth ) );
				output.Write( ' ' );
			}
			output.WriteLine();
		}

		public void TraceCurrentRow( TextWriter output )
		{
			int maxLineCount = 0;
			StringCollection[] allColumnLines = new StringCollection[columnInfos.Count];
			for ( int i = 0; i < this.dataReader.FieldCount; ++i ) {
				ColumnInfo columnInfo = columnInfos[i];
				ColumnFormat columnFormat = columnInfo.Format;
				object columnValue = this.dataReader.GetValue( i );
				string columnText = columnFormat.Format( columnValue );
				var columnLines = GetRowColumns( columnInfo, columnText );
				int columnLineCount = columnLines.Count;
				if ( columnLineCount > maxLineCount ) {
					maxLineCount = columnLineCount;
				}
				allColumnLines[i] = columnLines;
			}

			for ( int lineIndex = 0; lineIndex < maxLineCount; ++lineIndex ) {
				for ( int i = 0; i < this.dataReader.FieldCount; ++i ) {
					var columnInfo = columnInfos[i];
					var columnLines = allColumnLines[i];
					if ( lineIndex < columnLines.Count ) {
						string lineText = columnLines[lineIndex];
						output.Write( lineText );
						output.Write( new string( ' ', columnInfo.MaxWidth - lineText.Length + 1 ) );
					}
					else {
						output.Write( new string( ' ', columnInfo.MaxWidth + 1 ) );
					}
				}
				output.WriteLine();
			}
		}

		private static StringCollection GetRowColumns( ColumnInfo columnInfo, string columnValue )
		{
			int tracedCharCount = 0;
			var result = new StringCollection();
			while ( tracedCharCount < columnValue.Length ) {
				int rowCharCount = Math.Min( columnValue.Length - tracedCharCount, columnInfo.MaxWidth );
				var rowText = columnValue.Substring( tracedCharCount, rowCharCount );
				result.Add( rowText );

				tracedCharCount += rowCharCount;
			}
			return result;
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

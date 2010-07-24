using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Text;

namespace csql
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
				ColumnFormat columnFormat = columnInfo.ColumnFormat;
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

		private class ColumnFormat
		{
			private readonly int maxWidth;
			private readonly string format;

			public ColumnFormat( int maxWidth, string format )
			{
				this.maxWidth = maxWidth;
				this.format = format;
			}

			public int MaxWidth { get { return maxWidth; } }

			public virtual string Format( object columnValue )
			{
				if ( DBNull.Value.Equals( columnValue ) ) {
					return "[null]";
				}
				else {
					string result = String.Format( format, columnValue );
					return result;
				}
			}
		}

		private class BinaryFormat : ColumnFormat
		{
			public BinaryFormat( int maxWidth )
				: base( 20, "" )
			{
			}

			public override string Format( object columnValue )
			{
				if ( DBNull.Value.Equals( columnValue ) ) {
					return base.Format( columnValue );
				}
				else {
					byte[] binaryValue = (byte[])columnValue;
					StringBuilder sb = new StringBuilder( 2 * binaryValue.Length + 2 );
					sb.Append( "0x" );
					foreach ( byte b in binaryValue ) {
						sb.AppendFormat( "{0:X2}", b );
					}
					string result = sb.ToString();
					return result;
				}
			}
		}

		private class BooleanFormat : ColumnFormat
		{
			public BooleanFormat()
				: base( 1, "" )
			{
			}

			public override string Format( object columnValue )
			{
				if ( DBNull.Value.Equals( columnValue ) ) {
					return "-";
				}
				else {
					bool boolValue = (bool)columnValue;
					return boolValue ? "1" : "0";
				}
			}
		}

		private class ColumnInfo
		{
			private readonly int index;
			private readonly string name;
			private readonly Type type;
			private readonly ColumnFormat columnFormat;
			private readonly int maxWidth;

			public ColumnInfo( int index, string name, Type type )
			{
				this.index = index;
				this.name = name;
				this.type = type;

				columnFormat = GetColumnFormat( type );

				if ( columnFormat.MaxWidth <= name.Length ) {
					this.maxWidth = name.Length;
				}
				else {
					this.maxWidth = columnFormat.MaxWidth;
				}
			}

			public string Name { get { return this.name; } }

			public ColumnFormat ColumnFormat { get { return columnFormat; } }

			public int MaxWidth { get { return maxWidth; } }

			private static ColumnFormat GetColumnFormat( Type type )
			{
				TypeCode typeCode = Type.GetTypeCode( type );
				switch ( typeCode ) {
					case TypeCode.Boolean:
						return new BooleanFormat();
					case TypeCode.Byte:
						return new ColumnFormat( 5, "{0}" );
					case TypeCode.Char:
						return new ColumnFormat( 1, "{0}" );
					case TypeCode.DateTime:
						return new ColumnFormat( 22, "{0:yyyyMMdd HH':'mm':'ss'.'fff}" );
					case TypeCode.Decimal:
						return new ColumnFormat( 22, "{0}" );
					case TypeCode.Double:
						return new ColumnFormat( 22, "{0}" );
					case TypeCode.Int16:
						return new ColumnFormat( 6, "{0}" );
					case TypeCode.Int32:
						return new ColumnFormat( 10, "{0}" );
					case TypeCode.Int64:
						return new ColumnFormat( 19, "{0}" );
					case TypeCode.SByte:
						return new ColumnFormat( 4, "{0}" );
					case TypeCode.Single:
						return new ColumnFormat( 16, "{0}" );
					case TypeCode.String:
						return new ColumnFormat( 40, "{0}" );
					case TypeCode.UInt16:
						return new ColumnFormat( 16, "{0}" );
					case TypeCode.UInt32:
						return new ColumnFormat( 9, "{0}" );
					case TypeCode.UInt64:
						return new ColumnFormat( 18, "{0}" );
					case TypeCode.Object:
						if ( type == typeof( byte[] ) ) {
							return new BinaryFormat( 20 );
						}
						else if ( type == typeof( Guid ) ) {
							return new ColumnFormat( 38, "{0:B}" );
						}
						else {
							return new ColumnFormat( 40, "{0}" );
						}
					case TypeCode.Empty:
					case TypeCode.DBNull:
						return new ColumnFormat( 6, "{0}" );
					default:
						throw new NotSupportedException( String.Format( "The attribute type {0} is not supported", type.ToString() ) );
				}
			}
		}
	}
}

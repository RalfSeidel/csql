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
			private readonly int m_maxWidth;
			private readonly string m_format;


			public ColumnFormat( int maxWidth, string format )
			{
				m_maxWidth = maxWidth;
				m_format = format;
			}

			public int MaxWidth { get { return m_maxWidth; } }

			public virtual string Format( object columnValue )
			{
				if ( DBNull.Value.Equals( columnValue ) ) {
					return "[null]";
				} else {
					string result = String.Format( m_format, columnValue );
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
				} else {
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
				} else {
					bool boolValue = (bool)columnValue;
					return boolValue ? "1" : "0";
				}
			}
		}

		private class ColumnInfo
		{
			private readonly int Index;
			public readonly string Name;
			private readonly Type Type;

			private readonly ColumnFormat m_columnFormat;
			public ColumnFormat ColumnFormat { get { return m_columnFormat; } }

			private readonly int m_maxWidth;
			public int MaxWidth { get { return m_maxWidth; } }

			public ColumnInfo( int index, string name, Type type )
			{
				this.Index = index;
				this.Name = name;
				this.Type = type;

				m_columnFormat = GetColumnFormat( type );

				if ( m_columnFormat.MaxWidth <= name.Length ) {
					this.m_maxWidth = name.Length;
				} else {
					this.m_maxWidth = m_columnFormat.MaxWidth;
				}
			}

			private static ColumnFormat GetColumnFormat( Type type )
			{
				TypeCode typeCode = Type.GetTypeCode( type );
				switch ( typeCode ) {
					case TypeCode.Boolean:
						return new BooleanFormat(); ;
					case TypeCode.Byte:
						return new ColumnFormat( 5, "{0}" );
					case TypeCode.Char:
						return new ColumnFormat( 1, "{0}" );
					case TypeCode.DateTime:
						return new ColumnFormat( 22, "{0:yyyyMMdd HH':'mm':'ss'.'fff}" ); ;
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
						return new ColumnFormat( 9, "{0}" ); ;
					case TypeCode.UInt64:
						return new ColumnFormat( 18, "{0}" );
					case TypeCode.Object:
						if ( type == typeof( byte[] ) ) {
							return new BinaryFormat( 20 ); ;
						} else if ( type == typeof( Guid ) ) {
							return new ColumnFormat( 38, "{0:B}" );
						} else {
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


		/// <summary>
		/// The data reader for whos data is to be traced.
		/// </summary>
		private readonly IDataReader m_dataReader;

		/// <summary>
		/// Meta data for each column.
		/// </summary>
		private readonly IList<ColumnInfo> m_columnInfos;

		public DataReaderTracer( IDataReader dataReader )
		{
			Debug.Assert( dataReader != null && !dataReader.IsClosed );
			this.m_dataReader = dataReader;
			List<ColumnInfo> columnInfos = new List<ColumnInfo>();
			for ( int i = 0; i < dataReader.FieldCount; ++i ) {
				String name = dataReader.GetName( i );
				Type type = dataReader.GetFieldType( i );
				ColumnInfo columnInfo = new ColumnInfo( i, name, type );
				columnInfos.Add( columnInfo );
			}

			m_columnInfos = columnInfos.ToArray();
		}

		public void TraceAll( TextWriter output )
		{
			TraceHeader( output );
			while ( m_dataReader.Read() ) {
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
			for ( int i = 0; i < m_dataReader.FieldCount; ++i ) {
				var columnInfo = m_columnInfos[i];
				var columnName = columnInfo.Name;
				output.Write( columnName );
				output.Write( new String( ' ', columnInfo.MaxWidth - columnName.Length + 1 ) );
			}
			output.WriteLine();
			for ( int i = 0; i < m_dataReader.FieldCount; ++i ) {
				var columnInfo = m_columnInfos[i];
				var columnWidth = columnInfo.MaxWidth;
				output.Write( new String( '-', columnWidth ) );
				output.Write( ' ' );
			}
			output.WriteLine();
		}

		public void TraceCurrentRow( TextWriter output )
		{
			int maxLineCount = 0;
			StringCollection[] allColumnLines = new StringCollection[m_columnInfos.Count];
			for ( int i = 0; i < m_dataReader.FieldCount; ++i ) {
				ColumnInfo columnInfo = m_columnInfos[i];
				ColumnFormat columnFormat = columnInfo.ColumnFormat;
				object columnValue = m_dataReader.GetValue( i );
				string columnText = columnFormat.Format( columnValue );
				var columnLines = GetRowColumns( columnInfo, columnText );
				int columnLineCount = columnLines.Count;
				if ( columnLineCount > maxLineCount ) {
					maxLineCount = columnLineCount;
				}
				allColumnLines[i] = columnLines;
			}

			for ( int lineIndex = 0; lineIndex < maxLineCount; ++lineIndex ) {
				for ( int i = 0; i < m_dataReader.FieldCount; ++i ) {
					var columnInfo = m_columnInfos[i];
					var columnLines = allColumnLines[i];
					if ( lineIndex < columnLines.Count ) {
						string lineText = columnLines[lineIndex];
						output.Write( lineText );
						output.Write( new String( ' ', columnInfo.MaxWidth - lineText.Length + 1 ) );
					} else {
						output.Write( new String( ' ', columnInfo.MaxWidth + 1 ) );
					}
				}
				output.WriteLine();
			}
		}

		private StringCollection GetRowColumns( ColumnInfo columnInfo, string columnValue )
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

	}
}

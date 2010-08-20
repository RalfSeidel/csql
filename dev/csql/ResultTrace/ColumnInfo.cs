using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	internal class ColumnInfo
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
			} else {
				this.maxWidth = columnFormat.MaxWidth;
			}
		}

		public string Name { get { return this.name; } }

		public ColumnFormat Format { get { return columnFormat; } }

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

	}
}

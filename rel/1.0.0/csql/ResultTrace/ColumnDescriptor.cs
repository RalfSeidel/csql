using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace csql.ResultTrace
{
	/// <summary>
	/// Meta data class for a column in the query result.
	/// </summary>
	internal class ColumnDescriptor
	{
		private readonly string name;
		private readonly ColumnFormat columnFormat;
		[SuppressMessage( "Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="For debugging only." )]
		private readonly int index;
		[SuppressMessage( "Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="For debugging only." )]
		private readonly Type type;

		public ColumnDescriptor( int index, string name, Type type, int maxWidth )
		{
			this.index = index;
			this.name = name;
			this.type = type;

			if ( maxWidth <= name.Length ) {
				maxWidth = name.Length;
			}
			columnFormat = GetColumnFormat( type, maxWidth );
		}

		/// <summary>
		/// Gets the name of the column in the result set.
		/// </summary>
		public string Name { get { return this.name; } }

		public ColumnFormat Format { get { return columnFormat; } }

		private static ColumnFormat GetColumnFormat( Type type, int maxWidth )
		{
			TypeCode typeCode = Type.GetTypeCode( type );
			switch ( typeCode ) {
				case TypeCode.Boolean:
					return new BooleanColumnFormat();
				case TypeCode.Byte:
					return new FixedColumnFormat( 5, "{0}" );
				case TypeCode.Char:
					return new FixedColumnFormat( 1, "{0}" ); 
				case TypeCode.DateTime:
					return new DateTimeColumnFormat();
				case TypeCode.Decimal:
					return new FixedColumnFormat( 22, "{0}" );
				case TypeCode.Double:
					return new FixedColumnFormat( 22, "{0}" );
				case TypeCode.Int16:
					return new FixedColumnFormat( 6, "{0}" );
				case TypeCode.Int32:
					return new FixedColumnFormat( 10, "{0}" );
				case TypeCode.Int64:
					return new FixedColumnFormat( 19, "{0}" );
				case TypeCode.SByte:
					return new FixedColumnFormat( 4, "{0}" );
				case TypeCode.Single:
					return new FixedColumnFormat( 16, "{0}" );
				case TypeCode.String:
					return new StringColumnFormat( maxWidth );
				case TypeCode.UInt16:
					return new FixedColumnFormat( 6, "{0}" );
				case TypeCode.UInt32:
					return new FixedColumnFormat( 9, "{0}" );
				case TypeCode.UInt64:
					return new FixedColumnFormat( 18, "{0}" );
				case TypeCode.Object:
					if ( type == typeof( byte[] ) ) {
						return new BinaryColumnFormat( maxWidth );
					} else if ( type == typeof( Guid ) ) {
						return new FixedColumnFormat( 38, "{0:B}" );
					} else {
						return new StringColumnFormat( maxWidth );
					}
				case TypeCode.Empty:
				case TypeCode.DBNull:
					return new FixedColumnFormat( ColumnFormat.NullText.Length, "{0}" );
				default:
					throw new NotSupportedException( String.Format( "The attribute type {0} is not supported", type.ToString() ) );
			}
		}

	}
}

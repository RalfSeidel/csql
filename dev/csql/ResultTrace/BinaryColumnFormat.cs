using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	internal sealed class BinaryColumnFormat : ColumnFormat
	{
		public BinaryColumnFormat( int maxWidth )
			: base( ColumnFormat.NullText.Length, maxWidth, String.Empty )
		{
		}

		public override bool LimitWidthByUserOptions
		{
			get { return true; }
		}

		public override bool LimitLengthByUserOptions
		{
			get { return true; }
		}

		public override void AutoFormat( IEnumerable<object> prefetchValues, bool fetchedAll )
		{
			int newMaxWidth = MinWidth;
			int nullWidth = ColumnFormat.NullText.Length;

			foreach ( object value in prefetchValues ) {
				if ( DBNull.Value.Equals( value ) ) {
					if ( newMaxWidth < nullWidth ) {
						newMaxWidth = nullWidth;
					}
				} else {
					byte[] data = (byte[])value;
					int length = data.Length * 2 + 2;
					if ( length > newMaxWidth )
						newMaxWidth = length;
				}
			}

			MaxWidth = newMaxWidth;
		}


		protected override string FormatCore( object columnValue )
		{
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

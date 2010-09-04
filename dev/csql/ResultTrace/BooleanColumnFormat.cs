using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	/// <summary>
	/// Format for bit/boolean values.
	/// </summary>
	internal sealed class BooleanColumnFormat : ColumnFormat
	{
		public BooleanColumnFormat()
			: base( 1, 1, String.Empty )
		{
		}

		public override bool LimitWidthByUserOptions
		{
			get { return false; }
		}

		public override bool LimitLengthByUserOptions
		{
			get { return false; }
		}

		/// <summary>
		/// Does nothing because the boolean format and with is fixed.
		/// </summary>
		public override void AutoFormat( IEnumerable<object> prefetchValues, bool fetchedAll )
		{
			int newMaxWidth = MaxWidth;
			int nullWidth = ColumnFormat.NullText.Length;
			foreach ( object value in prefetchValues ) {
				if ( DBNull.Value.Equals( value ) && newMaxWidth < nullWidth ) {
					newMaxWidth = nullWidth;
				}
			}
			MaxWidth = newMaxWidth;
		}

		protected override string FormatCore( object columnValue )
		{
			bool boolValue = (bool)columnValue;
			return boolValue ? "1" : "0";
		}
	}
}

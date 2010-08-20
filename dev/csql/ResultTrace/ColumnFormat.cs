using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	internal class ColumnFormat
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
			} else {
				string result = String.Format( format, columnValue );
				return result;
			}
		}
	}
}

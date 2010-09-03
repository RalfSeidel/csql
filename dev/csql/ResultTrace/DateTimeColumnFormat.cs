using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	internal class DateTimeColumnFormat : ColumnFormat
	{
		private const string DefaultFormat = "{0:yyyy-MM-dd HH':'mm':'ss'.'fff}";
		private const string DateOnlyFormat = "{0:yyyy-MM-dd}";

		public DateTimeColumnFormat()
			: base( ColumnFormat.NullText.Length, 24, DefaultFormat )
		{
		}

		public override void AutoFormat( IEnumerable<object> prefetchValues, bool fetchedAll )
		{
			bool dateOnly = true;
			foreach ( object value in prefetchValues ) {
				if ( DBNull.Value.Equals( value ) )
					continue;

				DateTime dtValue = (DateTime)value;
				if ( !IsDateOnly( dtValue ) ) {
					dateOnly = false;
				}
			}

			if ( dateOnly ) {
				DateTime sample = new DateTime( 2000, 12, 31 );
				string formated = sample.ToString( DateOnlyFormat );
				FormatString = DateOnlyFormat;
				MaxWidth = formated.Length;
			}
		}

		public override string Format( object columnValue )
		{
			if ( DBNull.Value.Equals( columnValue ) )
				return base.Format( columnValue );

			DateTime dtValue = (DateTime)columnValue;
			if ( IsDateOnly( dtValue ) ) {
				return String.Format( DateOnlyFormat, dtValue );
			} else {
				return String.Format( DefaultFormat, dtValue );
			}
		}

		private static bool IsDateOnly( DateTime dt )
		{
			return dt.Equals( dt.Date );
		}
	}
}

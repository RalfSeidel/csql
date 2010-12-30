using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	internal sealed class DateTimeColumnFormat : ColumnFormat
	{
		private const string FullFormat = "{0:yyyy-MM-dd HH':'mm':'ss'.'fff}";
		private const string NoMillisecondsFormat = "{0:yyyy-MM-dd HH':'mm':'ss}";
		private const string NoSecondsFormat = "{0:yyyy-MM-dd HH':'mm}";
		private const string NoTimeFormat = "{0:yyyy-MM-dd}";

		public DateTimeColumnFormat()
			: base( ColumnFormat.NullText.Length, 24, FullFormat )
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

		public override void AutoFormat( IEnumerable<object> prefetchValues, bool fetchedAll )
		{
			bool dateOnly = true;
			bool noMilliseconds = true;
			bool noSeconds = true;
			foreach ( object value in prefetchValues ) {
				if ( DBNull.Value.Equals( value ) )
					continue;

				DateTime dtValue = (DateTime)value;
				if ( !IsDateOnly( dtValue ) ) {
					dateOnly = false;
				}
				if ( dtValue.Millisecond != 0 ) {
					noMilliseconds = false;
					noSeconds = false;
				}
				if ( dtValue.Second != 0 ) {
					noSeconds = false;
				}
			}

			if ( dateOnly ) {
				FormatString = NoTimeFormat;
			}
			else if ( noSeconds ) {
				FormatString = NoSecondsFormat;
			}
			else if ( noMilliseconds ) {
				FormatString = NoMillisecondsFormat;
			}
			else {
				FormatString = FullFormat;
			}
			DateTime sample = new DateTime( 2000, 12, 31, 23, 59, 59, 500 );
			string formated = String.Format( FormatString, sample );
			MaxWidth = formated.Length;
		}

		protected override string FormatCore( object columnValue )
		{
			DateTime dtValue = (DateTime)columnValue;
			if ( IsDateOnly( dtValue ) ) {
				return String.Format( NoTimeFormat, dtValue );
			}
			else if ( dtValue.Second == 0 && dtValue.Millisecond == 0 ) {
				return String.Format( NoSecondsFormat, dtValue );
			}
			else if ( dtValue.Millisecond == 0 ) {
				return String.Format( NoMillisecondsFormat, dtValue );
			}
			else {
				return String.Format( FullFormat, dtValue );
			}
		}

		private static bool IsDateOnly( DateTime dt )
		{
			return dt.Equals( dt.Date );
		}
	}
}

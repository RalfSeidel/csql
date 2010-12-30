using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	internal abstract class ColumnFormat
	{
		/// <summary>
		/// The text traced for null values.
		/// </summary>
		public const string NullText = "[null]";

		private int minWidth;
		private int maxWidth;
		private string format;

		protected ColumnFormat( int defaultMinWidth, int defaultMaxWidth, string defaultFormat )
		{
			this.minWidth = defaultMinWidth;
			this.maxWidth = defaultMaxWidth;
			this.format = defaultFormat;
		}

		public abstract bool LimitWidthByUserOptions
		{
			get;
		}

		public abstract bool LimitLengthByUserOptions
		{
			get;
		}

		public int MinWidth 
		{ 
			get { return minWidth; } 
			protected set { this.minWidth = value; }
		}

		public int MaxWidth 
		{ 
			get { return maxWidth; } 
			set { this.maxWidth = value; }
		}

		public string FormatString
		{
			get { return this.format; }
			set { this.format = value; }
		}

		public abstract void AutoFormat( IEnumerable<object> prefetchValues, bool fetchedAll );

		public string Format( object columnValue )
		{
			if ( DBNull.Value.Equals( columnValue ) ) {
				return ColumnFormat.NullText;
			}
			else {
				return FormatCore( columnValue );
			}
		}

		protected virtual string FormatCore( object columnValue )
		{
			string result = String.Format( FormatString, columnValue );
			return result;
		}
	}
}

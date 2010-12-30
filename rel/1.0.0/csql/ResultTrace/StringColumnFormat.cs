using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	/// <summary>
	/// Format for text columns.
	/// </summary>
	internal sealed class StringColumnFormat : ColumnFormat
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FixedColumnFormat"/> class.
		/// </summary>
		/// <param name="maxWidth">The default max width of a text column.</param>
		/// <param name="format">The fixed format.</param>
		public StringColumnFormat( int maxWidth )
			: base( ColumnFormat.NullText.Length, maxWidth, "{0}" )
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


		/// <summary>
		/// This implementation does nothing because the fixed column format 
		/// does not support variable formats.
		/// </summary>
		/// <param name="prefetchValues">The prefetched values.</param>
		public override void AutoFormat( IEnumerable<object> prefetchValues, bool fetchedAll )
		{
			int newMaxWidth = MinWidth;
			int nullWidth = ColumnFormat.NullText.Length;

			foreach ( object value in prefetchValues ) {
				if ( DBNull.Value.Equals( value ) && newMaxWidth < nullWidth ) {
					newMaxWidth = nullWidth;
				} else {

					string text = value.ToString();
					int length = text.Length;
					if ( length > newMaxWidth )
						newMaxWidth = length;
				}
			}

			MaxWidth = newMaxWidth;
		}
	}
}

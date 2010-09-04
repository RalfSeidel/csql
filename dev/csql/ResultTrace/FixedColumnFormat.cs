using System;
using System.Collections.Generic;
using System.Text;

namespace csql.ResultTrace
{
	/// <summary>
	/// Format for columns having a fixed format.
	/// </summary>
	internal sealed class FixedColumnFormat : ColumnFormat
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FixedColumnFormat"/> class.
		/// </summary>
		/// <param name="width">The fixed width.</param>
		/// <param name="format">The fixed format.</param>
		public FixedColumnFormat( int width, string format )
			: base( width, width, format )
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
		/// This implementation does nothing because the fixed column format 
		/// does not support variable formats.
		/// </summary>
		/// <param name="prefetchValues">The prefetched values.</param>
		public override void AutoFormat( IEnumerable<object> prefetchValues, bool fetchedAll )
		{
		}
	}
}

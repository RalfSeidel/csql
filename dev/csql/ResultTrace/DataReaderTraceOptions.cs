
namespace csql.ResultTrace
{
	/// <summary>
	/// Options for the trace of query result.
	/// </summary>
	internal class DataReaderTraceOptions
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DataReaderTraceOptions()
		{
			AutoFormatPrefetchCount = 10;
			MaxResultColumnWidth = 40;
			MaxResultColumnLength = int.MaxValue;
		}

		/// <summary>
		/// The number of rows retrieved to determine the maximal length and default format
		/// of a column value.
		/// </summary>
		public int AutoFormatPrefetchCount { get; set; }

		/// <summary>
		/// Gets or sets the maximal width of a single result column when traceing query results.
		/// Any data that is longer than the specified length is wrapped.
		/// </summary>
		public int MaxResultColumnWidth { get; set; }

		/// <summary>
		/// Gets or sets the maximal length of a single result column. Any data 
		/// that is longer will be truncated.
		/// </summary>
		public int MaxResultColumnLength { get; set; }
	}
}

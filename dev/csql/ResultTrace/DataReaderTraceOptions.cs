
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
			MaxResultColumnWidth = 40;
		}

		/// <summary>
		/// Gets or sets the maximal width of a single result column when traceing query results.
		/// </summary>
		public int MaxResultColumnWidth { get; set; }
	}
}

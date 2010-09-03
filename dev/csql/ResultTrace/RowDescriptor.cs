using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;

namespace csql.ResultTrace
{
	internal class RowDescriptor
	{
		private ColumnDescriptor[] columns;

		public RowDescriptor( IList<ColumnDescriptor> columns )
		{
			Debug.Assert( columns != null );
			this.columns = new ColumnDescriptor[columns.Count];
			for ( int i = 0; i < columns.Count; ++i ) {
				ColumnDescriptor column = columns[i];
				Debug.Assert( column != null );
				this.columns[i] = column;
			}
		}

		public static RowDescriptor CreateRowDescriptor( IDataReader dataReader, DataReaderTraceOptions options )
		{
			List<ColumnDescriptor> columns = new List<ColumnDescriptor>( dataReader.FieldCount );
			for ( int i = 0; i < dataReader.FieldCount; ++i ) {
				string name = dataReader.GetName( i );
				Type type = dataReader.GetFieldType( i );
				ColumnDescriptor columnInfo = new ColumnDescriptor( i, name, type, options.MaxResultColumnWidth );
				columns.Add( columnInfo );
			}

			RowDescriptor result = new RowDescriptor( columns );
			return result;
		}
	}
}

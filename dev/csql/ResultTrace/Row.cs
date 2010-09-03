using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace csql.ResultTrace
{
	/// <summary>
	/// Simple container for all values returned by a data reader.
	/// </summary>
	internal class Row : IEnumerable
	{
		private readonly object[] columnValues;

		public Row( IDataReader dataReader )
		{
			this.columnValues = new object[dataReader.FieldCount];
			for ( int i = 0; i < dataReader.FieldCount; ++i ) {
				object fieldValue = dataReader.GetValue( i );
				this.columnValues[i] = fieldValue;
			}
		}

		public int Count
		{
			get
			{
				return this.columnValues == null ? 0 : this.columnValues.Length;
			}
		}

		public object this[int index]
		{
			get
			{
				object value = this.columnValues[index];
				return value;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.columnValues.GetEnumerator();
		}
	}
}

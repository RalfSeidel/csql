using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sqt.DbcProvider;
using System.ComponentModel;

namespace csql.addin.Settings
{
	internal class CSqlSettings : INotifyPropertyChanged
	{
		private DbConnectionParameter dbConnectionParameter;
		private CSqlParameter csqlParameter;


		/// <summary>
		/// Add summary description
		/// </summary>
		public DbConnectionParameter DbConnectionParameter
		{
			get
			{
				return dbConnectionParameter;
			}
			set
			{
				if ( !Object.Equals( dbConnectionParameter, value ) ) {
					dbConnectionParameter = value;
					OnPropertyChanged( "DbConnection" );
				}
			}
		}


		/// <summary>
		/// Add summary description
		/// </summary>
		public CSqlParameter CSqlParameter
		{
			get
			{
				return csqlParameter;
			}
			set
			{
				if ( !Object.Equals( csqlParameter, value ) ) {
					csqlParameter = value;
					OnPropertyChanged( "CSqlParameter" );
				}
			}
		}






		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		protected internal void OnPropertyChanged( string propertyName )
		{
			if ( PropertyChanged != null ) {
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}

		#endregion
	}
}

using System;

namespace Sqt.DbcProvider
{
	public partial class Datasources
	{
		/// <summary>
		/// Get the datasources stored in the connection history with the specified address and ip port.
		/// </summary>
		/// <remarks>
		/// The method first seeks for an entry where address and port number matches.
		/// </remarks>
		/// <param name="address">
		/// The address of the datasource, i.e. the host name or ip address.
		/// The address case is irrelevant.
		/// </param>
		/// <param name="port">The server connection ip port or zero if not used.</param>
		public Datasource FindDatasourceByAddress( string address, int port )
		{
			foreach ( var ds in this.Datasource ) {
				if ( !ds.Address.Equals( address, StringComparison.CurrentCultureIgnoreCase ) )
					continue;
				if ( ds.Port != port )
					continue;

				return ds;
			}
			// If the port number was not specified return the first entry where only the address matches.
			if ( port == 0 ) {
				foreach ( var ds in this.Datasource ) {
					if ( !ds.Address.Equals( address, StringComparison.CurrentCultureIgnoreCase ) )
						continue;

					return ds;
				}
			}
			return null;
		}

		public Datasource FindDatasourceByComment( string comment )
		{
			foreach ( var ds in this.Datasource ) {
				if ( !String.Equals( ds.Comment, comment, StringComparison.CurrentCultureIgnoreCase ) )
					continue;

				return ds;
			}
			return null;
		}
	}

}

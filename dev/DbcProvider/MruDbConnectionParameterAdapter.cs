using System;
using System.Collections.Generic;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Bridge between the most recently used connection store
	/// and the connection parameters currently used.
	/// </summary>
	public static class MruDbConnectionParameterAdapter
	{
		private const int MaxMruEntries = 10;


		/// <summary>
		/// Get the moust recently used connection parameter stored in the murConnection list.
		/// </summary>
		/// <param name="mruConnections">The most recently used database connection.</param>
		/// <returns>The first entry in the list.</returns>
		public static DbConnectionParameter GetMruDbConnectionParameter( MruConnections mruConnections )
		{
			DbConnectionParameter result = new DbConnectionParameter();
			if ( mruConnections.Datasources == null || mruConnections.Datasources.Count == 0 )
				return result;

			Datasources mruDatasources = mruConnections.Datasources[0];
			result.Provider = mruDatasources.Provider;

			if ( mruDatasources.Datasource == null || mruDatasources.Datasource.Count == 0 )
				return result;

			Datasource mruDatasource = mruDatasources.Datasource[0];
			result.DatasourceAddress = mruDatasource.Address;
			result.DatasourcePort = mruDatasource.Port;
			result.DatasourceComment = mruDatasource.Comment;

			if ( mruDatasource.Catalogs != null && mruDatasource.Catalogs.Count != 0 ) {
				string mruCatalog = mruDatasource.Catalogs[0];
				result.Catalog = mruCatalog;
			}

			if ( mruDatasource.Authentications != null && mruDatasource.Authentications.Count != 0 ) {
				Authentication mruAuthentication = mruDatasource.Authentications[0];
				result.IntegratedSecurity = mruAuthentication.Integrated;
				result.UserId = mruAuthentication.UserId;
				result.Password = mruAuthentication.Password;
			}

			return result;
		}


		/// <summary>
		/// Update the most recently used connection store with the latest parameters used.
		/// </summary>
		/// <param name="mruConnections">The most recently used connection store.</param>
		/// <param name="mruParameter">The most recently connection parameter used.</param>
		/// <returns><c>true</c> if the store was update. <c>false</c> if the store is up to date.</returns>
		public static bool SetMruDbConnectionParameter( MruConnections mruConnections, DbConnectionParameter mruParameter )
		{
			if ( mruParameter == null )
				return false;

			DbConnectionParameter currentMruParameter = MruDbConnectionParameterAdapter.GetMruDbConnectionParameter( mruConnections );
			if ( Object.Equals( currentMruParameter, mruParameter ) )
				return false;

			DatasourcesList newMruDatasources = new DatasourcesList();
			if ( mruParameter.Provider != ProviderType.Undefined ) {
				Datasources newFirstDatasources = new Datasources() { Provider = mruParameter.Provider };
				ProviderDataSources datasources = new ProviderDataSources( mruParameter.Provider );
				if ( !String.IsNullOrEmpty( mruParameter.DatasourceAddress ) ) {
					Datasource newFirstDatasource = new Datasource()
					{
						Address = mruParameter.DatasourceAddress,
						Port = mruParameter.DatasourcePort,
						Comment = mruParameter.DatasourceComment
					};
					datasources.Add( newFirstDatasource );

					AddCatalogs( newFirstDatasource, mruConnections, mruParameter );
					AddAuthentications( newFirstDatasource, mruConnections, mruParameter );
					AddDatasourcesFromMruList( datasources, mruConnections );
				}
				newFirstDatasources.Datasource = datasources;

				newMruDatasources.Add( newFirstDatasources );
			}

			// Add all previously used datasources 
			IEnumerable<Datasources> otherDatasources = mruConnections.Datasources;
			foreach( Datasources mruDatasources in otherDatasources ) {
				if ( newMruDatasources.Count >= MaxMruEntries )
					break;

				if ( newMruDatasources.Contains( mruDatasources ) ) {
					continue;
				}

				newMruDatasources.Add( mruDatasources );
			}

			mruConnections.Datasources = newMruDatasources;

			return true;
		}

		private static void AddCatalogs( Datasource datasource, MruConnections mruConnections, DbConnectionParameter mruParameter )
		{
			List<string> catalogs = new List<string>();
			catalogs.Add( mruParameter.Catalog ?? "" );
			AddCatalogsFromMruList( catalogs, mruConnections, mruParameter.Provider, datasource );
			datasource.Catalogs = catalogs;
		}

		private static void AddCatalogsFromMruList( ICollection<string> catalogs, MruConnections mruConnections, ProviderType providerId, Datasource datasource )
		{
			Datasources providerDatasources = mruConnections.FindDatasources( providerId );
			if ( providerDatasources == null )
				return;

			if ( providerDatasources.Datasource == null || providerDatasources.Datasource.Count == 0 )
				return;

			Datasource mruDatasource = providerDatasources.FindDatasourceByAddress( datasource.Address, datasource.Port );
			if ( mruDatasource == null || mruDatasource.Catalogs == null )
				return;

			foreach ( string catalog in mruDatasource.Catalogs ) {
				if ( catalogs.Count >= MaxMruEntries )
					break;
				if ( !catalogs.Contains( catalog ) ) {
					catalogs.Add( catalog );
				}
			}
		}

		private static void AddAuthentications( Datasource datasource, MruConnections mruConnections, DbConnectionParameter mruParameter )
		{
			AuthenticationList authentications = new AuthenticationList();
			Authentication authentication = new Authentication()
			{
				Integrated = mruParameter.IntegratedSecurity,
				UserId = mruParameter.UserId,
				Password = mruParameter.Password
			};

			if ( authentication.IsUsable ) {
				authentications.Add( authentication );
			}
			AddAuthenticationsFromMruList( authentications, mruConnections, mruParameter.Provider, datasource );
			datasource.Authentications = authentications;
		}

		private static void AddAuthenticationsFromMruList( AuthenticationList authentications, MruConnections mruConnections, ProviderType providerId, Datasource datasource )
		{
			Datasources providerDatasources = mruConnections.FindDatasources( providerId );
			if ( providerDatasources == null )
				return;

			if ( providerDatasources.Datasource == null || providerDatasources.Datasource.Count == 0 )
				return;

			Datasource mruDatasource = providerDatasources.FindDatasourceByAddress( datasource.Address, datasource.Port );
			if ( mruDatasource == null || mruDatasource.Authentications == null )
				return;

			foreach ( var authentication in mruDatasource.Authentications ) {
				if ( authentications.Count >= MaxMruEntries )
					break;
				if ( !authentications.Contains( authentication ) ) {
					authentications.Add( authentication );
				}
			}
		}

		private static void AddDatasourcesFromMruList( ProviderDataSources datasources, MruConnections mruConnections )
		{
			ProviderType providerId = datasources.ProviderType;
			Datasources provider = mruConnections.FindDatasources( providerId );
			if ( provider == null )
				return;

			if ( provider.Datasource == null || provider.Datasource.Count == 0 )
				return;

			foreach ( Datasource datasource in provider.Datasource ) {
				if ( !ContainsDatasource( datasources, datasource.Address ) ) {
					datasources.Add( datasource );
				}
			}
		}


		public static bool ContainsDatasource( IEnumerable<Datasource> datasources, string datasource )
		{
			foreach ( var item in datasources ) {
				if ( item.Address.Equals( datasource, StringComparison.CurrentCultureIgnoreCase ) )
					return true;
			}
			return false;
		}


		private class ProviderDataSources : List<Datasource>
		{
			public ProviderDataSources( ProviderType providerId )
			{
				this.ProviderType = providerId;
			}

			public ProviderType ProviderType { get; private set; }
		}


		private class DatasourcesList : List<Datasources>
		{
			public new bool Contains( Datasources datatasources )
			{
				foreach ( var item in this ) {
					if ( item.Provider == datatasources.Provider )
						return true;
				}

				return false;
			}
		}


		private class AuthenticationList : List<Authentication>
		{
			public new bool Contains( Authentication authentication )
			{
				foreach ( var item in this ) {
					if ( Object.Equals( item, authentication ) )
						return true;

					// Allow only one item with integrated security
					if ( authentication.Integrated && item.Integrated )
						return true;
				}

				return false;
			}
		}
	}
}

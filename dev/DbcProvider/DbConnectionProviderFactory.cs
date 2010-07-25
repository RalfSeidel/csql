using System;
using System.Collections.Generic;
using Sqt.DbcProvider.Provider;


namespace Sqt.DbcProvider
{
	/// <summary>
	/// Meta factory to provide the factories for supported database provider.
	/// </summary>
	public static class DbConnectionProviderFactory
	{
		private static readonly IDictionary<ProviderType, IDbcProvider> provider = new Dictionary<ProviderType, IDbcProvider>();

		static DbConnectionProviderFactory() 
		{
			provider.Add( ProviderType.MsSql, new MsSqlConnectionFactory() );
			provider.Add( ProviderType.Sybase, new SybaseConnectionFactory() );
			provider.Add( ProviderType.Oracle, new OracleConnectionFactory() );
			provider.Add( ProviderType.IbmDb2, new IbmDb2ConnectionFactory() );
		}

		/// <summary>
		/// Gets the factory for the specified provider.
		/// </summary>
		public static IDbcProvider GetFactory( ProviderType providerType )
		{
			if ( providerType == ProviderType.Undefined )
				throw new ArgumentNullException( "providerType" );

			IDbcProvider factory;
			bool exists = provider.TryGetValue( providerType, out factory );
			if ( !exists ) {
				throw new ArgumentException( "Unsupported database connection provider: " + providerType, "providerType" );
			}

			return factory;
		}
	}
}

using System.Data;

namespace Sqt.DbcProvider.Provider
{
	/// <summary>
	/// Interface for a factory of database connection.
	/// </summary>
	public interface IWrappedDbConnectionFactory
	{
		/// <summary>
		/// Gets the name of the provider as used by the ADO provider/factory model.
		/// </summary>
		string ProviderName { get; }

		/// <summary>
		/// Gets the connection string used to open the connection.
		/// </summary>
		string GetConnectionString( DbConnectionParameter parameter );

		/// <summary>
		/// Creates an connection for the specified connection parameter.
		/// </summary>
		/// <param name="parameter">The connection parameter parameter.</param>
		WrappedDbConnection CreateConnection( DbConnectionParameter parameter );
	}
}

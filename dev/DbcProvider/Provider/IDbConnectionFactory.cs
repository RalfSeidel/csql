using System.Data;

namespace Sqt.DbcProvider.Provider
{
	/// <summary>
	/// Interface for a factory of database connection.
	/// </summary>
	public interface IDbConnectionFactory
	{
		/// <summary>
		/// Creates an connection for the specified connection parameter.
		/// </summary>
		/// <param name="parameter">The connection parameter parameter.</param>
		DbConnection CreateConnection( DbConnectionParameter parameter );
	}
}

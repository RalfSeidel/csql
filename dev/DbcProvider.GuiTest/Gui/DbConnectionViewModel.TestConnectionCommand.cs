using System;
using System.Data;
using System.Data.Common;
using System.Windows.Input;
using Sqt.DbcProvider.Provider;
using System.Windows;

namespace Sqt.DbcProvider.Gui
{
	public partial class DbConnectionViewModel
	{
		/// <summary>
		/// The command to execute when the users wants to try to connect with the database.
		/// </summary>
		private class TestConnectionCommand : ICommand
		{

			/// <summary>
			/// Initializes a new instance of the <see cref="TestConnectionCommand"/> class.
			/// </summary>
			/// <param name="viewModel">The view model this command belongs to.</param>
			public TestConnectionCommand( DbConnectionViewModel viewModel )
			{
			}

			#region ICommand Members

			public event EventHandler CanExecuteChanged;

			/// <summary>
			/// Defines the method that determines whether the command can execute in its current state.
			/// </summary>
			/// <param name="parameter">The connection parameter.</param>
			/// <returns>
			/// true if this command can be executed; otherwise, false.
			/// </returns>
			public bool CanExecute( object parameter )
			{
				if ( !(parameter is DbConnectionParameter) )
					return false;

				DbConnectionParameter connectionParameter = (DbConnectionParameter)parameter;
				if ( connectionParameter == null || connectionParameter.Provider == ProviderType.Undefined )
					return false;

				if ( !connectionParameter.IntegratedSecurity ) {
					if ( String.IsNullOrEmpty( connectionParameter.UserId ) )
						return false;
				}


				return true;
			}


			/// <summary>
			/// Defines the method to be called when the command is invoked.
			/// </summary>
			/// <param name="parameter">The connection parameter.</param>
			public void Execute( object parameter )
			{
				DbConnectionParameter connectionParameter = (DbConnectionParameter)parameter;
				IDbConnectionFactory connectionFactory = DbConnectionFactoryProvider.GetFactory( connectionParameter.Provider );
				DbConnection dbConnection = null;
				bool succeeded = false;
				try {
					dbConnection = connectionFactory.CreateConnection( connectionParameter );
					succeeded = true;
				}
				catch ( DbException ) {
				}
				finally {
					if ( dbConnection != null ) {
						dbConnection.Dispose();
					}
				}

				if ( succeeded ) {
					MessageBox.Show( "Connection succeeded", "Connection Test", MessageBoxButton.OK, MessageBoxImage.Information );
				} else {
					MessageBox.Show( "Connection failed", "Connection Test", MessageBoxButton.OK, MessageBoxImage.Error );
				}
			}


			public void OnCanExecuteChanged()
			{
				if ( this.CanExecuteChanged != null ) {
					CanExecuteChanged( this, EventArgs.Empty );
				}
			}

			#endregion
		}
	}
}

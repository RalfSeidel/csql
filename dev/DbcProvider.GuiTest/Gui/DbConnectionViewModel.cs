using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace Sqt.DbcProvider.Gui
{
	/// <summary>
	/// Model for the db connection history control
	/// </summary>
	public partial class DbConnectionViewModel : INotifyPropertyChanged
	{
		#region Property fields

		/// <summary>
		/// The most recentently used connection history this view model is based on.
		/// </summary>
		private MruConnections mruConnections;

		/// <summary>
		/// Provider combo box items.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private ProviderList providerList;

		/// <summary>
		/// Currently selected provider.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ProviderType provider;

		/// <summary>
		/// Datasource / server combo box items.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private ComboList datasourceList;

		/// <summary>
		/// Currently entered combo box.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string datasource;

		/// <summary>
		/// Catalog / database combobox items.
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private ComboList catalogList;

		/// <summary>
		/// Currently entered / selected catalog
		/// </summary>
		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string catalog;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private Authentication authentication = new Authentication();

		private readonly TestConnectionCommand testConnectionCommand;

		private readonly SaveCommand saveCommand;

		/// <summary>
		/// Flag indicating if the user has entered a data source name
		/// of if the view model only contains the connection mru.
		/// </summary>
		private bool hasDatasourceChanged;

		/// <summary>
		/// Flag indicating if the user has entered a catalog name
		/// of if the view model only contains the connection history
		/// file.
		/// </summary>
		private bool hasCatalogChanged;

		/// <summary>
		/// Flag indicating if the user has entered a login parameter
		/// of if the view model only contains the values 
		/// from the connection history file.
		/// </summary>
		private bool hasLoginChanged;


		#endregion

		#region Properties

		/// <summary>
		/// Gets the connections this view model is based on
		/// </summary>
		internal MruConnections MruConnections
		{
			get
			{
				return this.mruConnections;
			}
		}

		/// <summary>
		/// Gets the collection of supported providers.
		/// </summary>
		public ProviderList Providers
		{
			get
			{
				if ( this.providerList == null ) {
					this.providerList = ProviderList.CreateProviderList();
				}
				return this.providerList;
			}
			set
			{
				this.providerList = value;
				OnPropertyChanged( "Providers" );
			}
		}

		/// <summary>
		/// Gets or sets the provider to connection with.
		/// </summary>
		public ProviderType Provider
		{
			get
			{
				return this.provider;
			}
			set
			{
				if ( Object.Equals( this.provider, value ) ) 
					return;

				string currentDatasource = Datasource;
				this.provider = value;
				this.datasourceList = LoadDataSources( this.provider );
				OnPropertyChanged( "Provider" );
				OnPropertyChanged( "Datasources" );
				OnPropertyChanged( "ConnectionParameter" );
				this.testConnectionCommand.OnCanExecuteChanged();

				if ( this.hasDatasourceChanged || this.datasourceList.Contains( currentDatasource ) ) {
					Datasource = currentDatasource;
				} else if ( Datasources.Any() ) {
					ComboListItem firstDatasource = Datasources.First();
					Datasource = firstDatasource.Name;
				} else {
					Datasource = "";
				}
			}
		}

		/// <summary>
		/// Gets the datasources presented in the server combo box.
		/// </summary>
		public IEnumerable<ComboListItem> Datasources
		{
			get
			{
				if ( this.datasourceList == null ) {
					datasourceList =  LoadDataSources( this.provider );
				}
				return this.datasourceList;
			}
		}

		/// <summary>
		/// Gets or sets the datasource (server) to connect with.
		/// </summary>
		public string Datasource
		{
			get
			{
				return this.datasource;
			}
			set
			{
				if ( Object.Equals( this.datasource, value ) )
					return;

				string currentCatalog = Catalog;
				string currentUserId = UserId;
				string currentPassword = Password;
				bool currentIntegratedSecurity = IntegratedSecurity;

				this.hasDatasourceChanged = IsManualEnteredDatasource( value );
				this.datasource = value;
				this.catalogList = LoadCatalogs( Provider, Datasource, 0 );
				OnPropertyChanged( "Datasource" );
				OnPropertyChanged( "Catalogs" );
				OnPropertyChanged( "ConnectionParameter" );
				testConnectionCommand.OnCanExecuteChanged();

				if ( this.hasCatalogChanged || this.catalogList.Contains( currentCatalog ) ) {
					Catalog = currentCatalog;
				} else if ( Catalogs.Any() ) {
					ComboListItem firstCatalog = Catalogs.First();
					Catalog = firstCatalog.Name;
				} else {
					Catalog = "";
				}

				if ( this.hasLoginChanged ) {
					IntegratedSecurity = currentIntegratedSecurity;
					UserId = currentUserId;
					Password = currentPassword;
				} else {
					Authentication authentication = LoadAuthentication( Provider, Datasource, 0 );
					if ( authentication != null ) {
						IntegratedSecurity = authentication.Integrated;
						UserId = authentication.UserId;
						Password = authentication.Password;
					}
				}

			}
		}

		/// <summary>
		/// Gets the list of catalogs.
		/// </summary>
		public IEnumerable<ComboListItem> Catalogs
		{
			get
			{
				if ( this.catalogList == null ) {
					this.catalogList = LoadCatalogs( Provider, Datasource, 0 );
					if ( catalogList.Any() ) {
						string firstCatalog = catalogList.First().Name;
						Catalog = firstCatalog;
					}
				}
				return this.catalogList;
			}
		}

		/// <summary>
		/// Gets or sets the intial catalog.
		/// </summary>
		public string Catalog
		{
			get
			{
				return this.catalog;
			}
			set
			{
				if ( this.catalog == value ) 
					return;

				this.hasCatalogChanged = IsManualEnteredCatalog( value );
				this.catalog = value;
				OnPropertyChanged( "Catalog" );
				OnPropertyChanged( "ConnectionParameter" );
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether to use integrated security or server security for the logon.
		/// </summary>
		public bool IntegratedSecurity
		{
			get
			{
				return this.authentication.Integrated;
			}
			set
			{
				if ( this.authentication.Integrated != value ) {
					this.authentication.Integrated = value;
					this.hasLoginChanged = true;
					OnPropertyChanged( "IntegratedSecurity" );
					OnPropertyChanged( "ConnectionParameter" );
					this.testConnectionCommand.OnCanExecuteChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the user ID.
		/// </summary>
		public string UserId
		{
			get
			{
				return this.authentication.UserId;
			}
			set
			{
				if ( this.authentication.UserId != value ) {
					if ( !String.IsNullOrEmpty( value ) ) {
						this.hasLoginChanged = true;
					}
					this.authentication.UserId = value;
					OnPropertyChanged( "UserId" );
					OnPropertyChanged( "ConnectionParameter" );
					this.testConnectionCommand.OnCanExecuteChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		public string Password
		{
			get
			{
				return this.authentication.Password;
			}
			set
			{
				if ( this.authentication.Password != value ) {
					if ( !String.IsNullOrEmpty( value ) ) {
						this.hasLoginChanged = true;
					}
					this.authentication.Password = value;
					OnPropertyChanged( "ConnectionParameter" );
					OnPropertyChanged( "Password" );
				}
			}
		}


		/// <summary>
		/// Get the currently selected connection parameter.
		/// </summary>
		/// <value>The connection parameter.</value>
		public DbConnectionParameter ConnectionParameter
		{
			get
			{
				DbConnectionParameter result = new DbConnectionParameter();

				if ( this.Provider == ProviderType.Undefined )
					return null;

				result.Provider = this.Provider;
				result.DatasourceAddress = this.Datasource;
				//result.DatasourcePort = this.Port;
				result.Catalog = this.Catalog;
				result.IntegratedSecurity = this.IntegratedSecurity;
				result.UserId = this.UserId;
				result.Password = this.Password;

				return result;
			}
		}


		/// <summary>
		/// Command bound to the test connection button.
		/// </summary>
		public ICommand TestConnection
		{
			get { return this.testConnectionCommand; }
		}

		/// <summary>
		/// Command bound to save button.
		/// </summary>
		public ICommand Save
		{
			get { return this.saveCommand; }
		}

		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public DbConnectionViewModel()
		{
			this.testConnectionCommand = new TestConnectionCommand( this );
			this.saveCommand = new SaveCommand( this );
		}

		/// <summary>
		/// Load view model from persisted mru file.
		/// </summary>
		/// <param name="history"></param>
		public void Load( MruConnections history )
		{
			this.mruConnections = history;
			this.Providers = ProviderList.CreateProviderList( history );
			this.hasCatalogChanged = false;
			this.hasDatasourceChanged = false;
			this.hasLoginChanged = false;
		}


		private ComboList LoadDataSources( ProviderType currentProvider )
		{
			if ( this.mruConnections == null ) {
				return new ComboList();
			}


			var datasources = mruConnections.FindDatasources( currentProvider );

			if ( datasources == null )
				return new ComboList();

			List<string> datasourceNames = new List<string>();
			foreach ( var datasource in datasources.Datasource ) {
				string dsAddress = datasource.Address;
				if ( !datasourceNames.Contains( dsAddress ) ) 
					datasourceNames.Add( dsAddress );
			}

			ComboList cl = new ComboList( datasourceNames );
			return cl;
		}

		private ComboList LoadCatalogs( ProviderType currentProvider, string currentDatasource, int currentPort )
		{
			if ( this.mruConnections == null ) {
				return new ComboList();
			}

			var datasource = mruConnections.FindDatasourceByAddress( currentProvider, currentDatasource, currentPort );
			if ( datasource == null )
				return new ComboList();

			var catalogNames = new List<string>();
			foreach ( var catalog in datasource.Catalogs ) {
				if ( !catalogNames.Contains( catalog ) )
					catalogNames.Add( catalog );
			}

			ComboList cl = new ComboList( catalogNames );
			return cl;
		}

		private Authentication LoadAuthentication( ProviderType currentProvider, string currentDatasource, int currentPort )
		{
			if ( this.mruConnections == null )
				return new Authentication();

			var datasource = mruConnections.FindDatasourceByAddress( currentProvider, currentDatasource, currentPort );
			if ( datasource == null )
				return new Authentication();

			if ( datasource.Authentications.Any() )
				return datasource.Authentications.First();

			return null;
		}


		/// <summary>
		/// Check if the specified datasource was taken from the list 
		/// of datasources or entered manually.
		/// </summary>
		/// <param name="datasource">The name of the datasource</param>
		private bool IsManualEnteredDatasource( string datasource )
		{
			if ( String.IsNullOrEmpty( datasource ) )
				return false;

			if ( this.datasourceList == null )
				return true;

			foreach ( var item in this.datasourceList ) {
				if ( item.IsRefresh )
					continue;

				if ( item.Name.Equals( datasource, StringComparison.CurrentCultureIgnoreCase ) )
					return false;
			}

			return true;
		}


		/// <summary>
		/// Check if the specified datasource was taken from the list 
		/// of catalog or entered manually.
		/// </summary>
		/// <param name="catalog">The name of the catalog</param>
		private bool IsManualEnteredCatalog( string catalog )
		{
			if ( String.IsNullOrEmpty( catalog ) )
				return false;

			if ( this.catalogList == null )
				return true;

			foreach ( var item in this.catalogList ) {
				if ( item.IsRefresh )
					continue;

				if ( item.Name.Equals( catalog, StringComparison.CurrentCultureIgnoreCase ) )
					return false;
			}

			return true;
		}





		#region INotifyPropertyChanged Members

		/// <summary>
		/// Event raised when a property value changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged( string p )
		{
			if ( this.PropertyChanged != null ) {
				this.PropertyChanged( this, new PropertyChangedEventArgs( p ) );
			}
		}

		#endregion

		#region Provider List

		/// <summary>
		/// Item for the list of providers in the provider selection combo box.
		/// </summary>
		[DebuggerDisplay( "{Id}" )]
		public class ProviderListItem
		{
			private readonly ProviderType id;
			private readonly string name;

			public ProviderType ID { get { return this.id; } }

			public string Name { get { return this.name; } }

			public ProviderListItem( ProviderType id, string name )
			{
				this.id = id;
				this.name = name;
			}

			public override string ToString()
			{
				return Name;
			}
		}

		/// <summary>
		/// List of providers for the provider selection combo box.
		/// </summary>
		public class ProviderList : IEnumerable<ProviderListItem>
		{
			private readonly IList<ProviderListItem> items = new List<ProviderListItem>();

			private ProviderList()
			{
			}

			/// <summary>
			/// Static constructor.
			/// </summary>
			internal static ProviderList CreateProviderList()
			{
				ProviderList list = new ProviderList();

				var enumValues = System.Enum.GetValues( typeof( ProviderType ) );
				foreach ( ProviderType e in enumValues ) {
					if ( e != ProviderType.Undefined ) {
						var listItem = new ProviderListItem( e, e.ToString() );
						list.items.Add( listItem );
					}
				}
				return list;
			}

			/// <summary>
			/// Static constructor that inserts the most recently used providers
			/// first.
			/// </summary>
			internal static ProviderList CreateProviderList( MruConnections history )
			{
				if ( history == null )
					throw new ArgumentNullException( "history" );

				ProviderList list = new ProviderList();

				// Add all providers in the order of their appearance in 
				// the connection history.
				foreach ( var mruItem in history.Datasources ) {
					var provider = mruItem.Provider;

					if ( provider == ProviderType.Undefined )
						continue;

					if ( list.Contains( provider ) )
						continue;

					var listItem = new ProviderListItem( provider, provider.ToString() );
					list.items.Add( listItem );
				}

				var enumValues = System.Enum.GetValues( typeof( ProviderType ) );
				foreach ( ProviderType e in enumValues ) {
					if ( e == ProviderType.Undefined ) 
						continue;

					if ( list.Contains( e ) )
						continue;

					var listItem = new ProviderListItem( e, e.ToString() );
					list.items.Add( listItem );
				}

				return list;
			}

			/// <summary>
			/// Determines whether this list already contains an item for the specified provider.
			/// </summary>
			/// <param name="provider">The provider id.</param>
			/// <returns>
			/// <c>true</c> if this list already contains an item for the specified provider; otherwise, <c>false</c>.
			/// </returns>
			public bool Contains( ProviderType provider )
			{
				foreach ( var item in this.items ) {
					if ( item.ID == provider )
						return true;
				}
				return false;
			}

			public override string ToString()
			{
				string result = "";
				string separator = "";
				foreach ( var item in this.items ) {
					result+= separator;
					result+= item.ToString();
					separator = ", ";
				}
				return result;
			}

			#region IEnumerable Members

			public IEnumerator<ProviderListItem> GetEnumerator()
			{
				return this.items.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

		}

		#endregion

		#region Combo List

		/// <summary>
		/// Item for the list of catalogs in the Catalog selection combo box.
		/// </summary>
		public class ComboListItem
		{
			private readonly bool isRefreshItem;

			private readonly string name;

			/// <summary>
			/// If the item with IsRefresh == true is selected the app logic has to try to 
			/// enumerate all accessible catalog of the currently selected datasource and
			/// to present them in the catalogs combo box.
			/// </summary>
			public bool IsRefresh { get { return this.isRefreshItem; } }

			public string Name { get { return this.name; } }

			public ComboListItem( bool isRefreshItem, string name )
			{
				this.isRefreshItem = isRefreshItem;
				this.name = name;
			}

			public override string ToString()
			{
				return Name;
			}
		}

		/// <summary>
		/// Item list for the catalog combobox.
		/// </summary>
		public class ComboList : IEnumerable<ComboListItem>
		{
			private readonly IList<ComboListItem> items = new List<ComboListItem>();

			public ComboList()
			{
			}

			public ComboList( IEnumerable<string> catalogs )
			{
				foreach ( var catalog in catalogs ) {
					if ( this.Contains( catalog ) )
						continue;

					var listItem = new ComboListItem( false, catalog );
					items.Add( listItem );
				}
				//items.Add( new CatalogListItem( true, "<Refresh>" ) );
			}

			public bool Contains( string catalog )
			{
				foreach ( var item in this.items ) {
					if ( item.IsRefresh )
						continue;

					string itemCatalog = item.Name;
					if ( itemCatalog.Equals( catalog, StringComparison.CurrentCultureIgnoreCase ) )
						return true;
				}
				return false;
			}


			#region IEnumerable Members

			public IEnumerator<ComboListItem> GetEnumerator()
			{
				return this.items.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}


		#endregion

	}
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Serialization;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Generic database base connection parameter.
	/// </summary>
	[DefaultProperty("DatasourceName")]
	[CLSCompliant( true )]
	[Serializable]
	public class DbConnectionParameter : INotifyPropertyChanged
	{
		#region Private fields

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		[NonSerialized]
		private readonly Authentication authentication = new Authentication();

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private ProviderType provider;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string datasoureComment;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string datasourceAddress;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private int datasourcePort;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string catalog;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private int timeout;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private string applicationName;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		[NonSerialized]
		private TraceSwitch traceSwitch;

		#endregion

		/// <summary>
		/// Default contructor.
		/// </summary>
		public DbConnectionParameter()
		{
			Provider = ProviderType.MsSql;
			DatasourceAddress = string.Empty;
			Catalog = string.Empty;
			UserId = string.Empty;
			Password = string.Empty;
			IntegratedSecurity = true;
			Timeout = 2;
			this.traceSwitch = new TraceSwitch( "Verbosity", "Verbosity", TraceLevel.Warning.ToString() );

			Type thisType = GetType();
			PropertyDescriptorCollection pdCollection = TypeDescriptor.GetProperties( thisType );
			PropertyDescriptor pdName = pdCollection["DatasourceComment"];
			TypeConverter nameConverter = pdName.Converter;
			StringLookupConverter nameLookupConverter = (StringLookupConverter)nameConverter;
			nameLookupConverter.GetLookupValues += new EventHandler<StringLookupGetValuesEventArgs>( DatasourceName_GetLookupValues );

			PropertyDescriptor pdDatasource = pdCollection["DatasourceAddress"];
			TypeConverter datasourceConverter = pdDatasource.Converter;
			StringLookupConverter datasourceLookupConverter = (StringLookupConverter)datasourceConverter;
			datasourceLookupConverter.GetLookupValues += new EventHandler<StringLookupGetValuesEventArgs>( DatasourceAddress_GetLookupValues );

			PropertyDescriptor pdCatalog = pdCollection["Catalog"];
			TypeConverter catalogConverter = pdCatalog.Converter;
			StringLookupConverter catalogLookupConverter = (StringLookupConverter)catalogConverter;
			catalogLookupConverter.GetLookupValues += new EventHandler<StringLookupGetValuesEventArgs>( Catalog_GetLookupValues );

			PropertyDescriptor pdUserId = pdCollection["UserId"];
			TypeConverter userIDConverter = pdUserId.Converter;
			StringLookupConverter userIDLookupConverter = (StringLookupConverter)userIDConverter;
			userIDLookupConverter.GetLookupValues += new EventHandler<StringLookupGetValuesEventArgs>( UserId_GetLookupValues );
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="that">The parameters to copy.</param>
		public DbConnectionParameter( DbConnectionParameter that )
		{
			Provider = that.Provider;
			DatasourceAddress = that.DatasourceAddress;
			DatasourcePort = that.DatasourcePort;
			DatasourceComment = that.DatasourceComment;
			Catalog = that.Catalog;
			UserId = that.UserId;
			Password = that.Password;
			IntegratedSecurity = that.IntegratedSecurity;
			Timeout = that.Timeout;
			ApplicationName = that.ApplicationName;
			this.traceSwitch = that.traceSwitch;
		}

		#region Events

		public event EventHandler<StringLookupGetValuesEventArgs> GetDatasourceNameValues;

		public event EventHandler<StringLookupGetValuesEventArgs> GetDatasourceAddressValues;

		public event EventHandler<StringLookupGetValuesEventArgs> GetCatalogValues;

		public event EventHandler<StringLookupGetValuesEventArgs> GetUserIdValues;

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Properties

		/// <summary>
		/// The provider to use.
		/// </summary>
		[Category( "Database" )]
		[DefaultValue( ProviderType.MsSql )]
		public ProviderType Provider 
		{ 
			get { return this.provider; }
			set
			{
				if ( value == this.provider )
					return;

				this.provider = value;
				RaisePropertyChanged( "Provider" );
			}
		}

		/// <summary>
		/// Add summary description
		/// </summary>
		[Category( "Database" )]
		[DisplayName( "Connection Name" )]
		[Description( "An abritary name for the connection. If you have to specify an IP address for some connection the name is useful to remember the connection parameters." )]
		[TypeConverter( typeof( StringLookupConverter ) )]
		public string DatasourceComment
		{
			get { return datasoureComment; }
			set
			{
				if ( !Object.Equals( datasoureComment, value ) ) {
					datasoureComment = value;
					RaisePropertyChanged( "DatasourceComment" );
				}
			}
		}


		/// <summary>
		/// The datasource / server.
		/// </summary>
		[Category( "Database" )]
		[DisplayName( "Server" )]
		[TypeConverter( typeof( StringLookupConverter ) )]
		public string DatasourceAddress
		{ 
			get { return this.datasourceAddress; }
			set
			{
				if ( value == null )
					value = string.Empty;

				if ( String.Equals( this.datasourceAddress, value ) )
					return;

				this.datasourceAddress = value;
				RaisePropertyChanged( "DatasourceAddress" );
			}
		}


		/// <summary>
		/// When using a tcp/ip connection this property defines
		/// the ip port to use e.g. 1433 for MS SQL Server or 1521
		/// for Oracle.
		/// </summary>
		[Category( "Database" )]
		[DisplayName( "Port" )]
		[Description( "When using a tcp/ip connection this property defines the ip port to use e.g. 1433 for MS SQL Server or 1521 for Oracle." )]
		[DefaultValue( 0 )]
		public int DatasourcePort
		{
			get { return this.datasourcePort; }
			set
			{
				if ( this.datasourcePort == value  ) 
					return;
				this.datasourcePort = value;
				RaisePropertyChanged( "DatasourcePort" );
			}
		}

		/// <summary>
		/// The initial catalog / database.
		/// </summary>
		[Category( "Database" )]
		[DisplayName( "Database" )]
		[TypeConverter(typeof( StringLookupConverter ))]
		public string Catalog
		{ 
			get { return this.catalog; }
			set
			{
				if ( value == null )
					value = string.Empty;

				if ( String.Equals( this.catalog, value ) )
					return;

				this.catalog = value;
				RaisePropertyChanged( "Catalog" );
			}
		}

		/// <summary>
		/// The user id if using server authentication for the login
		/// </summary>
		[Category( "Database" )]
		[DefaultValue( true )]
		public bool IntegratedSecurity
		{
			get { return this.authentication.Integrated; }
			set
			{
				if ( value == authentication.Integrated )
					return;

				authentication.Integrated = value;
				RaisePropertyChanged( "IntegratedSecurity" );
			}
		}


		/// <summary>
		/// The user id if using server authentication for the login
		/// </summary>
		[Category( "Database" )]
		[DisplayName( "Login Name" )]
		[TypeConverter( typeof( StringLookupConverter ) )]
		public string UserId 
		{
			get { return this.authentication.UserId; }
			set
			{
				if ( value == null )
					value = string.Empty;

				if ( string.Equals( this.authentication.UserId, value ) )
					return;

				authentication.UserId = value;
				RaisePropertyChanged( "UserId" );
			}
		}

		/// <summary>
		/// The password if using server authentication for the login
		/// </summary>
		[Category( "Database" )]
		[XmlIgnore]
		public string Password
		{
			get { return this.authentication.Password; }
			set
			{
				if ( value == null )
					value = string.Empty;

				if ( string.Equals( this.authentication.Password, value ) )
					return;

				authentication.Password = value;
				RaisePropertyChanged( "Password" );
			}
		}

		/// <summary>
		/// The timeout when trying to establish / open the connection.
		/// </summary>
		[Category( "Database" )]
		[DisplayName( "Connection Timeout" )]
		[DefaultValue( 2 )]
		public int Timeout
		{
			get { return this.timeout; }
			set
			{
				if ( this.timeout == value )
					return;
				this.timeout = value;
				RaisePropertyChanged( "Timeout" );
			}
		}

		/// <summary>
		/// The verbosity level of the traces.
		/// </summary>
		[Browsable( false )]
		[XmlIgnore]
		public string ApplicationName
		{
			get { return this.applicationName; }
			set { this.applicationName = value; }
		}

		/// <summary>
		/// The verbosity level of the traces.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public TraceLevel VerbosityLevel
		{
			get { return this.traceSwitch.Level; }
			set { this.traceSwitch.Level = value; }
		}

		internal TraceSwitch VerbositySwitch
		{
			get { return this.traceSwitch; }
		}


		#endregion

		#region Common Object Overrides

		/// <inheritdoc />
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( "Provider=" ).Append( Provider );

			if ( !String.IsNullOrEmpty( DatasourceAddress ) )
				sb.Append( ", Datasource=" ).Append( DatasourceAddress );

			if ( DatasourcePort > 0 )
				sb.Append( ":" ).Append( DatasourcePort );

			if ( !String.IsNullOrEmpty( DatasourceComment ) )
				sb.Append( " (" ).Append( DatasourceComment ).Append( ")" );

			if ( !String.IsNullOrEmpty( Catalog ) )
				sb.Append( ", Catalog=" ).Append( Catalog );

			if ( IntegratedSecurity )
				sb.Append( ", IntegratedSecurity=true" );

			if ( !String.IsNullOrEmpty( UserId ) )
				sb.Append( ", UserId=" ).Append( UserId );

			if ( !String.IsNullOrEmpty( ApplicationName ) )
				sb.Append( ", ApplicationName=" ).Append( ApplicationName );

			sb.Append( ", Timeout=" ).Append( Timeout );

			return sb.ToString();
		}


		/// <inheritdoc />
		public override bool Equals( object obj )
		{
			if ( obj == null || GetType() != obj.GetType() ) {
				return false;
			}

			var that = (DbConnectionParameter)obj;
			if ( this.Provider != that.Provider )
				return false;

			if ( !String.Equals( this.DatasourceAddress, that.DatasourceAddress ) )
				return false;

			if ( !String.Equals( this.DatasourceComment, that.DatasourceComment ) )
				return false;

			if ( this.DatasourcePort != that.DatasourcePort )
				return false;

			if ( !String.Equals( this.Catalog, that.Catalog ) )
				return false;

			if ( !Object.Equals( this.authentication, that.authentication ) )
				return false;

			if ( this.Timeout != that.Timeout )
				return false;

			return true;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked {
				int hashCode = Provider.GetHashCode();
				hashCode = hashCode * 31 + (DatasourceAddress == null ? 0 : DatasourceAddress.GetHashCode());
				hashCode = hashCode * 31 + DatasourcePort;
				hashCode = hashCode * 31 + (DatasourceComment == null ? 0 : DatasourceComment.GetHashCode());
				hashCode = hashCode * 31 + (Catalog == null ? 0 : Catalog.GetHashCode());
				hashCode = hashCode * 31 + (authentication == null ? 0 : authentication.GetHashCode());
				hashCode = hashCode * 31 + Timeout;
				return hashCode;
			}
		}


		#endregion

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		[SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This method raises an event." )]
		protected internal void RaisePropertyChanged( string propertyName )
		{
			if ( PropertyChanged != null ) {
				PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
			}
		}
		#endregion

		#region Event Handler to retrieve standard values for some fields.

		private void DatasourceName_GetLookupValues( object sender, StringLookupGetValuesEventArgs args )
		{
			if ( !Object.Equals( args.Context.Instance, this ) )
				return;

			if ( this.GetDatasourceNameValues != null ) {
				GetDatasourceNameValues( this, args );
			}
		}

		private void DatasourceAddress_GetLookupValues( object sender, StringLookupGetValuesEventArgs args )
		{
			if ( !Object.Equals( args.Context.Instance, this ) )
				return;

			if ( this.GetDatasourceAddressValues != null ) {
				GetDatasourceAddressValues( this, args );
			}
		}

		private void Catalog_GetLookupValues( object sender, StringLookupGetValuesEventArgs args )
		{
			if ( !Object.Equals( args.Context.Instance, this ) )
				return;

			if ( this.GetCatalogValues != null ) {
				GetCatalogValues( this, args );
			}
		}

		private void UserId_GetLookupValues( object sender, StringLookupGetValuesEventArgs args )
		{
			if ( !Object.Equals( args.Context.Instance, this ) )
				return;

			if ( this.GetUserIdValues != null ) {
				GetUserIdValues( this, args );
			}
		}

		#endregion
	}
}

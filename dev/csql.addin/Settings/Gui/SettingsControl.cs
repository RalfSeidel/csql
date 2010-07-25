using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Sqt.DbcProvider;
using EnvDTE;
using System.Diagnostics.CodeAnalysis;

namespace csql.addin.Settings.Gui
{
	public partial class SettingsControl : UserControl
	{
		private _DTE application;
		private MruConnections mruConnections;
		private DbConnectionParameter dbConnectionParameter;
		private CSqlParameter csqlParameter;
		private bool dbConnectionParameterChanged;
		private bool csqlParameterChanged;
		private bool reloadSettings;
		private Timer saveChangesTimer;
		private Timer dbConnectionParameterChangedTimer;
		private bool ignoreParameterPropertyChanges;

		public SettingsControl()
		{
			InitializeComponent();
		}

		internal void SetApplication( _DTE application )
		{
			this.application = application;
			SettingsManager settingsManager = SettingsManager.GetInstance( application );
			settingsManager.SettingsReloaded += new SettingsReloadedDelegate( SettingsManager_SettingsReloaded );
			ReloadSettings();
		}

		private static void SelectMruDatasource( DbConnectionParameter parameter, MruConnections mruConnection )
		{
			if ( mruConnection == null )
				return;

			var datasources = mruConnection.FindDatasources( parameter.Provider );
			if ( datasources == null || datasources.Datasource == null || datasources.Datasource.Count == 0 )
				return;

			var datasource = datasources.Datasource[0];
			parameter.DatasourceComment = datasource.Comment;
			parameter.DatasourceAddress = datasource.Address;
			parameter.DatasourcePort = datasource.Port;
			SelectMruDatasourceParameter( parameter, datasource );
		}

		private static void SelectMruDatasourceParameterByComment( DbConnectionParameter parameter, MruConnections mruConnection )
		{
			if ( mruConnection == null )
				return;

			var datasource = mruConnection.FindDatasourceByComment( parameter.Provider, parameter.DatasourceComment );
			if ( datasource == null )
				return;

			parameter.DatasourceAddress = datasource.Address;
			parameter.DatasourcePort = datasource.Port;
			SelectMruDatasourceParameter( parameter, datasource );

		}

		private static void SelectMruDatasourceParameterByAddress( DbConnectionParameter parameter, MruConnections mruConnection )
		{
			if ( mruConnection == null )
				return;

			var datasource = mruConnection.FindDatasourceByAddress( parameter.Provider, parameter.DatasourceAddress, parameter.DatasourcePort );
			if ( datasource == null )
				return;

			parameter.DatasourceComment = datasource.Comment;
			parameter.DatasourceAddress = datasource.Address;
			parameter.DatasourcePort = datasource.Port;
			SelectMruDatasourceParameter( parameter, datasource );
		}


		private static void SelectMruDatasourceParameter( DbConnectionParameter parameter, Datasource datasource )
		{
			if ( datasource.Catalogs != null && datasource.Catalogs.Count > 0 ) {
				string catalog = datasource.Catalogs[0];
				parameter.Catalog = catalog;
			}
			if ( datasource.Authentications != null && datasource.Authentications.Count > 0 ) {
				Authentication authentication = datasource.Authentications[0];
				parameter.IntegratedSecurity = authentication.Integrated;
				parameter.UserId = authentication.UserId;
				parameter.Password = authentication.Password;
			}
		}


		private Datasources FindCurrentDatasources()
		{
			if ( this.mruConnections == null )
				return null;

			Datasources datasources = mruConnections.FindDatasources( dbConnectionParameter.Provider );
			return datasources;
		}

		private Datasource FindCurrentDatasource()
		{
			if ( this.mruConnections == null )
				return null;

			Datasource datasource = mruConnections.FindDatasourceByAddress( dbConnectionParameter.Provider, dbConnectionParameter.DatasourceAddress, dbConnectionParameter.DatasourcePort );
			return datasource;
		}

		private void ReloadSettings()
		{
			SettingsManager settingsManager = SettingsManager.GetInstance( application );
			this.mruConnections = settingsManager.MruDbConnectionParameters;
			this.dbConnectionParameter = settingsManager.CurrentDbConnectionParameter;
			this.csqlParameter = settingsManager.CurrentScriptParameter;
			InitObjectSelectionComboBox();
			this.reloadSettings = false;

			if ( dbConnectionParameter != null ) {
				dbConnectionParameter.GetDatasourceNameValues += new EventHandler<StringLookupGetValuesEventArgs>( DbConnectionParameter_GetDatasourceNameValues );
				dbConnectionParameter.GetDatasourceAddressValues += new EventHandler<StringLookupGetValuesEventArgs>( DbConnectionParameter_GetDatasourceAddressValues );
				dbConnectionParameter.GetCatalogValues += new EventHandler<StringLookupGetValuesEventArgs>( DbConnectionParameter_GetCatalogValues );
				dbConnectionParameter.GetUserIdValues += new EventHandler<StringLookupGetValuesEventArgs>( DbConnectionParameter_GetUserIdValues );
				dbConnectionParameter.PropertyChanged += new PropertyChangedEventHandler( DbConnectionParameter_PropertyChanged );
			}
		}


		#region Event Handler

		private void SettingsManager_SettingsReloaded( object sender, System.EventArgs eventArgs )
		{
			if ( this.Visible ) {
				ReloadSettings();
			}
			else {
				this.editorObjects.DataSource = null;
				this.propertyGrid.SelectedObject = null;
				this.reloadSettings = true;
			}
		}

		private void DbConnectionParameter_GetDatasourceNameValues( object sender, StringLookupGetValuesEventArgs args )
		{
			if ( this.mruConnections == null )
				return;

			Datasources datasources = mruConnections.FindDatasources( dbConnectionParameter.Provider );
			if ( datasources == null )
				return;

			foreach ( var ds in datasources.Datasource ) {
				args.Values.Add( ds.Comment );
			}

		}

		private void DbConnectionParameter_GetDatasourceAddressValues( object sender, StringLookupGetValuesEventArgs args )
		{
			Datasources datasources = FindCurrentDatasources();
			if ( datasources == null )
				return;

			foreach ( var ds in datasources.Datasource ) {
				args.Values.Add( ds.Address );
			}
		}

		private void DbConnectionParameter_GetCatalogValues( object sender, StringLookupGetValuesEventArgs args )
		{
			Datasource datasource = FindCurrentDatasource();
			if ( datasource == null )
				return;

			foreach ( var catalog in datasource.Catalogs ) {
				args.Values.Add( catalog );
			}
		}

		private void DbConnectionParameter_GetUserIdValues( object sender, StringLookupGetValuesEventArgs args )
		{
			Datasource datasource = FindCurrentDatasource();
			if ( datasource == null )
				return;

			foreach ( var authentication in datasource.Authentications ) {
				args.Values.Add( authentication.UserId ?? "" );
			}
		}

		/// <summary>
		/// Start a time that will defer the updates a bit deferred.
		/// </summary>
		[SuppressMessage( "Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "The timer is stoped after it eplapsed for the first time." )]
		private void DbConnectionParameter_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if ( this.ignoreParameterPropertyChanges )
				return;

			if ( this.dbConnectionParameterChangedTimer == null ) {
				dbConnectionParameterChangedTimer = new Timer();
			}
			else {
				dbConnectionParameterChangedTimer.Stop();
			}
			dbConnectionParameterChangedTimer.Interval = 100;
			dbConnectionParameterChangedTimer.Tag = e.PropertyName;
			dbConnectionParameterChangedTimer.Tick += new EventHandler( DbConnectionParameter_PropertyChangedTimer_Tick );
			dbConnectionParameterChangedTimer.Start();
		}

		private void DbConnectionParameter_PropertyChangedTimer_Tick( object sender, EventArgs e )
		{
			Timer timer = (Timer)sender;
			string propertyName = (string)timer.Tag;
			timer.Stop();

			this.ignoreParameterPropertyChanges = true;

			try {
				DbConnectionParameter parameter = this.dbConnectionParameter;
				switch ( propertyName ) {
					case "Provider":
						SelectMruDatasource( parameter, mruConnections );
						break;
					case "DatasourceComment":
						SelectMruDatasourceParameterByComment( parameter, mruConnections );
						break;
					case "DatasourceAddress":
						SelectMruDatasourceParameterByAddress( parameter, mruConnections );
						break;
					case "DatasourcePort":
						SelectMruDatasourceParameterByAddress( parameter, mruConnections );
						break;
				}
				this.propertyGrid.Refresh();
			}
			finally {
				this.ignoreParameterPropertyChanges = false;
			}
		}


		private void SettingsControl_VisibleChanged( object sender, System.EventArgs e )
		{
			if ( this.reloadSettings ) {
				this.reloadSettings = false;
			}
		}

		private void EditorObjects_SelectionChangeCommitted( object sender, System.EventArgs e )
		{
			var combo = (ComboBox)sender;
			var selection = (SelectedObjectComboBoxItem)combo.SelectedItem;
			this.propertyGrid.SelectedObject = selection.EditorObject;
		}

		/// <summary>
		/// Starts a timer that will save the changes a short while after the user made the modification.
		/// </summary>
		private void PropertyGrid_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			if ( this.saveChangesTimer != null ) {
				this.saveChangesTimer.Stop();
			}
			else {
				this.saveChangesTimer = new Timer();
			}

			if ( this.propertyGrid.SelectedObject == this.dbConnectionParameter ) {
				dbConnectionParameterChanged = true;
			}
			if ( this.propertyGrid.SelectedObject == this.csqlParameter ) {
				csqlParameterChanged = true;
			}


			this.saveChangesTimer.Interval = 1000;
			this.saveChangesTimer.Tick += new EventHandler( SaveChangesTimer_Tick );
			this.saveChangesTimer.Start();
		}

		private void SaveChangesTimer_Tick( object sender, System.EventArgs e )
		{
			if ( this.saveChangesTimer != null ) {
				this.saveChangesTimer.Stop();
				this.saveChangesTimer.Dispose();
				this.saveChangesTimer = null;
			}
			if ( dbConnectionParameterChanged ) {
				if ( false && this.application != null ) {
					SettingsManager settingsManager = SettingsManager.GetInstance( this.application );
					settingsManager.SaveDbConnectionParameter( dbConnectionParameter );
				}
				dbConnectionParameterChanged = false;
			}

			if ( this.csqlParameterChanged ) {
				if ( this.application != null ) {
					SettingsManager settingsManager = SettingsManager.GetInstance( this.application );
					settingsManager.SaveScriptParameter( csqlParameter );
				}
				csqlParameterChanged = false;
			}
		}

		#endregion

		private void InitObjectSelectionComboBox()
		{
			var comboItems = CreateComboItems( dbConnectionParameter, csqlParameter );
			this.editorObjects.DataSource = comboItems;

			if ( IsPropertyGridObjectValid( comboItems ) )
				return;

			if ( comboItems.Count > 0 ) {
				this.editorObjects.SelectedIndex = 0;
				this.propertyGrid.SelectedObject = comboItems[0].EditorObject;
			}
			else {
				this.propertyGrid.SelectedObject = null;
				this.editorObjects.SelectedIndex = 1;
			}
		}

		private bool IsPropertyGridObjectValid( IEnumerable<SelectedObjectComboBoxItem> comboItems )
		{
			foreach ( var item in comboItems ) {
				if ( propertyGrid.SelectedObject == item.EditorObject )
					return true;
			}
			return false;
		}

		private IList<SelectedObjectComboBoxItem> CreateComboItems( DbConnectionParameter dbConnectionParameter, CSqlParameter csqlParameter )
		{
			var result = new List<SelectedObjectComboBoxItem>();

			if ( dbConnectionParameter != null ) {
				SelectedObjectComboBoxItem connectionItem = new SelectedObjectComboBoxItem( "Database Parameter", dbConnectionParameter );
				result.Add( connectionItem );
			}
			if ( csqlParameter != null ) {
				SelectedObjectComboBoxItem parameterItem = new SelectedObjectComboBoxItem( "Script Parameter " + csqlParameter.Name, csqlParameter );
				result.Add( parameterItem );
			}

			return result;
		}

		internal class SelectedObjectComboBoxItem
		{
			public SelectedObjectComboBoxItem( string name, object editorObject )
			{
				this.Name = name;
				this.EditorObject = editorObject;
			}

			public string Name { get; private set; }
			public object EditorObject { get; private set; }

			public override string ToString()
			{
				return Name;
			}
		}
	}
}

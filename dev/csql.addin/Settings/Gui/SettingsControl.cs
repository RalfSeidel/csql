using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using EnvDTE;
using Sqt.DbcProvider;
using System.Globalization;

namespace csql.addin.Settings.Gui
{
	public partial class SettingsControl : UserControl
	{
		private _DTE application;
		private MruConnections mruConnections;
		private BindingList<SelectedObjectComboBoxItem> editorObjectComboItems;
		private ScriptParameterCollection scriptParameters;
		private DbConnectionParameter dbConnectionParameter;
		//private ScriptParameter currentScriptParameter;
		private bool hasConnectionParameterChanged;
		private bool hasScriptParameterChanged;
		private bool reloadSettings;
		private Timer saveChangesTimer;
		private Timer dbConnectionParameterChangedTimer;
		private bool ignoreParameterPropertyChanges;

		public SettingsControl()
		{
			InitializeComponent();
			this.saveChangesButton.Image = Resources.Save;
			this.copyScriptParameterButton.Image = Resources.CreateCopy;
			this.deleteScriptParameterButton.Image = Resources.Delete;
			UpdateCommandState();
		}

		/// <summary>
		/// Gets or sets a value indicating whether the database connection parameter where changed.
		/// </summary>
		private bool HasConnectionParameterChanged
		{
			get { return this.hasConnectionParameterChanged; }
			set
			{
				this.hasConnectionParameterChanged = value;
				UpdateCommandState();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the script parameter where changed.
		/// </summary>
		private bool HasScriptParameterChanged
		{
			get { return this.hasScriptParameterChanged; }
			set
			{
				this.hasScriptParameterChanged = value;
				UpdateCommandState();
			}
		}


		internal void SetApplication( _DTE application )
		{
			this.application = application;
			SettingsManager settingsManager = SettingsManager.GetInstance( application );
			settingsManager.SettingsReloaded += new EventHandler( SettingsManager_SettingsReloaded );
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

			var provider = parameter.Provider;
			var datasource = mruConnection.FindDatasourceByComment( ref provider, parameter.DatasourceComment );
			if ( datasource == null )
				return;

			parameter.Provider = provider;
			parameter.DatasourceComment = datasource.Comment;
			parameter.DatasourceAddress = datasource.Address;
			parameter.DatasourcePort = datasource.Port;
			SelectMruDatasourceParameter( parameter, datasource );
		}

		private static void SelectMruDatasourceParameterByAddress( DbConnectionParameter parameter, MruConnections mruConnection )
		{
			if ( mruConnection == null )
				return;

			var provider = parameter.Provider;
			var datasource = mruConnection.FindDatasourceByAddress( provider, parameter.DatasourceAddress, parameter.DatasourcePort );
			if ( datasource == null )
				return;

			parameter.Provider = provider;
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
			this.scriptParameters = settingsManager.ScriptParameters;
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
			} else {
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

			args.Values.Clear();
			foreach ( var ds in datasources.Datasource ) {
				args.Values.Add( ds.Comment );
			}

		}

		private void DbConnectionParameter_GetDatasourceAddressValues( object sender, StringLookupGetValuesEventArgs args )
		{
			Datasources datasources = FindCurrentDatasources();
			if ( datasources == null )
				return;

			args.Values.Clear();
			foreach ( var ds in datasources.Datasource ) {
				args.Values.Add( ds.Address );
			}
		}

		private void DbConnectionParameter_GetCatalogValues( object sender, StringLookupGetValuesEventArgs args )
		{
			Datasource datasource = FindCurrentDatasource();
			if ( datasource == null )
				return;

			args.Values.Clear();
			foreach ( var catalog in datasource.Catalogs ) {
				args.Values.Add( catalog );
			}
		}

		private void DbConnectionParameter_GetUserIdValues( object sender, StringLookupGetValuesEventArgs args )
		{
			Datasource datasource = FindCurrentDatasource();
			if ( datasource == null )
				return;

			args.Values.Clear();
			foreach ( var authentication in datasource.Authentications ) {
				args.Values.Add( authentication.UserId ?? "" );
			}
		}

		/// <summary>
		/// Start a time that will defer the updates a bit deferred.
		/// </summary>
		[SuppressMessage( "Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification="The timer is stoped after it eplapsed for the first time." )]
		private void DbConnectionParameter_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if ( this.ignoreParameterPropertyChanges )
				return;

			if ( this.dbConnectionParameterChangedTimer == null ) {
				dbConnectionParameterChangedTimer = new Timer();
			} else {
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

			var scriptParameter = selection.EditorObject as ScriptParameter;
			if ( scriptParameter != null ) {
				if ( scriptParameter != this.scriptParameters.Current ) {
					this.scriptParameters.Current = scriptParameter;
					HasScriptParameterChanged = true;
				}
			}

			UpdateCurrentScriptParameterFlag();
			UpdateCommandState();
		}

		private void UpdateCurrentScriptParameterFlag()
		{
			// Buffer the items to update in a new collection. If modified in the first loop you
			// would get a "collection changed" exception.
			var updateItems = new List<SelectedScriptParameterComboBoxItem>();
			foreach ( var item in this.editorObjects.Items ) {
				SelectedScriptParameterComboBoxItem parameterItem = item as SelectedScriptParameterComboBoxItem;
				if ( parameterItem != null ) {
					bool isCurrent = parameterItem.EditorObject.Equals( this.scriptParameters.Current );
					if ( isCurrent != parameterItem.IsCurrent ) {
						updateItems.Add( parameterItem );
					}
				}
			}
			foreach ( var parameterItem in updateItems ) {
				bool isCurrent = parameterItem.EditorObject.Equals( this.scriptParameters.Current );
				parameterItem.IsCurrent = isCurrent;
			}
		}


		/// <summary>
		/// Starts a timer that will save the changes a short while after the user made the modification.
		/// </summary>
		[SuppressMessage( "Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification="Timer is stoped when it ellapsed for the first time." )]
		private void PropertyGrid_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			if ( this.saveChangesTimer != null ) {
				this.saveChangesTimer.Stop();
			} else {
				this.saveChangesTimer = new Timer();
			}

			if ( this.propertyGrid.SelectedObject == this.dbConnectionParameter ) {
				this.HasConnectionParameterChanged = true;
			}
			if ( this.propertyGrid.SelectedObject == this.scriptParameters.Current ) {
				this.HasScriptParameterChanged = true;
			}

			this.saveChangesTimer.Interval = 500;
			this.saveChangesTimer.Tick += new EventHandler( SaveChangesTimer_Tick );
			this.saveChangesTimer.Start();
		}

		/// <summary>
		/// Save changes automaticly after any setting has changes.
		/// </summary>
		private void SaveChangesTimer_Tick( object sender, System.EventArgs e )
		{
			if ( this.saveChangesTimer != null ) {
				this.saveChangesTimer.Stop();
				this.saveChangesTimer.Dispose();
				this.saveChangesTimer = null;
			}

			SaveChangesAutomaticly();
		}

		/// <summary>
		/// Save changes explicitly when the user pressed the save button.
		/// </summary>
		private void SaveChanges_Click( object sender, EventArgs e )
		{
			SaveChangesAutomaticly();

			if ( HasConnectionParameterChanged ) {
				if ( this.application != null ) {
					SettingsManager settingsManager = SettingsManager.GetInstance( this.application );
					settingsManager.SaveDbConnectionParameterInMruHistory( dbConnectionParameter );
					settingsManager.SaveDbConnectionParameterInGlobals( dbConnectionParameter );
				}
				this.HasConnectionParameterChanged = false;
			}
		}

		private void SaveChangesAutomaticly()
		{
			if ( HasConnectionParameterChanged ) {
				if ( this.application != null ) {
					SettingsManager settingsManager = SettingsManager.GetInstance( this.application );
					settingsManager.SaveDbConnectionParameterInGlobals( dbConnectionParameter );
				}
			}

			if ( this.HasScriptParameterChanged ) {
				if ( this.application != null ) {
					SettingsManager settingsManager = SettingsManager.GetInstance( this.application );
					settingsManager.SaveScriptParameterInSolutionSettings( this.scriptParameters );

				}
				HasScriptParameterChanged = false;
			}
		}

		private void CopyScriptParameter_Click( object sender, EventArgs e )
		{
			var selection = (SelectedObjectComboBoxItem)this.editorObjects.SelectedItem;
			var currentScriptParameter = selection.EditorObject as ScriptParameter;
			if ( currentScriptParameter == null )
				return;

			var copiedScriptParameter = new ScriptParameter( currentScriptParameter );
			copiedScriptParameter.Name = GetScriptParameterName( currentScriptParameter );
			this.scriptParameters.Add( copiedScriptParameter );
			this.scriptParameters.Current = copiedScriptParameter;
			
			var comboBoxItem = new SelectedScriptParameterComboBoxItem( copiedScriptParameter );
			this.editorObjectComboItems.Add( comboBoxItem );
			this.editorObjects.SelectedItem = comboBoxItem;
			this.propertyGrid.SelectedObject = copiedScriptParameter;
			this.HasScriptParameterChanged = true;
			UpdateCurrentScriptParameterFlag();
			UpdateCommandState();
		}

		private void DeleteScriptParameter_Click( object sender, EventArgs e )
		{
			var selection = (SelectedObjectComboBoxItem)this.editorObjects.SelectedItem;
			var currentScriptParameter = selection.EditorObject as ScriptParameter;
			if ( currentScriptParameter == null )
				return;

			this.scriptParameters.Remove( currentScriptParameter );
			this.editorObjectComboItems.Remove( selection );

			var newSelection = (SelectedObjectComboBoxItem)this.editorObjects.SelectedItem;
			var newEditorObject = newSelection.EditorObject;
			this.propertyGrid.SelectedObject = newEditorObject;

			currentScriptParameter = newEditorObject as ScriptParameter;
			if ( currentScriptParameter != null )
				this.scriptParameters.Current = currentScriptParameter;

			this.HasScriptParameterChanged = true;
			UpdateCurrentScriptParameterFlag();
			UpdateCommandState();
		}

		#endregion

		private void UpdateCommandState()
		{
			this.saveChangesButton.Enabled = this.HasScriptParameterChanged || this.HasConnectionParameterChanged;

			if ( this.editorObjects.SelectedIndex < 0 ) {
				this.deleteScriptParameterButton.Enabled = false;
				this.copyScriptParameterButton.Enabled = false;
				return;
			}
			var selection = (SelectedObjectComboBoxItem)this.editorObjects.SelectedItem;
			var scriptParameter = selection.EditorObject as ScriptParameter;
			this.copyScriptParameterButton.Enabled = scriptParameter != null;
			this.deleteScriptParameterButton.Enabled = scriptParameter != null && this.scriptParameters.Count > 1;
		}

		private void InitObjectSelectionComboBox()
		{
			this.editorObjectComboItems = CreateComboItems( dbConnectionParameter, scriptParameters );
			this.editorObjects.DataSource = editorObjectComboItems;
			this.editorObjects.DisplayMember = "Name";

			if ( IsPropertyGridObjectValid( editorObjectComboItems ) )
				return;

			if ( editorObjectComboItems.Count > 0 ) {
				this.editorObjects.SelectedIndex = 0;
				this.propertyGrid.SelectedObject = editorObjectComboItems[0].EditorObject;
			} else {
				this.editorObjects.SelectedIndex = -1;
				this.propertyGrid.SelectedObject = null;
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

		private string GetScriptParameterName( ScriptParameter parameterCopied )
		{
			string baseName = parameterCopied.Name;
			string copyTextBase = " - Copy";
			int copyTextIndex = baseName.IndexOf( copyTextBase, StringComparison.CurrentCultureIgnoreCase );
			if ( copyTextIndex > 0 ) {
				baseName = baseName.Substring( 0, copyTextIndex );
			}
			string name = baseName + copyTextBase;
			if ( !ScriptParameterNameExists( name ) )
				return name;

			for ( int i = 2; i < 100; ++i ) {
				name = baseName + copyTextBase + " " + i.ToString( CultureInfo.InvariantCulture );
				if ( !ScriptParameterNameExists( name ) )
					return name;
			}
			return name;
		}

		private bool ScriptParameterNameExists( string name )
		{
			foreach ( var parameter in this.scriptParameters ) {
				if ( string.Compare( parameter.Name, name, StringComparison.CurrentCultureIgnoreCase ) == 0 )
					return true;
			}
			return false;
		}


		private static BindingList<SelectedObjectComboBoxItem> CreateComboItems( DbConnectionParameter dbConnectionParameter, ScriptParameterCollection scriptParameters )
		{
			var result = new BindingList<SelectedObjectComboBoxItem>();

			if ( dbConnectionParameter != null ) {
				var connectionItem = new DbConnectionComboBoxItem( dbConnectionParameter );
				result.Add( connectionItem );
			}
			if ( scriptParameters != null ) {
				foreach ( var scriptParameter in scriptParameters ) {
					var parameterItem = new SelectedScriptParameterComboBoxItem( scriptParameter );
					parameterItem.IsCurrent = scriptParameter.Equals( scriptParameters.Current );
					result.Add( parameterItem );
				}
			}

			return result;
		}

		internal abstract class SelectedObjectComboBoxItem : INotifyPropertyChanged
		{
			protected SelectedObjectComboBoxItem( object editorObject )
			{
				this.EditorObject = editorObject;
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public abstract string Name { get; }

			public object EditorObject { get; private set; }

			public override string ToString()
			{
				return Name;
			}

			/// <summary>
			/// Raises the property changed event.
			/// </summary>
			[SuppressMessage( "Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification="The method is a wrapper to raise the event." )]
			protected void RaisePropertyChanged( string propertyName )
			{
				if ( PropertyChanged != null ) {
					PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
				}
			}
		}

		internal class DbConnectionComboBoxItem : SelectedObjectComboBoxItem
		{
			internal DbConnectionComboBoxItem( DbConnectionParameter connectionParameter )
				: base( connectionParameter )
			{
			}

			public override string Name
			{
				get { return "Database Parameter"; }
			}
		}

		internal class SelectedScriptParameterComboBoxItem : SelectedObjectComboBoxItem
		{
			private bool isCurrent;

			internal SelectedScriptParameterComboBoxItem( ScriptParameter scriptParameter )
				: base( scriptParameter )
			{
				scriptParameter.PropertyChanged+=ScriptParameter_PropertyChanged;
			}

			public override string Name
			{
				get 
				{
					ScriptParameter parameter = (ScriptParameter)EditorObject;
					return "Script Parameter " + parameter.Name + (this.isCurrent ? " *" : string.Empty); 
				}
			}

			internal bool IsCurrent 
			{
				get { return this.isCurrent; }
				set
				{
					if ( value == this.isCurrent )
						return;

					this.isCurrent = value;
					RaisePropertyChanged( "IsCurrent" );
					RaisePropertyChanged( "Name" );
				}
			}



			private void ScriptParameter_PropertyChanged( object sender, PropertyChangedEventArgs e )
			{
				if ( e.PropertyName == "Name" ) {
					RaisePropertyChanged( "Name" );
				}
			}
		}

	}
}

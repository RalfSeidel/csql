using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using csql.addin.Settings;

namespace csql.addin.Gui.Views
{
	public class SettingsPanelViewModel : Bases.ViewModel
	{
		private readonly ISettingsPersistensProvider settingsPersistensProvider;
		private readonly SettingsCollection settings;
		ObservableCollection<SettingsItemViewModel> itemsSource;


		public ICollectionView ItemsSourceView { get; set; }
		public ICommand NewEntryCommand { get; private set; }
		public ICommand DeleteEntryCommand { get; private set; }
		public ICommand CopyEntryCommand { get; private set; }

		public SettingsPanelViewModel( ISettingsPersistensProvider settingsPersistensProvider )
		{
			this.settingsPersistensProvider = settingsPersistensProvider;
			this.settings = LoadSettings();

			itemsSource = new ObservableCollection<SettingsItemViewModel>();
			ItemsSourceView = CollectionViewSource.GetDefaultView( itemsSource );



			foreach ( csql.addin.Settings.SettingsItem item in settings.SettingsObjects ) {
				SettingsItemViewModel settinbsObjectViewModel = new SettingsItemViewModel( item );
				itemsSource.Add( settinbsObjectViewModel );
				settinbsObjectViewModel.SaveChanges += new SettingsItemViewModel.SaveChangesEventHanlder( settinbsObjectViewModel_SaveChanges );
			}

			//Das erste Element selektieren
			ItemsSourceView.MoveCurrentToFirst();

			NewEntryCommand = new SimpleCommand
			{
				CanExecuteDelegate = x => true,
				ExecuteDelegate = x => this.InsertEntry( new csql.addin.Settings.SettingsItem() )
			};

			CopyEntryCommand = new SimpleCommand
			{
				CanExecuteDelegate = x => (this.ItemsSourceView.CurrentItem != null),
				ExecuteDelegate = x => this.InsertEntry( new csql.addin.Settings.SettingsItem( (this.ItemsSourceView.CurrentItem as SettingsItemViewModel).SettingsObject ) )
			};

			DeleteEntryCommand = new SimpleCommand
			{
				CanExecuteDelegate = x => (this.ItemsSourceView.CurrentItem != null),
				ExecuteDelegate = x => this.DeleteEntry( this.ItemsSourceView.CurrentItem as SettingsItemViewModel )
			};

		}

		void settinbsObjectViewModel_SaveChanges()
		{
			this.SaveSettings();
		}

		void DeleteEntry( SettingsItemViewModel settingsObjectViewModel )
		{
			this.itemsSource.Remove( settingsObjectViewModel );
			this.settings.SettingsObjects.Remove( settingsObjectViewModel.SettingsObject );

			SaveSettings();
		}

		void InsertEntry( csql.addin.Settings.SettingsItem settingsObject )
		{
			SettingsItemViewModel settingsObjectViewModel = new SettingsItemViewModel( settingsObject );
			settingsObjectViewModel.SaveChanges += new SettingsItemViewModel.SaveChangesEventHanlder( settinbsObjectViewModel_SaveChanges );

			this.settings.SettingsObjects.Add( settingsObject );
			this.itemsSource.Add( settingsObjectViewModel );
			this.ItemsSourceView.MoveCurrentTo( settingsObjectViewModel );

			SaveSettings();
		}

		private SettingsCollection LoadSettings()
		{
			SettingsCollection settings = null;
			if ( this.settingsPersistensProvider.CanLoadSettings ) {
				try {
					Trace.TraceInformation( "Loading settings from \"" + this.settingsPersistensProvider.Url + "\"" );
					settings = this.settingsPersistensProvider.LoadSettings();
				}
				catch ( IOException e ) {
					Trace.TraceError( "Error loading settings: " + e.Message );
					Globals.Messages.ShowError( "Error loading settings", e.Message );
				}
			}

			if ( settings == null ) {
				settings = new SettingsCollection();
			}
			return settings;
		}

		private void SaveSettings()
		{
			try {
				Trace.TraceInformation( "Saving current setting in \"" + this.settingsPersistensProvider.Url + "\"" );
				this.settingsPersistensProvider.SaveSettings( this.settings );
			}
			catch ( IOException e ) {
				Trace.TraceError( "Error saving settings: " + e.Message );
				Globals.Messages.ShowError( "Error saving settings", e.Message );
			}
		}
	}
}

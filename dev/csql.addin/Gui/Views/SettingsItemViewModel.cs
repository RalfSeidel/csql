using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using csql.addin.Settings;

namespace csql.addin.Gui.Views
{
    public class SettingsItemViewModel : Bases.ViewModel
    {
		csql.addin.Settings.SettingsItem settingsObject;
		public csql.addin.Settings.SettingsItem SettingsObject { get { return this.settingsObject; } }
        
        public Bases.ViewModelWrapper<string> Description { get; private set; }
        public Bases.ViewModelWrapper<int> VerbosityLevel { get; private set; }

        public Bases.ViewModelWrapper<string> DbUsername { get; private set; }
        public Bases.ViewModelWrapper<string> DbPassword { get; private set; }
        public Bases.ViewModelWrapper<string> DbServer { get; private set; }
        public Bases.ViewModelWrapper<string> DbDatabase { get; private set; }
        public Bases.ViewModelWrapper<bool> DbUseIs { get; private set; }

        
        ObservableCollection<Bases.ViewModelWrapper<string>> includeFoldersItemsSource { get; set; }
        public ICollectionView IncludeFoldersItemsSourceView { get; set; }

        ObservableCollection<PreprocessorDefinitionViewModel> preprocessorDefinitionsItemsSource { get; set; }
        public ICollectionView PreprocessorDefinitionsItemsSourceView { get; set; }

        public Bases.ViewModelWrapper<string> PreprocessorAdvancedDefinitions { get; private set; }

        public Bases.ViewModelWrapper<string> Distributionfile { get; private set; }
        public Bases.ViewModelWrapper<bool> IsDistributionfileEnabled { get; private set; }
        public Bases.ViewModelWrapper<bool> IsPreprocessorEnabled { get; private set; }
        public Bases.ViewModelWrapper<bool> IsBreakOnErrosEnabled { get; private set; }
        public Bases.ViewModelWrapper<bool> IsTemporaryOutputFileEnabled { get; private set; }
        public Bases.ViewModelWrapper<string> TemporaryOutputFile { get; private set; }


        public bool HasChanges
        {
            get
            {
                if (
                        Description.HasChanges ||
                        DbUsername.HasChanges ||
                        DbPassword.HasChanges ||
                        DbServer.HasChanges ||
                        DbDatabase.HasChanges ||
                        DbUseIs.HasChanges ||
                        Distributionfile.HasChanges ||
                        IsDistributionfileEnabled.HasChanges ||
                        IsPreprocessorEnabled.HasChanges ||
                        PreprocessorAdvancedDefinitions.HasChanges ||
                        VerbosityLevel.HasChanges ||
                        TemporaryOutputFile.HasChanges ||
                        IsTemporaryOutputFileEnabled.HasChanges
                        ) 
                    return true;

                //Auflistungen auf Veränderungen überprüfen
                foreach (Bases.ViewModelWrapper<string> item in this.includeFoldersItemsSource)
                    if (item.HasChanges) return true;

                if (this.includeFoldersItemsSource.Count != settingsObject.IncludeFolders.Count)
                    return true;

                foreach (PreprocessorDefinitionViewModel item in this.preprocessorDefinitionsItemsSource)
                    if (item.HasChanges) return true;

                if (this.preprocessorDefinitionsItemsSource.Count != settingsObject.PreprocessorDefinitions.Count)
                    return true;


                return false;
            }
        }


        public ICommand SaveChangesCommand { get; private set; }
        public ICommand ResetChangesCommand { get; private set; }

        public ICommand DatabaseConnectionTestCommand { get; private set; }

        public ICommand RemovePreprocessorDefinitionCommand { get; private set; }
        public ICommand AddPreprocessorDefinitionCommand { get; private set; }

        public ICommand RemoveIncludeFolderCommand { get; private set; }
        public ICommand AddIncludeFolderCommand { get; private set; }

        public ICommand ChooseDistributionfileCommand { get; private set; }
        public ICommand ChooseTemporaryOutputFileCommand { get; private set; }
        public ICommand ChooseIncludeFolderCommand { get; private set; }

        public ICommand ExecuteAllCommand { get; private set; }
        public ICommand ExecuteSelectionCommand { get; private set; }


		public SettingsItemViewModel( csql.addin.Settings.SettingsItem settingsObject )
        {
            this.settingsObject = settingsObject;


            includeFoldersItemsSource = new ObservableCollection<Bases.ViewModelWrapper<string>>();
            IncludeFoldersItemsSourceView = CollectionViewSource.GetDefaultView(includeFoldersItemsSource);
           
            preprocessorDefinitionsItemsSource = new ObservableCollection<PreprocessorDefinitionViewModel>();
            PreprocessorDefinitionsItemsSourceView = CollectionViewSource.GetDefaultView(preprocessorDefinitionsItemsSource);

            
            //Propertys initialisieren

            DbDatabase = new Bases.ViewModelWrapper<string>(settingsObject.DbDatabase);
            DbDatabase.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            DbUsername = new Bases.ViewModelWrapper<string>(settingsObject.DbUsername);
            DbUsername.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            DbPassword = new Bases.ViewModelWrapper<string>(settingsObject.DbPassword);
            DbPassword.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            DbServer = new Bases.ViewModelWrapper<string>(settingsObject.DbServer);
            DbServer.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            DbUseIs = new Bases.ViewModelWrapper<bool>(settingsObject.DbUseIs);
            DbUseIs.HasChanged += new Bases.ViewModelWrapper<bool>.HasChangedEventHandler(PropertyHasChanged);


            Description = new Bases.ViewModelWrapper<string>(settingsObject.Description);
            Description.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            VerbosityLevel = new Bases.ViewModelWrapper<int>(settingsObject.VerbosityLevel);
            VerbosityLevel.HasChanged += new Bases.ViewModelWrapper<int>.HasChangedEventHandler(PropertyHasChanged);

            Distributionfile = new Bases.ViewModelWrapper<string>(settingsObject.Distributionfile);
            Distributionfile.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            IsDistributionfileEnabled = new Bases.ViewModelWrapper<bool>(settingsObject.IsDistributionfileEnabled);
            IsDistributionfileEnabled.HasChanged += new Bases.ViewModelWrapper<bool>.HasChangedEventHandler(PropertyHasChanged);

            IsPreprocessorEnabled = new Bases.ViewModelWrapper<bool>(settingsObject.IsPreprocessorEnabled);
            IsPreprocessorEnabled.HasChanged += new Bases.ViewModelWrapper<bool>.HasChangedEventHandler(PropertyHasChanged);

            IsBreakOnErrosEnabled = new Bases.ViewModelWrapper<bool>(settingsObject.IsBreakOnErrosEnabled);
            IsPreprocessorEnabled.HasChanged += new Bases.ViewModelWrapper<bool>.HasChangedEventHandler(PropertyHasChanged);

            PreprocessorAdvancedDefinitions = new Bases.ViewModelWrapper<string>(settingsObject.PreprocessorAdvancedDefinitions);
            PreprocessorAdvancedDefinitions.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            TemporaryOutputFile = new Bases.ViewModelWrapper<string>(settingsObject.TemporaryOutputFile);
            TemporaryOutputFile.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);

            IsTemporaryOutputFileEnabled = new Bases.ViewModelWrapper<bool>(settingsObject.IsTemporaryOutputFileEnabled);
            IsTemporaryOutputFileEnabled.HasChanged += new Bases.ViewModelWrapper<bool>.HasChangedEventHandler(PropertyHasChanged);

            ResetChanges();

            #region CommandsDefinitions

            SaveChangesCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => HasChanges,
                ExecuteDelegate = x => AcceptChanges()
            };

            ResetChangesCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => HasChanges,
                ExecuteDelegate = x => ResetChanges()
            };


            AddIncludeFolderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => 
                {
                    Bases.ViewModelWrapper<string> includeFolder = new Bases.ViewModelWrapper<string>("Neuer Eintrag");
                    includeFoldersItemsSource.Add(includeFolder);
                    IncludeFoldersItemsSourceView.MoveCurrentTo(includeFolder);
                }
            };

            RemoveIncludeFolderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => (IncludeFoldersItemsSourceView.CurrentItem != null),
                ExecuteDelegate = x => includeFoldersItemsSource.Remove(IncludeFoldersItemsSourceView.CurrentItem as Bases.ViewModelWrapper<string>)
            };



            AddPreprocessorDefinitionCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => preprocessorDefinitionsItemsSource.Add(new PreprocessorDefinitionViewModel(new PreprocessorDefinition()))
            };

            RemovePreprocessorDefinitionCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => (PreprocessorDefinitionsItemsSourceView.CurrentItem != null),
                ExecuteDelegate = x => preprocessorDefinitionsItemsSource.Remove(PreprocessorDefinitionsItemsSourceView.CurrentItem as PreprocessorDefinitionViewModel)
            };


            DatabaseConnectionTestCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => DatabaseConnectionTest()
            };


            ChooseDistributionfileCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => ChooseDistributionfile()
            };

            ChooseTemporaryOutputFileCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => ChooseTemporaryOutputFile()
            };


            ExecuteAllCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x => GeneratePreprocessorArgs().Log()
            };

            ChooseIncludeFolderCommand = new SimpleCommand
            {
                CanExecuteDelegate = x => (this.IncludeFoldersItemsSourceView.CurrentItem != null),
                ExecuteDelegate = x => ChooseIncludeFolder()
            };

            #endregion
        }

        void PropertyHasChanged()
        {
            RaisePropertyChanged("HasChanges");
        }



        void ChooseIncludeFolder()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();

            if (folderBrowserDialog.SelectedPath != "")
            {
                var item = (IncludeFoldersItemsSourceView.CurrentItem as Bases.ViewModelWrapper<string>);
                item.Value = folderBrowserDialog.SelectedPath;
                item.ForceRaisPropertyChanged();
            }
        }


        void ChooseDistributionfile()
        {            
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                Distributionfile.Value = saveFileDialog.FileName;
                Distributionfile.ForceRaisPropertyChanged();
            }
        }

        void ChooseTemporaryOutputFile()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                TemporaryOutputFile.Value = saveFileDialog.FileName;
                TemporaryOutputFile.ForceRaisPropertyChanged();
            }
        }

        public void AcceptChanges()
        {
            Description.AcceptChanges();
            settingsObject.Description = Description.Value;

            VerbosityLevel.AcceptChanges();
            settingsObject.VerbosityLevel = VerbosityLevel.Value;


            DbUsername.AcceptChanges();
            settingsObject.DbUsername = DbUsername.Value;

            DbPassword.AcceptChanges();
            settingsObject.DbPassword = DbPassword.Value;

            DbServer.AcceptChanges();
            settingsObject.DbServer = DbServer.Value;

            DbDatabase.AcceptChanges();
            settingsObject.DbDatabase = DbDatabase.Value;

            DbUseIs.AcceptChanges();
            settingsObject.DbUseIs = DbUseIs.Value;



            settingsObject.PreprocessorDefinitions.Clear();
            foreach (PreprocessorDefinitionViewModel item in this.preprocessorDefinitionsItemsSource)
            {
                item.AcceptChanges();
                settingsObject.PreprocessorDefinitions.Add(item.PreprocessorDefinition);
            }

            settingsObject.IncludeFolders.Clear();
            foreach (Bases.ViewModelWrapper<string> item in this.includeFoldersItemsSource)
            {
                item.AcceptChanges();
                settingsObject.IncludeFolders.Add(item.Value);
            }

            Distributionfile.AcceptChanges();
            settingsObject.Distributionfile = Distributionfile.Value;

            IsDistributionfileEnabled.AcceptChanges();
            settingsObject.IsDistributionfileEnabled = IsDistributionfileEnabled.Value;

            IsPreprocessorEnabled.AcceptChanges();
            settingsObject.IsPreprocessorEnabled = IsPreprocessorEnabled.Value;

            IsBreakOnErrosEnabled.AcceptChanges();
            settingsObject.IsBreakOnErrosEnabled = IsBreakOnErrosEnabled.Value;

            PreprocessorAdvancedDefinitions.AcceptChanges();
            settingsObject.PreprocessorAdvancedDefinitions = PreprocessorAdvancedDefinitions.Value;

            TemporaryOutputFile.AcceptChanges();
            settingsObject.TemporaryOutputFile = TemporaryOutputFile.Value;

            IsTemporaryOutputFileEnabled.AcceptChanges();
            settingsObject.IsTemporaryOutputFileEnabled = IsTemporaryOutputFileEnabled.Value;

            //Event auslösen
            OnSaveChanges();
        }

        public void ResetChanges()
        {
            this.includeFoldersItemsSource.Clear();
            foreach (string item in settingsObject.IncludeFolders)
            {
                Bases.ViewModelWrapper<string> stringViewModel = new Bases.ViewModelWrapper<string>(item);
                stringViewModel.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(PropertyHasChanged);
                this.includeFoldersItemsSource.Add(stringViewModel);
            }

            this.preprocessorDefinitionsItemsSource.Clear();
            foreach (PreprocessorDefinition item in settingsObject.PreprocessorDefinitions)
            {
                PreprocessorDefinitionViewModel preprocessorDefinitionViewModel = new PreprocessorDefinitionViewModel(item);
                preprocessorDefinitionViewModel.HasChanged += new PreprocessorDefinitionViewModel.HasChangedEventHandler(PropertyHasChanged);
                this.preprocessorDefinitionsItemsSource.Add(preprocessorDefinitionViewModel);
            }

            Description.ResetChanges();
            VerbosityLevel.ResetChanges();

            DbUsername.ResetChanges();
            DbPassword.ResetChanges();
            DbServer.ResetChanges();
            DbDatabase.ResetChanges();
            DbUseIs.ResetChanges();


            Distributionfile.ResetChanges();
            IsDistributionfileEnabled.ResetChanges();
            IsPreprocessorEnabled.ResetChanges();
            IsBreakOnErrosEnabled.ResetChanges();

            PreprocessorAdvancedDefinitions.ResetChanges();
            TemporaryOutputFile.ResetChanges();
            IsTemporaryOutputFileEnabled.ResetChanges();

            this.RaiseAllPropertiesChanged();
        }

        private void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged("HasChanges");
        }

        private void DatabaseConnectionTest()
        {
            try
            {

                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();

                sqlConnectionStringBuilder.Password = DbPassword.Value;
                sqlConnectionStringBuilder.UserID = DbUsername.Value;
                sqlConnectionStringBuilder.DataSource = DbServer.Value;
                sqlConnectionStringBuilder.IntegratedSecurity = DbUseIs.Value;
                sqlConnectionStringBuilder.InitialCatalog = DbDatabase.Value;

                sqlConnectionStringBuilder.ConnectTimeout = 1;

                SqlConnection sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);

                sqlConnection.Open();
                sqlConnection.Close();

                Globals.Messages.ShowInformation("Information", "Datenbankverbindung konnte erfolgreich hergestellt werden");
            }
            catch (Exception ex)
            {
                Globals.Messages.ShowError("Datenbankverbindung konnte nicht hergestellt werden", ex.Message);
            }



        }


        #region Events

        public delegate void SaveChangesEventHanlder();
        public event SaveChangesEventHanlder SaveChanges;

        public void OnSaveChanges()
        {
            if (SaveChanges != null) SaveChanges();
        }

        #endregion


        #region Parameter Generation

     
        public string GeneratePreprocessorArgs()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.PreprocessorAdvancedDefinitions.Value + " ");

            foreach (PreprocessorDefinitionViewModel item in this.preprocessorDefinitionsItemsSource)
            {
                if (item.IsEnabled.Value)
                    stringBuilder.Append("/D" + item.Key.Value + "=" + item.Value.Value + " ");
            }

            foreach (var item in this.includeFoldersItemsSource)
            {
                stringBuilder.Append("/I" + item.Value + " ");
            }


            return stringBuilder.ToString();
        }

        #endregion

    }
}

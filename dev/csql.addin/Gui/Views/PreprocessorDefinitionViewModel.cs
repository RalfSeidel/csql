
using csql.addin.Settings;
namespace csql.addin.Gui.Views
{
    public class PreprocessorDefinitionViewModel : Bases.ViewModel
    {
        PreprocessorDefinition preprocessorDefinition;
        public PreprocessorDefinition PreprocessorDefinition { get { return preprocessorDefinition; } }

        public Bases.ViewModelWrapper<bool> IsEnabled { get; private set; }
        public Bases.ViewModelWrapper<string> Key { get; private set; }
        public Bases.ViewModelWrapper<string> Value { get; private set; }

        public bool HasChanges 
        {
            get
            {
                return (IsEnabled.HasChanges || Key.HasChanges || Value.HasChanges);
            }
        }

        public PreprocessorDefinitionViewModel(PreprocessorDefinition preprocessorDefinition)
        {
            this.preprocessorDefinition = preprocessorDefinition;

            //Property initialisieren
            IsEnabled = new Bases.ViewModelWrapper<bool>(preprocessorDefinition.IsEnabled);
            IsEnabled.HasChanged += new Bases.ViewModelWrapper<bool>.HasChangedEventHandler(OnHasChanged);

            Key = new Bases.ViewModelWrapper<string>(preprocessorDefinition.Key);
            Key.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(OnHasChanged);

            Value = new Bases.ViewModelWrapper<string>(preprocessorDefinition.Value);
            Value.HasChanged += new Bases.ViewModelWrapper<string>.HasChangedEventHandler(OnHasChanged);

        }

        


        public delegate void HasChangedEventHandler();
        public event HasChangedEventHandler HasChanged;

        public void OnHasChanged()
        {
            if (HasChanged != null) HasChanged();
        }

        public void AcceptChanges()
        {
            IsEnabled.AcceptChanges();
            preprocessorDefinition.IsEnabled = IsEnabled.Value;

            Key.AcceptChanges();
            preprocessorDefinition.Key = Key.Value;
            
            Value.AcceptChanges();
            preprocessorDefinition.Value = Value.Value;
        }

        public void ResetChanges()
        {
            IsEnabled.ResetChanges();
            Key.ResetChanges();
            Value.ResetChanges();
        }
    }
}

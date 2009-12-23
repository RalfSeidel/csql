
namespace csql.addin.Settings
{
	/// <summary>
	/// Macro definition for the pre processor (sqtpp)
	/// </summary>
    public class PreprocessorDefinition
    {
        public string Key { get; set; }
        public string Value { get; set; }
		public bool IsEnabled { get; set; }

        public PreprocessorDefinition()
        {
            IsEnabled = true;
            Key = "Name";
            Value = string.Empty;
        }

        public PreprocessorDefinition(PreprocessorDefinition preprocessorDefinition)
        {
            this.IsEnabled = preprocessorDefinition.IsEnabled;
            this.Key = preprocessorDefinition.Key;
            this.Value = preprocessorDefinition.Value;
        }
    }
}

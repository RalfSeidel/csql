using System.Collections.Generic;
using System.ComponentModel;

namespace csql.addin.Settings.Gui
{
	/// <summary>
	/// A pseudo converter that provides a list of predefined values for a string property.
	/// </summary>
	internal class StringDomainConverter : StringConverter
	{
		public override bool GetStandardValuesSupported( ITypeDescriptorContext context )
		{
			return true;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
		{
			List<string> domainValues = new List<string>();
			domainValues.Add( "test1" );
			domainValues.Add( "test2" );

			StandardValuesCollection result = new StandardValuesCollection( domainValues );
			return result;
		}
	}
}

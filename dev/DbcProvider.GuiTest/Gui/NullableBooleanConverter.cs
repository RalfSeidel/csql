using System;
using System.Globalization;
using System.Windows.Data;

namespace Sqt.DbcProvider.Gui
{
	/// <summary>
	/// 
	/// </summary>
	[ValueConversion( typeof( bool? ), typeof( bool ) )]
	public class NullableBooleanConverter : IValueConverter
	{
		/// <inheritdoc/>
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			if ( value == null ) {
				return false;
			} else {
				bool param = bool.Parse( parameter.ToString() );
				return !((bool)value ^ param);
			}
		}

		/// <inheritdoc/>
		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			bool param = bool.Parse( parameter.ToString() );
			return !((bool)value ^ param);
		}
	}
}

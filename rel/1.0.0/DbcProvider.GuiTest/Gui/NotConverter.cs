using System;
using System.Globalization;
using System.Windows.Data;

namespace Sqt.DbcProvider.Gui
{
	/// <summary>
	/// Invert/negate boolean values.
	/// </summary>
	[ValueConversion( typeof( bool ), typeof( bool ) )]
	public class NotConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return !((bool)value);
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return !((bool)value);
		}
	}
}

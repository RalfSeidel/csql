using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace csql.addin.Settings.Gui
{
	/// <summary>
	/// Convert a collection of string to a single string of semicolon separated items and back.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible( true )]
	internal class PathCollectionConverter : TypeConverter
	{
		/// <summary>
		/// Gets a value indicating whether this converter can convert an object in the given source type to a string using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you wish to convert from.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
		{
			if ( sourceType == typeof( string ) ) {
				return true;
			}
			else {
				return base.CanConvertFrom( context, sourceType );
			}
		}


		/// <summary>
		/// Convert from a semicolon separated string to a list of string.
		/// </summary>
		public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
		{
			string stringValue = value as string;
			if ( stringValue != null ) {
				ICollection<string> result = new List<String>();
				string[] items = stringValue.Split( ';' );
				foreach ( var i in items ) {
					string item = i.Trim();
					if ( item.Length == 0 )
						continue;

					result.Add( item );
				}
				return result;
			}
			else {
				return base.ConvertFrom( context, culture, value );
			}
		}


		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
		{
			if ( destinationType == typeof( string ) ) {
				return true;
			}
			else {
				return base.CanConvertTo( context, destinationType );
			}
		}

		/// <summary>
		/// Convert from a list of string to a string of semicolon separated value.
		/// </summary>
		public override object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType )
		{
			if ( destinationType == typeof( string ) ) {
				IEnumerable<string> items = (IEnumerable<string>)value;
				StringBuilder sb = new StringBuilder();
				string separator = "";
				foreach ( string item in items ) {
					sb.Append( separator );
					sb.Append( item );

					separator = ";";
				}
				return sb.ToString();
			}
			else {
				return base.ConvertTo( context, culture, value, destinationType );
			}
		}

		/// <summary>
		/// Check if the given type is compatible with IEnumerable&lt;string&gt;
		/// </summary>
		private bool IsEnumerableStringType( Type type )
		{
			if ( !type.IsGenericType )
				return false;

			Type[] genericArguments = type.GetGenericArguments();
			if ( genericArguments.Length != 1 )
				return false;

			Type genericArgument = genericArguments[0];
			if ( genericArgument != typeof( string ) )
				return false;

			Type enumerableType = typeof( IEnumerable<> ).MakeGenericType( genericArguments );
			if ( !enumerableType.IsAssignableFrom( type ) )
				return false;

			return true;
		}
	}
}
